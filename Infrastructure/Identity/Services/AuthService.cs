using Application.Features.Auth.DTOs;
using Application.Interfaces;
using Domain.Common;
using Domain.Constants;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Infrastructure.Identity.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly IJwtService _jwtService;
        private readonly IOtpService _otpService;

        public AuthService(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork, RoleManager<IdentityRole<int>> roleManager, IJwtService jwtService, IOtpService otpService)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _roleManager = roleManager;
            _jwtService = jwtService;
            _otpService = otpService;
        }

        

        public async Task<Result<AuthResult>> ClientRegisterAsync(ClientRegisterRequest registerRequest)
        {

            var user = await _userManager.FindByEmailAsync(registerRequest.Email);
            if (user != null)
            {
                if (user.EmailConfirmed)
                {
                    return Result<AuthResult>.Failure("Email is already exists");

                }



                var newOtp = _otpService.GenerateOtp();

                user.EmailVerificationOtp = newOtp;
                user.OtpExpiryTime = DateTime.UtcNow.AddMinutes(10);

                await _userManager.UpdateAsync(user);

                return Result<AuthResult>.Success(new AuthResult
                {
                    UserId = user.Id,
                    CanResendOtp = true,
                    OTP = newOtp
                }, "Verification code sent again");


            }
                var otp = _otpService.GenerateOtp();
                var otpExpiry = DateTime.UtcNow.AddMinutes(10);
                var appUser = new ApplicationUser
                {
                    Email = registerRequest.Email,
                    PhoneNumber = registerRequest.PhoneNumber,
                    EmailVerificationOtp = otp,
                    IsActive = true,
                    UserType = UserType.CLIENT,
                    EmailConfirmed = false,
                    UserName = registerRequest.Email,
                    OtpExpiryTime = otpExpiry,
                };
                var result = await _userManager.CreateAsync(appUser, registerRequest.Password);
                if (!result.Succeeded)
                {
                    return Result<AuthResult>.Failure(result.Errors.Select(e => e.Description).ToList(), "Registration failed");
                }
                await _userManager.AddToRoleAsync(appUser, Roles.Client);
                var client = Client.Create(appUser.Id, registerRequest.Name, registerRequest.Email, registerRequest.PhoneNumber);
                await _unitOfWork.Clients.AddAsync(client);
                await _unitOfWork.SaveChangesAsync();
                var authResult = new AuthResult
                {
                    UserId = appUser.Id,
                    OTP = otp

                };
                return Result<AuthResult>.Success(authResult, "Registration Successful");
            

        }

        public async Task<Result> ConfirmEmailAsync(string email, string otp)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return Result.Failure("User not found");
            }
            if(user.EmailConfirmed)
            {
                return Result.Failure("Email is already confirmed");
            }
            if (user.EmailVerificationOtp != otp)
                return Result.Failure("Invalid otp code");
            if (user.OtpExpiryTime < DateTime.UtcNow)
                return Result.Failure("Expired otp code try resend another one");
            user.EmailConfirmed = true;
            user.EmailVerificationOtp = null;
            user.OtpExpiryTime = null;
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
                return Result.Success("Email confirmed successfully. You can now login.");
            return Result.Failure("An error occurred during verification.");
        }

        public async Task<Result<AuthResult>> ForgotPassword(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return Result<AuthResult>.Failure("User not found");
            }
            var otp = _otpService.GenerateOtp();

            user.EmailVerificationOtp = otp;
            user.OtpExpiryTime = DateTime.UtcNow.AddMinutes(10);
            await _userManager.UpdateAsync(user);
            var authResult = new AuthResult
            {
                UserId = user.Id,
                OTP = otp
            };
            return Result<AuthResult>.Success(authResult, "OTP sent successfully check your email");
        }

        public async Task<Result<AuthResult>> ClientLoginAsync(ClientLoginRequest clientLoginRequest)
        {
            var user = await _userManager.FindByEmailAsync(clientLoginRequest.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, clientLoginRequest.Password))
            {
                return Result<AuthResult>.Failure("Invalid email or password");
            }
            if (!user.EmailConfirmed)
                return Result<AuthResult>.Failure("Please confirm your email first");
            if (!user.IsActive)
                return Result<AuthResult>.Failure("Your email is not active please contact support");
            var client = await _unitOfWork.Clients.FindAsync(c => c.UserId == user.Id);

            var roles = await _userManager.GetRolesAsync(user);
            var token = _jwtService.GenerateAccessToken(user.Id, clientLoginRequest.Email, roles,clientId: client?.Id);
            user.RefreshTokens ??= new List<RefreshToken>();
            var activeRefreshToken = user.RefreshTokens.FirstOrDefault(i => i.IsActive);
            var authResult = new AuthResult();
            if (activeRefreshToken != null)
            {
                authResult.RefreshToken = activeRefreshToken.Token;
            }
            else
            {
                var refreshToken = _jwtService.GenerateRefreshToken();
                user.RefreshTokens.Add(refreshToken);
                await _userManager.UpdateAsync(user);
                authResult.RefreshToken = refreshToken.Token;
            }
            authResult.AccessToken = token;
            authResult.Email = clientLoginRequest.Email;
            authResult.UserId = user.Id;
            authResult.ExpiresAt = DateTime.UtcNow.AddHours(12);
            await _userManager.UpdateAsync(user);
            return Result<AuthResult>.Success(authResult, "Login successful");
        }

        public async Task<Result<AuthResult>> RefreshTokenAsync(string token)
        {
            var user = await _userManager.Users.Include(u => u.RefreshTokens).FirstOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));
            if(user == null)
            {
                return Result<AuthResult>.Failure("Invalid token");
            }
            var rT = user.RefreshTokens.Single(t => t.Token == token);
            if (!rT.IsActive)
            {
                return Result<AuthResult>.Failure("Token is inactive (expired or revoked)");
            }
            if (!user.IsActive)
            {
                return Result<AuthResult>.Failure("Your account is not active. Please contact support.");
            }
            int? workshopId = null;
            int? clientId = null;

            if (user.UserType == UserType.WORKSHOP)
            {
                var workshopEntity = await _unitOfWork.Workshops
                    .FindAsync(w => w.UserId == user.Id);
                workshopId = workshopEntity?.Id;
            }
            else if (user.UserType == UserType.CLIENT)
            {
                var clientEntity = await _unitOfWork.Clients
                    .FindAsync(c => c.UserId == user.Id);
                clientId = clientEntity?.Id;
            }
            rT.Revoke();
            var roleList = await _userManager.GetRolesAsync(user);
            var accessToken = _jwtService.GenerateAccessToken(user.Id, user.Email!, roleList);
            var newRefreshToken = _jwtService.GenerateRefreshToken();
            user.RefreshToken = newRefreshToken.Token;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            user.RefreshTokens.Add(newRefreshToken);

            await _userManager.UpdateAsync(user);

            var authResult = new AuthResult
            {
                UserId = user.Id,
                Email = user.Email,
                AccessToken = accessToken,
                RefreshToken = newRefreshToken.Token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(10),
              
            };

            return Result<AuthResult>.Success(authResult, "Token refreshed successfully");
        }

        public async Task<Result<AuthResult>> ResendOtp(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return Result<AuthResult>.Failure("User not found");
            if (user.EmailConfirmed)
                return Result<AuthResult>.Failure("This email already confirmed please login");
            var otp = _otpService.GenerateOtp();
            user.EmailVerificationOtp = otp;
            user.OtpExpiryTime = DateTime.UtcNow.AddMinutes(10);
            await _userManager.UpdateAsync(user);
            var authResult = new AuthResult
            {
                UserId = user.Id,
                OTP = otp,
          
            };
            return Result<AuthResult>.Success(authResult, "OTP  resend successfully");
        }

       

        public async Task<Result> RevokeTokenAsync(string token)
        {
            var user = await _userManager.Users.Include(x => x.RefreshTokens).FirstOrDefaultAsync(x => x.RefreshTokens.Any(x => x.Token == token));
            if (user == null) return Result.Failure("User not found with this token");
            var refreshToken = user.RefreshTokens.Single(x => x.Token == token);
            if (!refreshToken.IsActive) return Result.Failure("Token is not active");
            refreshToken.Revoke();
            await _userManager.UpdateAsync(user);
            return Result.Success("Logout successful");
        }

        public async Task<Result<AuthResult>> WorkshopLoginAsync(WorkshopLoginRequest workshopLoginRequest)
        {
            var workshop = await _userManager.FindByEmailAsync(workshopLoginRequest.Email);
            if(workshop == null || !await _userManager.CheckPasswordAsync(workshop, workshopLoginRequest.Password))
            {
                return Result<AuthResult>.Failure("Invalid email or password");
            }
            if(!workshop.IsActive)
            {
                return Result<AuthResult>.Failure("Your account is not active. Please contact support.");
            }
            var workshopEntity = await _unitOfWork.Workshops.FindAsync(x=>x.UserId == workshop.Id);
            if(workshopEntity.IsVerified == false)
            {
                return Result<AuthResult>.Failure("Your account is not verified yet. Please contact support.");
            }
            var roles = await _userManager.GetRolesAsync(workshop);
            var token = _jwtService.GenerateAccessToken(workshop.Id, workshopLoginRequest.Email, roles,workshopId: workshopEntity.Id);
            workshop.RefreshTokens ??= new List<RefreshToken>();
            var activeRefreshToken = workshop.RefreshTokens.FirstOrDefault(i => i.IsActive);
            var authResult = new AuthResult();
            if (activeRefreshToken != null)
            {
                authResult.RefreshToken = activeRefreshToken.Token;
            }
            else
            {
                var refreshToken = _jwtService.GenerateRefreshToken();
                workshop.RefreshTokens.Add(refreshToken);
                await _userManager.UpdateAsync(workshop);
                authResult.RefreshToken = refreshToken.Token;
            }
            authResult.AccessToken = token;
            authResult.Email = workshopLoginRequest.Email;
            authResult.UserId = workshop.Id;
            authResult.ExpiresAt = DateTime.UtcNow.AddHours(12);
            await _userManager.UpdateAsync(workshop);
            return Result<AuthResult>.Success(authResult, "Login successful");
        }

        public async Task<Result<AuthResult>> WorkShopRegisterAsync(WorkshopRegisterRequest registerRequest)
        {
            var userName = string.Concat(
    registerRequest.Name.Split(' ', StringSplitOptions.RemoveEmptyEntries)
).ToLower();
            var workshop = await _userManager.FindByEmailAsync(registerRequest.Email);
            if(workshop is not null)
            {
                return Result<AuthResult>.Failure("Workshop email already exists");

            }
            var nameExist = await _userManager.FindByNameAsync(userName);

            if (nameExist is not null)
            {
                return Result<AuthResult>.Failure("Workshop name already exists");
            }

            var appUser = new ApplicationUser
            {
                UserName = userName,
                Email = registerRequest.Email,
                UserType = UserType.WORKSHOP,
                PhoneNumber = registerRequest.Phone,
                EmailConfirmed = true,

            };
            var result = await _userManager.CreateAsync(appUser, registerRequest.Password);
            if (!result.Succeeded)
            {
                return Result<AuthResult>.Failure(result.Errors.Select(e => e.Description).ToList(), "Registration failed");
            }
            await _userManager.AddToRoleAsync(appUser,Roles.Workshop);
        //    if (!TimeOnly.TryParseExact(
        //registerRequest.OpeningTime,
        //"hh:mm tt",
        //CultureInfo.InvariantCulture,
        //DateTimeStyles.None,
        //out var openingTime))
        //    {
        //        throw new ValidationException("Opening time format is invalid.");
        //    }

        //    if (!TimeOnly.TryParseExact(
        //            registerRequest.ClosingTime,
        //            "hh:mm tt",
        //            CultureInfo.InvariantCulture,
        //            DateTimeStyles.None,
        //            out var closingTime))
        //    {
        //        throw new ValidationException("Closing time format is invalid.");
        //    }
            var newWorkshop = Workshop.Create(appUser.Id,registerRequest.Email, registerRequest.Name, registerRequest.Phone, registerRequest.Address);
            await _unitOfWork.Workshops.AddAsync(newWorkshop);
            await _unitOfWork.SaveChangesAsync();
            var authResult = new AuthResult
            {
                UserId = appUser.Id,
                
            };
            return Result<AuthResult>.Success(authResult, "Registration Successful");

        }

        public async Task<Result> ResetPassword(Application.Features.Auth.DTOs.ResetPasswordRequest resetPasswordRequest)
        {
           
            var user = await _userManager.FindByEmailAsync(resetPasswordRequest.Email);
            if (user is null)
                return Result.Failure("User not found");

            if (resetPasswordRequest.Otp != user.EmailVerificationOtp)
                return Result.Failure("Invalid OTP code");

            if (user.OtpExpiryTime < DateTime.UtcNow)
                return Result.Failure("OTP has expired, please request a new one");

            foreach (var validator in _userManager.PasswordValidators)
            {
                var validationResult = await validator.ValidateAsync(
                    _userManager, user, resetPasswordRequest.NewPassword);

                if (!validationResult.Succeeded)
                    return Result.Failure(
                        validationResult.Errors.Select(e => e.Description).ToList(),
                        "Password does not meet requirements");
            }

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(
                user, resetToken, resetPasswordRequest.NewPassword);

            if (!result.Succeeded)
                return Result.Failure(
                    result.Errors.Select(e => e.Description).ToList(),
                    "Password reset failed");

            user.EmailVerificationOtp = null;
            user.OtpExpiryTime = null;
            await _userManager.UpdateAsync(user);

            return Result.Success("Password reset successfully");
        }
    }
}
