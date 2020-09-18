using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Middleware.Domain.Interfaces;
using Middleware.Domain.Models;
using Middleware.Utils.Helpers;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Middleware.Application.Controllers
{
    /// <summary>
    /// User Controller
    /// </summary>
    [ApiController]
    [Route("api/users")]
    [Authorize(policy: "Manager")]
    [Produces("application/json")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly UserManager<User> _userManager;
        private readonly IRepository<User> _repository;
        private readonly IRepository<Organization> _orgRepository;
        public UserController(UserManager<User> userManager, IRepository<User> repository, IRepository<Organization> orgRepository, ILogger<UserController> logger)
        {
            _logger = logger;
            _userManager = userManager;
            _repository = repository;
            _orgRepository = orgRepository;
        }

        /// <summary>
        /// Returns all registered users
        /// </summary>
        /// <returns>Json Array</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            User tmpUser = await _userManager.GetUserAsync(User);
            User loggedUser = _repository.Find(tmpUser.Id);
            bool isAdmin = await _userManager.IsInRoleAsync(loggedUser, "Admin");
            return Ok(_repository.GetAll().Where(u => u.Organization == loggedUser.Organization || isAdmin));
        }

        /// <summary>
        /// Returns a specific user
        /// </summary>
        /// <param name="id">User identification string</param>
        /// <returns>Json Object</returns>
        [Route("{id?}")]
        [HttpGet]
        public async Task<IActionResult> FindById(String id)
        {
            if (Guid.TryParse(id, out Guid result))
            {
                User user = _repository.Find(result);
                User tmpUser = await _userManager.GetUserAsync(User);
                User loggedUser = _repository.Find(tmpUser.Id);
                bool isAdmin = await _userManager.IsInRoleAsync(loggedUser, "Admin");
                if ((user != null && user.Organization == loggedUser.Organization) || isAdmin)
                {
                    return Ok(user);
                }
            }
            return Ok(new { });
        }

        /// <summary>
        /// Updates a specific user
        /// </summary>
        /// <param name="id">User identification string</param>
        /// <param name="userModel">User object</param>
        /// <returns>if it is successfully updated it returns a user, otherwise it returns a json error</returns>
        [Route("{id?}")]
        [HttpPut]
        public async Task<IActionResult> Update(String id, [FromBody] User userModel)
        {
            User tmpUser = await _userManager.GetUserAsync(User);
            User loggedUser = _repository.Find(tmpUser.Id);
            User user = await _userManager.FindByIdAsync(id);
            bool isAdmin = await _userManager.IsInRoleAsync(loggedUser, "Admin");
            if ((user != null && user.Organization == loggedUser.Organization) || isAdmin)
            {
                user.Name = userModel.Name;
                user.Email = userModel.Email;
                user.UserName = userModel.Login;
                user.Login = userModel.Login;
               
                Organization org = _orgRepository.Find(userModel.Organization.Id);
                if (org != null && org.Active == true)
                {
                    user.Organization = org;
                }

                IdentityResult result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    _logger.LogInformation(String.Format("O Usuário {0} atualizou o usuário {1}", loggedUser.Login, user.Login));
                    return Ok(user);
                }
            }

            return BadRequest(new { error = "Não foi possível atualizar o usuário" });
        }

        /// <summary>
        /// Deletes a user
        /// </summary>
        /// <param name="id">User identification string</param>
        /// <returns>Status code 200 </returns>
        [Route("{id?}")]
        [HttpDelete]
        public async Task<IActionResult> Delete(String id)
        {
            User tmpUser = await _userManager.GetUserAsync(User);
            User loggedUser = _repository.Find(tmpUser.Id);
            User user = await _userManager.FindByIdAsync(id);
            bool isAdmin = await _userManager.IsInRoleAsync(loggedUser, "Admin");
            if ((user != null && user.Organization == loggedUser.Organization) || isAdmin)
            {
                IdentityResult result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    _logger.LogInformation(String.Format("O Usuário {0} deletou o usuário {1}", loggedUser.Login, user));
                    return Ok();
                }
            }
            return BadRequest(new { error = "Não foi possível excluir role" });
        }

        /// <summary>
        /// Reset the password for a specific user
        /// </summary>
        /// <remarks>
        ///     Sample request: 
        ///     {
        ///         "token" : "user refresh token",
        ///         "newPassword" : "Strign new password"
        ///     }
        /// </remarks>
        /// <param name="id">User identification string</param>
        /// <param name="json">JObject</param>
        /// <returns></returns>
        [Route("reset/{id?}")]
        [HttpPost]
        public async Task<IActionResult> ResetPassword(String id, [FromBody] JObject json)
        {
            String token = json.GetValue("token")?.ToString();
            String newPassword = json.GetValue("newPassword")?.ToString();

            if (String.IsNullOrEmpty(token) || String.IsNullOrEmpty(newPassword))
            {
                return BadRequest(new { error = "Não foi possível processar a solicitação" });
            }

            User loggedUser = await _userManager.GetUserAsync(User);
            User user = await _userManager.FindByIdAsync(id);
            bool isAdmin = await _userManager.IsInRoleAsync(loggedUser, "Admin");
            if ((user != null && user.Organization == loggedUser.Organization) || isAdmin)
            {
                _userManager.PasswordValidators.Clear();
                IdentityResult result = await _userManager.ResetPasswordAsync(user, token, newPassword);
                if (result.Succeeded)
                {
                    _logger.LogInformation("O Usuário {0} trocou a senha do usuário {1}", loggedUser.Login, user.Login);
                    return Ok();
                }
            }

            return BadRequest(new { error = "Não foi possível atualizar a senha, tente novamente" });
        }

        /// <summary>
        /// Generate token password refresh
        /// </summary>
        /// <param name="id">User identification string</param>
        /// <returns>String token</returns>
        [HttpGet]
        [Route("reset/{id?}")]
        public async Task<IActionResult> TokenReset(String id)
        {
            User user = await _userManager.FindByIdAsync(id);
            String token = await _userManager.GeneratePasswordResetTokenAsync(user);

            String _login = User.Claims.FirstOrDefault(c => c.Type == "Login").Value;
            _logger.LogInformation("O Usuário {0} Gerou um token refresh de senha {1} a partir do usuário {2}", _login, token, user.Login);

            return Ok(token);
        }

        [HttpPut]
        [Route("permission/{id?}")]
        public async Task<IActionResult> AddPermissionUser(String id, [FromBody] JObject json)
        {
            String permission = json.GetValue("permission")?.ToString();
            if (String.IsNullOrEmpty(permission))
            {
                return BadRequest(new { error = "Não foi possível processar solicitação" });
            }

            User tmpUser = await _userManager.GetUserAsync(User);
            User loggedUser = _repository.Find(tmpUser.Id);
            User user = await _userManager.FindByIdAsync(id);
            bool isAdmin = await _userManager.IsInRoleAsync(loggedUser, "Admin");
            if ((user != null && user.Organization == loggedUser.Organization) || isAdmin)
            {
                IdentityResult result = await _userManager.AddClaimAsync(user, new Claim("Permission", permission));
                if (result.Succeeded)
                {
                    _logger.LogInformation(String.Format("O usuário {0} alterou a permissão {1} do usuário {2}", loggedUser.Login, permission, user.Login));
                    return Ok();
                }
            }

            return BadRequest(new { error = "Não foi possível procesar solicitação" });
        }

        [HttpDelete]
        [Route("permission/{id?}")]
        public async Task<IActionResult> RemovePermissionUser(String id, [FromBody] JObject json)
        {
            String permission = json.GetValue("permission")?.ToString();
            if (String.IsNullOrEmpty(permission))
            {
                return BadRequest(new { error = "Não foi possível processar solicitação" });
            }

            User tmpUser = await _userManager.GetUserAsync(User);
            User loggedUser = _repository.Find(tmpUser.Id);
            User user = await _userManager.FindByIdAsync(id);
            bool isAdmin = await _userManager.IsInRoleAsync(loggedUser, "Admin");
            if ((user != null && user.Organization == loggedUser.Organization) || isAdmin)
            {
                IdentityResult result = await _userManager.RemoveClaimAsync(user, new Claim("Permission", permission));
                if (result.Succeeded)
                {
                    _logger.LogInformation(String.Format("O usuário {0} deletou a permissões {1} do usuário {2}", loggedUser.Login, permission, user.Login));
                    return Ok();
                }
            }

            return BadRequest(new { error = "Não foi possível procesar solicitação" });
        }
    }
}
