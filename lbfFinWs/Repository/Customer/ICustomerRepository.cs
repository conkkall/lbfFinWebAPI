using lbfFinWs.Models;


namespace lbfFinWs.Repository.Customer
{
    public interface ICustomerRepository
    {
        Task<List<CustomersModel>> GetAllCustomers();
        CustomersModel ValidateUser(CustomerLoginModel login);
        CustomersModel GetCustomerByCustId(TokenModel token);
        AuthTokenModel GetTokenByCustId(CustomersModel? cust);
        string CreateNewCustomer(CustomersModel customer);
        string CreateRefreshToken(ApplicationUser user);
        CustomersModel GetCustomerByCustId(int custId);
    }
}
