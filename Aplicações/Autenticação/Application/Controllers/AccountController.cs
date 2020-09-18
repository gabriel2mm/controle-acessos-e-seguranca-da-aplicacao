using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Middleware.Domain.Interfaces;
using Middleware.Domain.Models;
using Middleware.Utils.Helpers;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Middleware.Application.Controllers
{
    /// <summary>
    /// Account Controller
    /// </summary>
    [ApiController]
    [Route("api/accounts")]
    [Produces("application/json")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<AccountController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IRepository<User> _repository;
        private readonly IRepository<Organization> _orgRepository;

        public AccountController(ILogger<AccountController> logger, UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration configuration, RoleManager<Role> roleManager, IRepository<User> repository, IRepository<Organization> orgRepository)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _roleManager = roleManager;
            _repository = repository;
            _orgRepository = orgRepository;
        }

        /// <summary>
        /// Application login
        /// </summary>
        /// <param name="login">JObject</param>
        /// <remarks>
        ///     Sample request:
        ///
        ///     POST api/accounts/authenticate
        ///     {
        ///        "login" : "login",
        ///        "password" : "password"
        ///     }
        /// </remarks>
        /// <returns>String token jwt authenticate</returns>
        [Route("authenticate")]
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Authenticate([FromBody] JObject login)
        {
            try
            {
                User user = _repository.Get(u => u.Login == login.GetValue("login")?.ToString()).FirstOrDefault();
                if (user != null && user.Active && user.Organization.Active)
                {
                    var result = await _signInManager.PasswordSignInAsync(user, login.GetValue("password")?.ToString(), false, false);
                    if (result.Succeeded)
                    {
                        String token = TokenHelper.TokenWithRolesClaims(user, _userManager, _roleManager, _configuration).Result;
                        _logger.LogInformation(String.Format("Usuário  {0} efetuou login ás {1}", user.Login, DateTime.Now.ToString("dd/MM/yyyy HH:mm")));
                        return Ok(new { token });
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

        /// <summary>
        /// Return user claims information
        /// </summary>
        /// <returns>Json array with Claims information</returns>
        [HttpGet]
        [Authorize(policy: "Manager")]
        public IActionResult UserClaims()
        {
            Dictionary<String, String> pairsClaims = new Dictionary<string, string>();
            List<Claim> claims = User.Claims.Select(c => c).ToList();
            foreach (Claim c in claims)
            {
                if (!pairsClaims.ContainsKey(c.Type))
                {
                    pairsClaims.Add(c.Type, c.Value);
                }
            }

            return Ok(pairsClaims);
        }

        /// <summary>
        /// Register a new User
        /// </summary>
        /// <param name="user">User Object format</param>
        /// <returns>Return registred user</returns>
        [Route("register")]
        [HttpPost]
        [Authorize(policy: "Manager")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            try
            {
                _userManager.PasswordValidators.Clear();
                User tmpUser = await _userManager.GetUserAsync(User);
                User loggedUser = _repository.Find(tmpUser.Id);
                if (loggedUser != null && loggedUser.Organization != null)
                {
                    user.Organization = loggedUser.Organization;
                }

                user.UserName = user.Login;
                user.Active = true;
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

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> VerifyPermission([FromBody] Module module)
        {
            User tmpUser = await _userManager.GetUserAsync(User);
            User user = _repository.Find(tmpUser.Id);
            if (user != null && user.Organization != null)
            {
                Organization org = _orgRepository.Find(user.Organization.Id);
                Module selectModule = org.Modules.Where(m => m.Name.Equals(module.Name)).FirstOrDefault();
                if (selectModule != null && module.Active)
                {
                    IList<Claim> claims = await _userManager.GetClaimsAsync(user);
                    if (claims != null && claims.Count > 0)
                    {
                        Claim claim = claims.Where(c => "Permission".Equals(c.Type) && selectModule.Permission.Equals(c.Value)).FirstOrDefault();
                        if (claim != null)
                        {
                            return Ok();
                        }
                    }
                }
            }

            return Unauthorized();
        }
    }
}
