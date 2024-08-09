using lbfFinWs.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace lbfFinWs.Data
{
    public class ApplicationDBContext :  IdentityDbContext<ApplicationUser>
    {
        private readonly string _connectionString;

        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options)
        : base(options)
        {

        }

        public DbSet<CustomerLoginModel> Login { get; set; }
        public DbSet<CustomersModel> Customers { get; set; }
        public DbSet<PoliciesModel> PolicyDetails { get; set; }
        public DbSet<AuthTokenModel> AuthToken { get; set; }
    }
}
