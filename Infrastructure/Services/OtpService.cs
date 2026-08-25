using Application.Interfaces;
using System.Security.Cryptography;

namespace Infrastructure.Services
{
    public class OtpService : IOtpService
    {
        public string GenerateOtp(int length = 6)
        {
            const string numbers = "0123456789";
            var result = new char[length];

            using (var rng = RandomNumberGenerator.Create())
            {
                var data = new byte[length];
                rng.GetBytes(data);

                for (int i = 0; i < length; i++)
                {
                    result[i] = numbers[data[i] % numbers.Length];
                }
            }

            return new string(result);
        }
    }
}
