namespace TwoFactorAuthentication.DTOs
{
    public class VerifyDTO
    {
        public string Username { get; set; }
        public string TwoFactorCode { get; set; }
    }
}
