using lbfFinWs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics.Eventing.Reader;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Cryptography;
using Newtonsoft.Json.Linq;
using lbfFinWs.Repository.Policy;

namespace lbfFinWs.Controllers.Policy
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PolicyController : ControllerBase
    {
        private IConfiguration _config;
        private readonly ILogger<PolicyController> _logger;
        private readonly IPolicyRepository _policyRepository;
        public PolicyController(IConfiguration config, IPolicyRepository policyRepository)
        {
            _config = config;
            this._policyRepository = policyRepository;
        }

        [Authorize]
        [HttpGet]
        [Route("GetAllPolicies")]
        public async Task<IActionResult> GetAllPolicies()
        {
            try
            {
                return Ok(await _policyRepository.GetAllPolicies());
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                 "Error retrieving data from the database");
            }
        }

        [Authorize]
        [HttpPost]
        [Route("CreateNewPolicy")]
        public string CreateNewPolicy(PoliciesModel policy)
        {
            try
            {
                var res = _policyRepository.CreateNewPolicy(policy);
                var message = "";
                if (res != null)
                {
                    message = res;
                }
                return message;

            }
            catch (Exception)
            {
                return "Error saving data to the database";
                //return StatusCode(StatusCodes.Status500InternalServerError, "Error saving data to the database");
            }
        }
    }
}
