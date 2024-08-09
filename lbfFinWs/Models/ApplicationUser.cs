using Microsoft.AspNetCore.Identity;

namespace lbfFinWs.Models
{
    public class ApplicationUser : IdentityUser
    {
        public int TokenId { get; set; }
        public int CustomerId { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
    }
}
