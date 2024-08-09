using lbfFinWs.Models;


namespace lbfFinWs.Repository.Policy
{
    public interface IPolicyRepository
    {
        Task<List<PoliciesModel>> GetAllPolicies();
        string CreateNewPolicy(PoliciesModel policy);
    }
}
