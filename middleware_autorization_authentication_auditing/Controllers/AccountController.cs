using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using middleware.Helpers;
using middleware_autorization_authentication_auditing.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace middleware.Controllers
{
    [ApiController]
    [Route("api/accounts")]
    [Produces("Application/json")]
    public class AccountController : ControllerBase
    {
        private readonly ILogger<AccountController> _logger;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly RoleManager<IdentityRole> _roleManager;
        public AccountController(ILogger<AccountController> logger, UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration configuration, RoleManager<IdentityRole> roleManager)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _roleManager = roleManager;
        }

        [Route("authenticate")]
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Authenticate([FromBody] JObject login)
        {
            try
            {
                User user = await _userManager.FindByNameAsync(login.GetValue("login")?.ToString());
                if (user != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(user, login.GetValue("password")?.ToString(), false, false);
                    if (result.Succeeded)
                    {
                        String token = TokenHelper.TokenWithRolesClaims(user, _userManager, _roleManager, _configuration).Result;
                        _logger.LogInformation(String.Format("Usuário  {0} efetuou login ás {1}", user.Login, DateTime.Now.ToString("dd/MM/yyyy HH:mm")));
                        return Ok(new { token = token });
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Não foi possível recuperar usuário");
                return BadRequest(new { error = "Não foi possível processar solicitação" });
            }

            return Unauthorized(new { error = "Usuário e/ou Senha incorretos." });
        }

        [Route("register")]
        [Authorize(Roles = "Admin,Manager")]
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            try
            {
                user.UserName = user.Login;
                _userManager.PasswordValidators.Clear();
                IdentityResult result = await _userManager.CreateAsync(user, user.Password);
                if (result.Succeeded)
                {
                    user.Password = String.Empty;
                    user.PasswordHash = String.Empty;
                    _logger.LogInformation(String.Format("O Usuário {0} foi registrado na data {1}", user.Login, DateTime.Now.ToString("dd/MM/yyyy HH:mm")));
                    return CreatedAtAction(nameof(Register), user);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, String.Format("Não foi possível registrar usuário {0}", user.Login));
            }

            return BadRequest(new { error = "Não foi possível registrar usuário, tente novamente!" });
        }
    }
}
