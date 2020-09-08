using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using middleware.Helpers;
using middleware.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace middleware.Controllers
{
    /// <summary>
    /// User Controller
    /// </summary>
    [ApiController]
    [Route("api/users")]
    [Produces("application/json")]
    public class UserController : ControllerBase
    {
        private ILogger<UserController> _logger;
        private UserManager<User> _userManager;
        public UserController(UserManager<User> userManager, ILogger<UserController> logger)
        {
            _logger = logger;
            _userManager = userManager;
        }

        /// <summary>
        /// Returns all registered users
        /// </summary>
        /// <returns>Json Array</returns>
        [HttpGet]
        [Authorize(policy: Permission.Users.Viwer)]
        public IActionResult GetAll()
        {
            String _login = User.Claims.FirstOrDefault(c => c.Type == "Login").Value;
            _logger.LogInformation(String.Format("O Usuário {0} visualizou a lista de usuários", _login));

            return Ok(_userManager.Users.ToList());
        }

        /// <summary>
        /// Returns a specific user
        /// </summary>
        /// <param name="id">User identification string</param>
        /// <returns>Json Object</returns>
        [Route("{id?}")]
        [HttpGet]
        [Authorize(policy: Permission.Users.Viwer)]
        public async Task<IActionResult> FindById(String id)
        {
            User user = await _userManager.FindByIdAsync(id);
            String _login = User.Claims.FirstOrDefault(c => c.Type == "Login").Value;
            _logger.LogInformation(String.Format("O Usuário {0} visualizou o Usuário {1}", _login, user.Login));
            return Ok(user);
        }

        /// <summary>
        /// Updates a specific user
        /// </summary>
        /// <param name="id">User identification string</param>
        /// <param name="userModel">User object</param>
        /// <returns>if it is successfully updated it returns a user, otherwise it returns a json error</returns>
        [Route("{id?}")]
        [HttpPut]
        [Authorize(policy: Permission.Users.Manager)]
        public async Task<IActionResult> Update(String id, [FromBody] User userModel)
        {
            String _login = User.Claims.FirstOrDefault(c => c.Type == "Login").Value;
            User user = await _userManager.FindByIdAsync(id);
            user.Name = userModel.Name;
            user.Email = userModel.Email;
            user.UserName = userModel.Login;
            user.Login = userModel.Login;

            IdentityResult result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                _logger.LogInformation(String.Format("O Usuário {0} atualizou o usuário {1}", _login, user.Login));
                return Ok(user);
            }
            _logger.LogError(String.Format("Erro ao atualizar role: O usuário {0} tentou atualizar o usuário {1}", _login, id));
            return BadRequest(new { error = "Não foi possível atualizar a role" });
        }

        /// <summary>
        /// Deletes a user
        /// </summary>
        /// <param name="id">User identification string</param>
        /// <returns>Status code 200 </returns>
        [Route("{id?}")]
        [HttpDelete]
        [Authorize(policy: Permission.Users.Manager)]
        public async Task<IActionResult> Delete(String id)
        {
            User user = await _userManager.FindByIdAsync(id);
            IdentityResult result = await _userManager.DeleteAsync(user);
            String _login = User.Claims.FirstOrDefault(c => c.Type == "Login").Value;
            if (result.Succeeded)
            {
                _logger.LogInformation(String.Format("O Usuário {0} deletou o usuário {1}", _login, user));
                return Ok();
            }

            _logger.LogError(String.Format("Erro ao criar role: O usuário {0} tentou deletar o usuário {0} }", _login, id));
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
        [Authorize(policy: Permission.Users.Manager)]
        public async Task<IActionResult> ResetPassword(String id, [FromBody] JObject json)
        {
            String _login = User.Claims.FirstOrDefault(c => c.Type == "Login").Value;
            String token = json.GetValue("token")?.ToString();
            String newPassword = json.GetValue("newPassword")?.ToString();

            if (String.IsNullOrEmpty(token) || String.IsNullOrEmpty(newPassword))
            {
                _logger.LogError(String.Format("Erro: O usuário {0}, tentou alterar a senha do usuário {1}", _login, id));
                return BadRequest(new { error = "Não foi possível processar a solicitação" });
            }

            _userManager.PasswordValidators.Clear();
            User user = await _userManager.FindByIdAsync(id);
            IdentityResult result = await _userManager.ResetPasswordAsync(user, token, newPassword);
            if (result.Succeeded)
            {
                _logger.LogInformation("O Usuário {0} trocou a senha do usuário {1}", _login, user.Login);
                return Ok();
            }

            _logger.LogError(String.Format("Erro ao alterar senha do usuário {0}", user.Login));
            return BadRequest(new { error = "Não foi possível atualizar a senha, tente novamente" });

        }

        /// <summary>
        /// Generate token password refresh
        /// </summary>
        /// <param name="id">User identification string</param>
        /// <returns>String token</returns>
        [HttpGet]
        [Route("reset/{id?}")]
        [Authorize(policy: Permission.Users.Manager)]
        public async Task<IActionResult> TokenReset(String id)
        {
            User user = await _userManager.FindByIdAsync(id);
            String token = await _userManager.GeneratePasswordResetTokenAsync(user);

            String _login = User.Claims.FirstOrDefault(c => c.Type == "Login").Value;
            _logger.LogInformation("O Usuário {0} Gerou um token refresh de senha {1} a partir do usuário {2}", _login, token, user.Login);

            return Ok(token);
        }
    }
}
