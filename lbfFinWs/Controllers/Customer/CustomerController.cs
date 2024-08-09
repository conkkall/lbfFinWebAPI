using lbfFinWs.Models;
using lbfFinWs.Repository.Customer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace lbfFinWs.Controllers.Customer
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private IConfiguration _config;
        private readonly ILogger<CustomerController> _logger;
        private readonly ICustomerRepository _customerRepository;

        private readonly UserManager<ApplicationUser> _userManager;
        //private readonly RoleManager<IdentityRole> _roleManager;

        public CustomerController(IConfiguration config, UserManager<ApplicationUser> userManager,
            ICustomerRepository customerRepository)
        {
            _config = config;
            _userManager = userManager;
            //_roleManager = roleManager;
            this._customerRepository = customerRepository;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] CustomerLoginModel model)
        {
            CustomersModel customers = new CustomersModel();
            customers = _customerRepository.ValidateUser(model);
            if (customers != null)
            {
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email, customers.Emailid),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };
                authClaims.Add(new Claim(ClaimTypes.Role, customers.Role));
                var token = CreateToken(authClaims);
                var refreshToken = GenerateRefreshToken();

                _ = int.TryParse(_config["JWT:RefreshTokenValidityInDays"], out int refreshTokenValidityInDays);

                ApplicationUser user = new ApplicationUser();
                user.CustomerId = customers.CustomerId;
                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime = DateTime.Now.AddDays(refreshTokenValidityInDays);
                var res = _customerRepository.CreateRefreshToken(user);
                return Ok(new
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    RefreshToken = refreshToken,
                    Expiration = token.ValidTo
                });
            }
            return Unauthorized();
        }

        private JwtSecurityToken CreateToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Key"]));
            _ = int.TryParse(_config["JWT:TokenValidityInMinutes"], out int tokenValidityInMinutes);

            var token = new JwtSecurityToken(
                issuer: _config["JWT:Issuer"],
                expires: DateTime.Now.AddMinutes(tokenValidityInMinutes),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }

        [HttpPost(Name = "Customer Login")]
        public IActionResult ValidateCustomer([FromBody] CustomerLoginModel login)
        {
            IActionResult response = Unauthorized();

            CustomersModel userCheck = _customerRepository.ValidateUser(login);
            //var userCheck = _customerRepository.ValidateUser(login);
            //var user = AuthenticateUser(login);

            if (userCheck != null)
            {
                var tokenString = GenerateJSONWebToken(login);
                var refreshToken = GenerateRefreshToken();
                response = Ok(new { token = tokenString });
                //response = Ok(new
                //{
                //    Token = new JwtSecurityTokenHandler().WriteToken(tokenString),
                //    RefreshToken = refreshToken,
                //    Expiration = tokenString.ValidTo
                //});
            }

            return response;
        }

        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private string GenerateJSONWebToken(CustomerLoginModel userInfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
              _config["Jwt:Issuer"],
              null,
              expires: DateTime.Now.AddMinutes(60),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpPost]
        [Route("refresh-token")]
        public async Task<IActionResult> RefreshToken(TokenModel tokenModel)
        {
            if (tokenModel is null)
            {
                return BadRequest("Invalid client request");
            }

            string? accessToken = tokenModel.AccessToken;
            string? refreshToken = tokenModel.RefreshToken;

            var principal = GetPrincipalFromExpiredToken(accessToken);

            if (principal == null)
            {
                return BadRequest("Invalid access token or refresh token");
            }

            CustomersModel userCheck = _customerRepository.GetCustomerByCustId(tokenModel);
            AuthTokenModel Tokenuser = new AuthTokenModel();
            Tokenuser = _customerRepository.GetTokenByCustId(userCheck);

            if (userCheck == null || Tokenuser.RefreshToken != refreshToken || Tokenuser.RefreshTokenExpiryTime <= DateTime.Now)
            {
                return BadRequest("Invalid access token or refresh token");
            }

            var newAccessToken = CreateToken(principal.Claims.ToList());
            var newRefreshToken = GenerateRefreshToken();
            _ = int.TryParse(_config["JWT:RefreshTokenValidityInDays"], out int refreshTokenValidityInDays);
            ApplicationUser user = new ApplicationUser();
            user.CustomerId = Tokenuser.CustomerId;
            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(refreshTokenValidityInDays);
            var res = _customerRepository.CreateRefreshToken(user);

            return new ObjectResult(new
            {
                accessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
                refreshToken = newRefreshToken
            });
        }

        private ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Key"])),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;

        }
        
        [Authorize]
        [HttpGet]
        [Route("GetAllCustomers")]
        public async Task<IActionResult> GetAllCustomers()
        {
            try
            {
                return Ok(await _customerRepository.GetAllCustomers());
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                 "Error retrieving data from the database");
            }
        }

        [Authorize]
        [HttpPost]
        [Route("CreateNewCustomer")]
        public string CreateNewCustomer(CustomersModel customer)
        {
            try
            {
                var res = _customerRepository.CreateNewCustomer(customer);
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

        //[Authorize]
        //[HttpPost]
        //[Route("revoke")]
        //public async Task<IActionResult> Revoke(int customerId)
        //{
        //    CustomersModel cust = new CustomersModel();
        //    cust = _customerRepository.GetCustomerByCustId(customerId);
        //    if (cust == null) return BadRequest("Invalid user name");

        //    ApplicationUser user = new ApplicationUser();
        //    user.CustomerId = customerId;
        //    user.RefreshToken = null;
        //    var res = _customerRepository.CreateRefreshToken(user);

        //    return NoContent();
        //}

    }
}
