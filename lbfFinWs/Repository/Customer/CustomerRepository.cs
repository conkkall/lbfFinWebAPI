using lbfFinWs.Common;
using lbfFinWs.Data;
using lbfFinWs.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json.Linq;
//using System.Data.SqlClient;

namespace lbfFinWs.Repository.Customer
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly ApplicationDBContext _appDbContext;
        public CustomerRepository(ApplicationDBContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public async Task<List<CustomersModel>> GetAllCustomers()
        {
            string sql = "EXEC GETALLCUSTOMERS";
            var records = await _appDbContext.Customers.FromSqlRaw(sql).ToListAsync();

            //var records = await _appDbContext.Customers.Select(x => new CustomersModel()
            //{
            //    CustomerId = x.CustomerId,
            //    Emailid = x.Emailid,
            //    Password = x.Password,
            //    CreatedDate = x.CreatedDate,
            //    ModifiedDate = x.ModifiedDate,
            //    Status = x.Status
            //}).ToListAsync();
            return records;
        }

        public string CreateNewCustomer(CustomersModel customer)
        {
            List<CustomersModel> list;
            var encPassword = EncryptionHelper.Encrypt(customer.Password);
            var cust = new CustomersModel { Emailid = customer.Emailid, Password = encPassword, CreatedDate = DateTime.Now, ModifiedDate = DateTime.Now, Status = "Active" };
            var res = _appDbContext.Customers.Add(cust);
            _appDbContext.SaveChanges();

            //string sql = "EXEC CreateNewCustomer @EmailId, @Password, @Status";
            ////var records = await _appDbContext.Customers.FromSqlRaw<CustomersModel>(sql).ToListAsync();
            //List<SqlParameter> parms = new List<SqlParameter>
            //{
            //    // Create parameter(s)    
            //    new SqlParameter { ParameterName = "@EmailId", Value = customer.Emailid },
            //    new SqlParameter { ParameterName = "@Password", Value = encPassword },
            //    new SqlParameter { ParameterName = "@Status", Value = "Active" }
            //};

            //list = _appDbContext.Customers.FromSqlRaw<CustomersModel>(sql, parms.ToArray()).ToList();

            return "Added Successfully";
        }

        public CustomersModel ValidateUser(CustomerLoginModel login)
        {
            CustomersModel obj = new CustomersModel();
            var encPassword = EncryptionHelper.Encrypt(login.Password);
            obj = _appDbContext.Customers.Where(s => s.Emailid == login.Emailid && s.Password == encPassword).FirstOrDefault();
            //if (customer != null)
            //    return "Valid User";
            //else
            //    return "Invalid User";
            return obj;
        }

        public CustomersModel GetCustomerByCustId(TokenModel token)
        {
            CustomersModel obj = new CustomersModel();
            obj = _appDbContext.Customers.Where(s => s.CustomerId == token.CustomerId).FirstOrDefault();
            return obj;
        }

        public CustomersModel GetCustomerByCustId(int custId)
        {
            CustomersModel obj = new CustomersModel();
            obj = _appDbContext.Customers.Where(s => s.CustomerId == custId).FirstOrDefault();
            return obj;
        }

        public AuthTokenModel GetTokenByCustId(CustomersModel? cust)
        {
            AuthTokenModel obj = new AuthTokenModel();
            obj = _appDbContext.AuthToken.Where(s => s.CustomerId == cust.CustomerId).FirstOrDefault();
            return obj;
        }

        public string CreateRefreshToken(ApplicationUser user)
        {
            //List<CustomersModel> list;
            //var encPassword = EncryptionHelper.Encrypt(customer.Password);
            //var cust = new CustomersModel { Emailid = customer.Emailid, Password = encPassword, CreatedDate = DateTime.Now, ModifiedDate = DateTime.Now, Status = "Active" };
            //var res = _appDbContext.Customers.Add(cust);
            //_appDbContext.SaveChanges();
            int rowsAffected;
            //var custId = new SqlParameter("@CustomerId", user.CustomerId);
            //var token = new SqlParameter("@RefreshToken", user.RefreshToken);
            //var tokenExp = new SqlParameter("@TokenExpiryTime", user.RefreshTokenExpiryTime);
            //_appDbContext.Database.ExecuteSqlCommand();
            //_appDbContext.ExecuteSqlCommand("exec messageinsert @Date , @Subject , @Body , @Fid", date, subject, body, fid);


            string sql = "EXEC CreateRefreshToken @CustomerId, @RefreshToken, @TokenExpiryTime";
            //var records = await _appDbContext.Customers.FromSqlRaw<CustomersModel>(sql).ToListAsync();
            List<SqlParameter> parms = new List<SqlParameter>
            {
                // Create parameter(s)    
                new SqlParameter { ParameterName = "@CustomerId", Value = user.CustomerId },
                new SqlParameter { ParameterName = "@RefreshToken", Value = user.RefreshToken },
                new SqlParameter { ParameterName = "@TokenExpiryTime", Value = user.RefreshTokenExpiryTime }
            };

            //list = _appDbContext.Customers.FromSqlRaw<CustomersModel>(sql, parms.ToArray()).ToList();
            rowsAffected = _appDbContext.Database.ExecuteSqlRaw(sql, parms.ToArray());
            return "Added Successfully";
        }
    
        
    }
}
