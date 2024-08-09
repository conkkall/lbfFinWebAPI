namespace lbfFinWs.Models
{
    public class TokenModel
    {
        public int CustomerId { get; set; }
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
    }
}
