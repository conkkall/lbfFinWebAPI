using lbfFinWs.Common;
using lbfFinWs.Data;
using lbfFinWs.Models;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;

namespace lbfFinWs.Repository.Policy
{
    public class PolicyRepository : IPolicyRepository
    {
        private readonly ApplicationDBContext _appDbContext;
        public PolicyRepository(ApplicationDBContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public async Task<List<PoliciesModel>> GetAllPolicies()
        {
            string sql = "EXEC GETALLPOLICIES";
            var records = await _appDbContext.PolicyDetails.FromSqlRaw(sql).ToListAsync();
            return records;
        }

        public string CreateNewPolicy(PoliciesModel policy)
        {
            var pol = new PoliciesModel { PolicyName = policy.PolicyName, PolicyAmount = policy.PolicyAmount, MaturityMonths = policy.MaturityMonths, MonthlyReturn = policy.MonthlyReturn, MonthlyPoints = policy.MonthlyPoints, Description = policy.Description, Status = "Active", Remarks = policy.Remarks };
            var res = _appDbContext.PolicyDetails.Add(pol);
            _appDbContext.SaveChanges();
            return "Added Successfully";
        }

    }
}
