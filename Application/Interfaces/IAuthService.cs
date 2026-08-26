using Application.Features.Auth.DTOs;
using Domain.Common;

namespace Application.Interfaces
{
    public interface IAuthService
    {
        public Task<Result<AuthResult>> ClientRegisterAsync(ClientRegisterRequest registerRequest);
        public Task<Result<AuthResult>> WorkShopRegisterAsync(WorkshopRegisterRequest registerRequest);
        public Task<Result<AuthResult>> ClientLoginAsync(ClientLoginRequest clientLoginRequest);
        public Task<Result<AuthResult>> WorkshopLoginAsync(WorkshopLoginRequest workshopLoginRequest);
        public Task<Result> ConfirmEmailAsync(string email, string otp);
        public Task<Result<AuthResult>> ResendOtp(string email);
        public Task<Result<AuthResult>> ForgotPassword(string email);
        public Task<Result> ResetPassword(ResetPasswordRequest resetPasswordRequest);
        public Task<Result> RevokeTokenAsync(string token);
        public Task<Result<AuthResult>> RefreshTokenAsync(string token);
        public Task<Result<AuthResult>> AdminLoginAsync(AdminLoginRequest adminLoginRequest);
    }
}
