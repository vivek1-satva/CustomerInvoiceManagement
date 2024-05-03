using DataAccess.Models;
using CustomerInvoiceManagement.CommonModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DataAccessLayer.Models.ViewModels;

namespace CustomerInvoiceManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private UserManager<ApplicationUser> _userManager;
        private RoleManager<ApplicationRole> _roleManager;
        private readonly IConfiguration _configuration;


        public AccountController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        [HttpPost]
        [Route("CreateRoles")]
        public async Task<IActionResult> CreateRoles()
        {
            await _roleManager.CreateAsync(new ApplicationRole { Name = "Admin" });
            await _roleManager.CreateAsync(new ApplicationRole { Name = "Employee" });
            return Ok();
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register(RegisterViewModel request = null)
        {
            var result = await RegisterAsync(request);
            return Ok(result);

        }

        #region private Method
        private async Task<CommonResponseFormat> RegisterAsync(RegisterViewModel request)
        {

            if (!ModelState.IsValid)
            {
                return ValidateModel();
            }
            var objUserInfromation = await _userManager.FindByEmailAsync(request.Email);

            if (objUserInfromation != null) return new CommonResponseFormat { Message = "User already exists", ResponseStatus = 0 };

            //if we get here, no user with this email..

            objUserInfromation = new ApplicationUser
            {
                FullName = request.Name,
                Email = request.Email,
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                UserName = request.Email,
                PhoneNumber = request.Number
            };



            var createUserResult = await _userManager.CreateAsync(objUserInfromation, request.Password);
            if (!createUserResult.Succeeded) return new CommonResponseFormat { Message = $"Create user failed {createUserResult?.Errors?.First()?.Description}", ResponseStatus = 0 };

            //var token = await _userManager.GenerateEmailConfirmationTokenAsync(objUserInfromation);
            //var confirmationLink = Url.Action("ConfirmEmail", "Account",
            //new { userId = objUserInfromation.Id, token = token }, Request.Scheme);
            //user is created...
            //then add user to a role...
            var addUserToRoleResult = await _userManager.AddToRoleAsync(objUserInfromation, "Admin");
            if (!addUserToRoleResult.Succeeded) return new CommonResponseFormat { Message = $"Create user succeeded but could not add user to role {addUserToRoleResult?.Errors?.First()?.Description}", ResponseStatus = 2 };

            //all is still well..
            return new CommonResponseFormat
            {
                ResponseStatus = 1,
                Message = "User registered successfully",
                Result = null
            };

        }

        [HttpGet("ConfirmEmail")]
        public async Task<ActionResult> ConfirmEmail(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return Ok(new CommonResponseFormat { Message = "User not found", ResponseStatus = 0 });
            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded) return Ok(new CommonResponseFormat { Message = "Email not confirmed", ResponseStatus = 0 });
            user.EmailConfirmed = true;
            return Ok(new CommonResponseFormat { Message = "Email confirmed", ResponseStatus = 1 });
        }


        [HttpPost]
        [Route("Login")]
        [AllowAnonymous]

        public async Task<IActionResult> Login([FromBody] CustomLoginRequestViewModel request)
        {

            string authenticationKey = _configuration["AuthenticationKey"];
            var result = await LoginAsync(request, authenticationKey);

            return Ok(result);

        }
        private async Task<CommonResponseFormat> LoginAsync(CustomLoginRequestViewModel request, string authenticationKey)
        {
            if (!ModelState.IsValid)
            {
                return ValidateModel();
            }
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user is null) return new CommonResponseFormat { Message = "Incorrect credentials", ResponseStatus = 0 };
            string passwordFromBody = request.Password;
            if (!await _userManager.CheckPasswordAsync(user, request.Password)) return new CommonResponseFormat { Message = "Incorrect credentials", ResponseStatus = 0 };
            //all is well if we reach here
            //bool IsUserEmailVerified = await _userManager.IsEmailConfirmedAsync(user);
            //if (!IsUserEmailVerified) return new CommonResponseFormat { Message = "Email not verified", ResponseStatus = 0 };

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = roles.Select(x => new Claim(ClaimTypes.Role, x));
            claims.AddRange(roleClaims);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddMinutes(30);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
                );

            return new CommonResponseFormat
            {

                Message = "Login Successful",
                Result = new
                {
                    AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                    Email = user?.Email,
                    UserId = user?.Id.ToString()
                },
                ResponseStatus = 1,
            };
        }

        //error 
        private CommonResponseFormat ValidateModel()
        {
            var errors = ModelState
                            .Where(x => x.Value.Errors.Any())
                            .Select(x => new
                            {
                                Field = x.Key,
                                Error = x.Value.Errors.First().ErrorMessage
                            })
                            .ToList();

            return new CommonResponseFormat
            {
                ResponseStatus = 0,
                Message = "Validation failed",
                Result = errors
            };
        }
        #endregion 
    }
}