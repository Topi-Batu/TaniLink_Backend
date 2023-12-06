namespace TaniLink_Backend.Interfaces
{
    public interface ISendMailRepository
    {
        Task<bool> SendVerificationEmail(string email);
        Task<bool> SendResetPasswordEmail(string email, string token);
    }
}
