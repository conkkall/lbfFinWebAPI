using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace lbfFinWs.Models
{
    public class AuthTokenModel
    {
        [Key]
        public int TokenId { get; set; }
        public int CustomerId { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
    }
}
