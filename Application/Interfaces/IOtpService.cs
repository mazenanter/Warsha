namespace Application.Interfaces
{
    public interface IOtpService
    {
        string GenerateOtp(int length = 6);
    }
}
