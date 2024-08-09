using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace lbfFinWs.Models
{
    public class CustomersModel
    {
        [Key]
        public int CustomerId { get; set; }
        public string? Emailid { get; set; }
        public string? Password { get; set; }
        public string? Role { get; set; }
        public string? Phone { get; set; }
        public int? SiteId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string Status { get; set; }
    }

    [Keyless]
    public class CustomerLoginModel
    {
        [Required(ErrorMessage = "EmailId is required")]
        public string? Emailid { get; init; }

        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; init; }
    }
}
