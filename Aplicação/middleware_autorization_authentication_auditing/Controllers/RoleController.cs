using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using middleware.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace middleware.Controllers
{
    /// <summary>
    /// Role Controller
    /// </summary>
    [ApiController]
    [Route("api/roles")]
    [Produces("application/json")]
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<RoleController> _logger;
        private readonly UserManager<User> _userManager;
        public RoleController(ILogger<RoleController> logger, RoleManager<IdentityRole> roleManager, UserManager<User> userManager)
        {
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        /// <summary>
        /// Returns all roles
        /// </summary>
        /// <returns>Json Array</returns>
        [HttpGet]
        [Authorize(policy: Helpers.Permission.Roles.Viwer)]
        public IActionResult GetAll()
        {
            String _login = User.Claims.FirstOrDefault(c => c.Type == "Login").Value;
            _logger.LogInformation(String.Format("O Usuário {0} visualizou a lista de roles", _login));
            return Ok(_roleManager.Roles.ToList());
        }

        /// <summary>
        /// Returns a specific role
        /// </summary>
        /// <param name="id">Role identification string</param>
        /// <returns>Json Object</returns>
        [Route("{id?}")]
        [HttpGet]
        [Authorize(policy: Helpers.Permission.Roles.Viwer)]
        public async Task<IActionResult> FindById(String id)
        {
            IdentityRole role = await _roleManager.FindByIdAsync(id);
            String _login = User.Claims.FirstOrDefault(c => c.Type == "Login").Value;
            _logger.LogInformation(String.Format("O Usuário {0} visualizou a Role {1}", _login, id));
            return Ok(role);
        }
        /// <summary>
        /// Updates a specific Role
        /// </summary>
        /// <remarks>
        ///     sample request: 
        ///     {
        ///         "name" : "role name"
        ///     }
        /// </remarks>
        /// <param name="id">Role identification string</param>
        /// <param name="json">JObject</param>
        /// <returns>Json Object</returns>
        [Route("{id?}")]
        [HttpPut]
        [Authorize(policy: Helpers.Permission.Roles.Manager)]
        public async Task<IActionResult> Update(String id, [FromBody] JObject json)
        {
            String _login = User.Claims.FirstOrDefault(c => c.Type == "Login").Value;
            IdentityRole role = await _roleManager.FindByIdAsync(id);
            role.Name = Strings.StrConv(json.GetValue("name")?.ToString(), VbStrConv.ProperCase);

            IdentityResult result = await _roleManager.UpdateAsync(role);

            if (result.Succeeded)
            {
                _logger.LogInformation(String.Format("O Usuário {0} Atualizou a role {1}", _login, id));
                return Ok(role);
            }
            _logger.LogError(String.Format("Erro ao atualizar role: O usuário {0} tentou atualizar a role {1}", _login, id));
            return BadRequest(new { error = "Não foi possível atualizar a role" });
        }

        /// <summary>
        /// Register a new role
        /// </summary>
        /// <param name="role">Identity Role Object format</param>
        /// <returns>Json Object</returns>
        [HttpPost]
        [Authorize(policy: Helpers.Permission.Roles.Manager)]
        public async Task<IActionResult> CreateRole([FromBody] IdentityRole role)
        {
            String _login = User.Claims.FirstOrDefault(c => c.Type == "Login").Value;
            IdentityRole createRole = new IdentityRole();
            createRole.Name = role.Name;

            IdentityResult result = await _roleManager.CreateAsync(createRole);

            if (result.Succeeded)
            {
                _logger.LogInformation(String.Format("O Usuário {0} Criou a role {1}", _login, role.Name));
                return CreatedAtAction(nameof(CreateRole), role);
            }
            _logger.LogError(String.Format("Erro ao criar role: O usuário {0} tentou criar uma nova role }", _login));
            return BadRequest(new { error = "Não foi possível criar Role" });
        }

        /// <summary>
        /// Deletes a Role
        /// </summary>
        /// <param name="id">Role identification string</param>
        /// <returns>Status code 200 </returns>
        [Route("{id?}")]
        [HttpDelete]
        [Authorize(policy: Helpers.Permission.Roles.Manager)]
        public async Task<IActionResult> Delete(String id)
        {
            String _login = User.Claims.FirstOrDefault(c => c.Type == "Login").Value;
            IdentityRole role = await _roleManager.FindByIdAsync(id);
            IdentityResult result = await _roleManager.DeleteAsync(role);
            if (result.Succeeded)
            {
                _logger.LogInformation(String.Format("O Usuário {0} Deletou a role {1}", _login, role.Name));
                return Ok();
            }

            _logger.LogError(String.Format("Erro ao deletar role: O usuário {0} tentou deletar a role {0} }", _login, id));
            return BadRequest(new { error = "Não foi possível excluir role" });
        }

        /// <summary>
        /// Insert a permission into the role
        /// </summary>
        /// <remarks>
        ///     sample request:
        ///     {
        ///         permission : "permission name"
        ///     }
        /// </remarks>
        /// <param name="id">Role identification string</param>
        /// <param name="json">Object</param>
        /// <returns>StatusCode 200</returns>
        [Route("permission/{id?}")]
        [HttpPost]
        [Authorize(policy: Helpers.Permission.Roles.Manager)]
        public async Task<IActionResult> Permission(String id, [FromBody] JObject json)
        {
            String _login = User.Claims.FirstOrDefault(c => c.Type == "Login").Value;
            IdentityRole role = await _roleManager.FindByIdAsync(id);
            if (role != null)
            {
                String permission = json.GetValue("permission")?.ToString();
                if (String.IsNullOrEmpty(permission))
                {
                    _logger.LogError(String.Format("Erro ao adicionar permissão: O usuário {0} tentou adicionar permissão a role {0} }", _login, id));
                    return BadRequest(new { error = "Não foi possível processar solicitação" });
                }

                IdentityResult result = await _roleManager.AddClaimAsync(role, new Claim(permission, permission));
                if (result.Succeeded)
                {
                    _logger.LogInformation(String.Format("O Usuário {0} Alterou a permissão da role {1}", _login, role.Name));
                    return Ok();
                }
            }
            _logger.LogError(String.Format("Erro ao adicionar permissão: O usuário {0} tentou adicionar permissão a role {0} }", _login, id));
            return BadRequest(new { error = "Não foi possível adicionar permissão ao perfil" });
        }


        /// <summary>
        /// Remove a permission into the role
        /// </summary>
        /// <remarks>
        ///     sample request:
        ///     {
        ///         permission : "permission name"
        ///     }
        /// </remarks>
        /// <param name="id">Role identification string</param>
        /// <param name="json">Object</param>
        /// <returns>StatusCode 200</returns>
        [Route("permission/{id?}")]
        [HttpDelete]
        [Authorize(policy: Helpers.Permission.Roles.Manager)]
        public async Task<IActionResult> RemovePermission(String id, [FromBody] JObject json)
        {
            String _login = User.Claims.FirstOrDefault(c => c.Type == "Login").Value;
            String permission = json.GetValue("permission")?.ToString();
            if (String.IsNullOrEmpty(permission))
            {
                _logger.LogError(String.Format("Erro ao remover permissão: O Usuário {0} tentou remover a permissão {1} sem parametro", _login, permission));
                return BadRequest(new { error = "Não foi possível processar solicitação" });
            }

            IdentityRole role = await _roleManager.FindByIdAsync(id);
            if (role != null)
            {
                IList<Claim> claims = await _roleManager.GetClaimsAsync(role);
                Claim claim = claims.Where(c => permission.Equals(c.Type)).FirstOrDefault();
                IdentityResult result = await _roleManager.RemoveClaimAsync(role, claim);

                if (result.Succeeded)
                {
                    _logger.LogInformation(String.Format("O Usuário {0} removeu a permissão {1} da role {0}", _login, permission, role.Name));
                    return Ok();
                }
                else
                {
                    _logger.LogError(String.Format("Erro ao remover permissão: O Usuário {0} tentou remover a permissão {1} da role {0}", _login, permission, role.Name));
                    return BadRequest(new { error = "Não foi possível remover permissão ao perfil" });
                }
            }
            return BadRequest(new { error = "Não foi possível remover permissão ao perfil" });
        }

        /// <summary>
        /// Insert a profile on the user
        /// </summary>
        /// <param name="id">Role identification string</param>
        /// <param name="userId">User identification string</param>
        /// <returns>Status Code 200</returns>
        [Route("{id}/users/{userId}")]
        [HttpPost]
        [Authorize(policy: Helpers.Permission.Roles.Manager)]
        public async Task<IActionResult> AddRoleInUser(String id, String userId)
        {
            String _login = User.Claims.FirstOrDefault(c => c.Type == "Login").Value;
            IdentityRole role = await _roleManager.FindByIdAsync(id);
            User user = await _userManager.FindByIdAsync(userId);

            IdentityResult result = await _userManager.AddToRoleAsync(user, role.Name);

            if (result.Succeeded)
            {
                _logger.LogInformation(String.Format("O Usuário {0} Adicionou a role {1} ao usuário {1}", _login, role.Name, user.Login));
                return Ok();
            }

            _logger.LogError(String.Format("Erro ao adicionar usuário a role: O Usuário {0} tentou adicionar o usuário {1} na role {2}", _login, user.Login, role.Name));
            return BadRequest(new { error = "Não foi possível adicionar o usuário a role" });
        }

        /// <summary>
        /// Remove profile on the user
        /// </summary>
        /// <param name="id">Role identification string</param>
        /// <param name="userId">User identification string</param>
        /// <returns>Status code 200</returns>
        [Route("{id}/users/{userId}")]
        [HttpDelete]
        [Authorize(policy: Helpers.Permission.Roles.Manager)]
        public async Task<IActionResult> RemoveRole(String id, String userId)
        {
            String _login = User.Claims.FirstOrDefault(c => c.Type == "Login").Value;
            IdentityRole role = await _roleManager.FindByIdAsync(id);
            User user = await _userManager.FindByIdAsync(userId);

            IdentityResult result = await _userManager.RemoveFromRoleAsync(user, role.Name);

            if (result.Succeeded)
            {
                _logger.LogInformation(String.Format("O Usuário {0} Removeu a role {1} do usuário {1}", _login, role.Name, user.Login));
                return Ok();
            }

            _logger.LogError(String.Format("Erro ao remover usuário da role: O Usuário {0} tentou remover o usuário {1} na role {2}", _login, user.Login, role.Name));
            return BadRequest(new { error = "Não foi possível remover a role do usuário" });
        }
    }
}

