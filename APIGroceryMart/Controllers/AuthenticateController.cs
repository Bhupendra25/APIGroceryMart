using GroceryStore.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
namespace GroceryStore.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        // private readonly IEmailService _emailService;

        public AuthenticateController(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            //IEmailService emailService,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            // _emailService = emailService;
        }

        //[Authorize]
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {

                var userRoles = await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                     new Claim(ClaimTypes.NameIdentifier, user.Id)
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var token = GetToken(authClaims);

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo,
                    uRole = userRoles
                });
            }
            return Unauthorized();
        }

        [HttpPost]
        [Route("User_register")]
        public async Task<IActionResult> Register([FromBody] RegiserModel model)
        {
            var userExists = await _userManager.FindByNameAsync(model.Username);
            var userExists1 = await _userManager.FindByNameAsync(model.Email);

            if (userExists != null || userExists1 != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });

            IdentityUser user = new()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username,
                TwoFactorEnabled = true
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });


            if (!await _roleManager.RoleExistsAsync(UserRoles.User))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));


            if (await _roleManager.RoleExistsAsync(UserRoles.User))
            {
                await _userManager.AddToRoleAsync(user, UserRoles.User);
            }

            return Ok(new Response { Status = "Success", Message = "User created successfully!" });
        }
        /* [HttpGet("ConfirmEmail")]
         public async Task<IActionResult> ConfirmEmail(string token, string email)
         {
             var user = await _userManager.FindByEmailAsync(email);
             if (user != null)
             {
                 var result = await _userManager.ConfirmEmailAsync(user, token);
                 if (result.Succeeded)
                 {
                     return StatusCode(StatusCodes.Status200OK,
                       new Response { Status = "Success", Message = "Email Verified Successfully" });
                 }
             }
             return StatusCode(StatusCodes.Status500InternalServerError,
                        new Response { Status = "Error", Message = "This User Doesnot exist!" });
         }*/
        // [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("register_Admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegiserModel model)
        {
            var userExists = await _userManager.FindByNameAsync(model.Username);
            var userExists1 = await _userManager.FindByNameAsync(model.Email);
            if (userExists != null || userExists1 != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });

            IdentityUser user = new()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username,
                TwoFactorEnabled = true
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });

            if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));

            if (await _roleManager.RoleExistsAsync(UserRoles.Admin))
            {
                await _userManager.AddToRoleAsync(user, UserRoles.Admin);
            }


            await _userManager.AddToRoleAsync(user, "Admin");

            return StatusCode(StatusCodes.Status200OK,
                new Response { Status = "Success", Message = $"Admin  {user.Email} created SuccessFully" });

        }




        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }
    }


}