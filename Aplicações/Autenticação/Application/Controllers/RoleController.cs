using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using Middleware.Domain.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Middleware.Utils.Helpers;
using Middleware.Domain.Interfaces;

namespace Middleware.Application.Controllers
{
    /// <summary>
    /// Role Controller
    /// </summary>
    [ApiController]
    [Route("api/roles")]
    [Produces("application/json")]
    [Authorize(policy: "Manager")]
    public class RoleController : Controller
    {
        private readonly RoleManager<Role> _roleManager;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<RoleController> _logger;
        private readonly IRepository<Role> _repository;
        private readonly IRepository<User> _userRepository;
        public RoleController(ILogger<RoleController> logger, UserManager<User> userManager, RoleManager<Role> roleManager, IRepository<Role> repository, IRepository<User> userRepository)
        {
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
            _repository = repository;
            _userRepository = userRepository;
        }
        /// <summary>
        /// Returns all roles
        /// </summary>
        /// <returns>Json Array</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            Guid.TryParse(User.Claims.Where(c => c.Type.Equals(ClaimTypes.NameIdentifier)).FirstOrDefault().Value, out Guid UserID);
            User loggedUser = _userRepository.Find(UserID);
            bool isAdmin = await _userManager.IsInRoleAsync(loggedUser, "Admin");
            return Ok(_repository.GetAll().ToList().Where(r => r.Organization == loggedUser.Organization || isAdmin));
        }

        /// <summary>
        /// Returns a specific role
        /// </summary>
        /// <param name="id">Role identification string</param>
        /// <returns>Json Object</returns>
        [Route("{id?}")]
        [HttpGet]
        public async Task<IActionResult> FindById(String id)
        {
            Guid.TryParse(User.Claims.Where(c => c.Type.Equals(ClaimTypes.NameIdentifier)).FirstOrDefault().Value, out Guid UserID);
            User loggedUser = _userRepository.Find(UserID);
            if (Guid.TryParse(id, out Guid result))
            {
                Role role = _repository.Find(result);
                bool isAdmin = await _userManager.IsInRoleAsync(loggedUser, "Admin");
                if ((role != null && role.Organization == loggedUser.Organization) || isAdmin)
                {
                    return Ok(role);
                }
            }
            return Ok(new { });
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
        public async Task<IActionResult> Update(String id, [FromBody] JObject json)
        {
            String nameRole = json.GetValue("name")?.ToString();
            if (String.IsNullOrEmpty(nameRole) || "admin".Equals(nameRole.ToLower()))
            {
                return BadRequest(new { error = "Não foi possível processar solicitação" });
            }
            Guid.TryParse(User.Claims.Where(c => c.Type.Equals(ClaimTypes.NameIdentifier)).FirstOrDefault().Value, out Guid UserID);
            User loggedUser = _userRepository.Find(UserID);
            Role role = await _roleManager.FindByIdAsync(id);
            bool isAdmin = await _userManager.IsInRoleAsync(loggedUser, "Admin");
            if ((role != null && role.Organization == loggedUser.Organization) || isAdmin)
            {
                role.Name = Strings.StrConv(nameRole, VbStrConv.ProperCase);
                IdentityResult result = await _roleManager.UpdateAsync(role);
                if (result.Succeeded)
                {
                    _logger.LogInformation(String.Format("O Usuário {0} Atualizou a role {1}", loggedUser.Login, id));
                    return Ok(role);
                }
            }

            return BadRequest(new { error = "Não foi possível atualizar a role" });
        }

        /// <summary>
        /// Register a new role
        /// </summary>
        /// <param name="role">Identity Role Object format</param>
        /// <returns>Json Object</returns>
        [HttpPost]
        public async Task<IActionResult> CreateRole([FromBody] Role role)
        {

            if (String.IsNullOrEmpty(role.Name) || "admin".Equals(role.Name.ToLower()) || "manager".Equals(role.Name.ToLower()))
            {
                return BadRequest(new { error = "Não foi possível processar solicitação" });
            }

            Guid.TryParse(User.Claims.Where(c => c.Type.Equals(ClaimTypes.NameIdentifier)).FirstOrDefault().Value, out Guid UserID);
            User user = _userRepository.Find(UserID);
            String _login = User.Claims.FirstOrDefault(c => c.Type == "Login").Value;
            Role createRole = new Role()
            {
                Name = role.Name,
                Organization = user.Organization
            };

            IdentityResult result = await _roleManager.CreateAsync(createRole);
            if (result.Succeeded)
            {
                _logger.LogInformation(String.Format("O Usuário {0} Criou a role {1}", _login, role.Name));
                return CreatedAtAction(nameof(CreateRole), role);
            }

            return BadRequest(new { error = "Não foi possível criar Role" });
        }

        /// <summary>
        /// Deletes a Role
        /// </summary>
        /// <param name="id">Role identification string</param>
        /// <returns>Status code 200 </returns>
        [Route("{id?}")]
        [HttpDelete]
        public async Task<IActionResult> Delete(String id)
        {
            Guid.TryParse(User.Claims.Where(c => c.Type.Equals(ClaimTypes.NameIdentifier)).FirstOrDefault().Value, out Guid UserID);
            User loggedUser = _userRepository.Find(UserID);
            Role role = await _roleManager.FindByIdAsync(id);
            bool isAdmin = await _userManager.IsInRoleAsync(loggedUser, "Admin");
            if ((role != null && role.Organization == loggedUser.Organization) || isAdmin)
            {
                IdentityResult result = await _roleManager.DeleteAsync(role);
                if (result.Succeeded)
                {
                    _logger.LogInformation(String.Format("O Usuário {0} Deletou a role {1}", loggedUser.Login, role.Name));
                    return Ok();
                }
            }

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
        [HttpPut]
        public async Task<IActionResult> UpdatePermission(String id, [FromBody] JObject json)
        {
            String permission = json.GetValue("permission")?.ToString();
            if (String.IsNullOrEmpty(permission))
            {
                return BadRequest(new { error = "Não foi possível processar solicitação" });
            }

            Guid.TryParse(User.Claims.Where(c => c.Type.Equals(ClaimTypes.NameIdentifier)).FirstOrDefault().Value, out Guid UserID);
            User loggedUser = _userRepository.Find(UserID);
            Role role = await _roleManager.FindByIdAsync(id);
            bool isAdmin = await _userManager.IsInRoleAsync(loggedUser, "Admin");
            if ((role != null && role.Organization == loggedUser.Organization) || isAdmin)
            {
                IdentityResult result = await _roleManager.AddClaimAsync(role, new Claim(permission, permission));
                if (result.Succeeded)
                {
                    _logger.LogInformation(String.Format("O Usuário {0} Alterou a permissão da role {1}", loggedUser.Login, role.Name));
                    return Ok();
                }
            }

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
        public async Task<IActionResult> RemovePermission(String id, [FromBody] JObject json)
        {
            String permission = json.GetValue("permission")?.ToString();
            if (String.IsNullOrEmpty(permission))
            {
                return BadRequest(new { error = "Não foi possível processar solicitação" });
            }

            Guid.TryParse(User.Claims.Where(c => c.Type.Equals(ClaimTypes.NameIdentifier)).FirstOrDefault().Value, out Guid UserID);
            User loggedUser = _userRepository.Find(UserID);
            Role role = await _roleManager.FindByIdAsync(id);
            bool isAdmin = await _userManager.IsInRoleAsync(loggedUser, "Admin");
            if ((role != null && role.Organization == loggedUser.Organization) || isAdmin)
            {
                IList<Claim> claims = await _roleManager.GetClaimsAsync(role);
                Claim claim = claims.Where(c => permission.Equals(c.Type)).FirstOrDefault();
                IdentityResult result = await _roleManager.RemoveClaimAsync(role, claim);
                if (result.Succeeded)
                {
                    _logger.LogInformation(String.Format("O Usuário {0} removeu a permissão {1} da role {0}", loggedUser.Login, permission, role.Name));
                    return Ok();
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
        [Route("{id}/user/{userId}")]
        [HttpPut]
        public async Task<IActionResult> AddRoleInUser(String id, String userId)
        {
            Guid.TryParse(User.Claims.Where(c => c.Type.Equals(ClaimTypes.NameIdentifier)).FirstOrDefault().Value, out Guid UserID);
            User loggedUser = _userRepository.Find(UserID);
            Role role = await _roleManager.FindByIdAsync(id);
            Guid.TryParse(userId, out Guid userGuidId);
            User user = _userRepository.Find(userGuidId);
            bool isAdmin = await _userManager.IsInRoleAsync(loggedUser, "Admin");
            if ((role != null && user != null && user.Organization == loggedUser.Organization && role.Organization == loggedUser.Organization) || isAdmin)
            {
                IdentityResult result = await _userManager.AddToRoleAsync(user, role.Name);
                if (result.Succeeded)
                {
                    _logger.LogInformation(String.Format("O Usuário {0} Adicionou a role {1} ao usuário {1}", loggedUser.Login, role.Name, user.Login));
                    return Ok();
                }
            }

            return BadRequest(new { error = "Não foi possível adicionar o usuário a role" });
        }

        /// <summary>
        /// Remove profile on the user
        /// </summary>
        /// <param name="id">Role identification string</param>
        /// <param name="userId">User identification string</param>
        /// <returns>Status code 200</returns>
        [Route("{id}/user/{userId}")]
        [HttpDelete]
        public async Task<IActionResult> RemoveRole(String id, String userId)
        {
            Guid.TryParse(User.Claims.Where(c => c.Type.Equals(ClaimTypes.NameIdentifier)).FirstOrDefault().Value, out Guid UserID);
            User loggedUser = _userRepository.Find(UserID);
            Role role = await _roleManager.FindByIdAsync(id);
            Guid.TryParse(userId, out Guid userGuidId);
            User user = _userRepository.Find(userGuidId);
            bool isAdmin = await _userManager.IsInRoleAsync(loggedUser, "Admin");
            if ((role != null && user != null && user.Organization == loggedUser.Organization && role.Organization == loggedUser.Organization) || isAdmin)
            {
                IdentityResult result = await _userManager.RemoveFromRoleAsync(user, role.Name);
                if (result.Succeeded)
                {
                    _logger.LogInformation(String.Format("O Usuário {0} Removeu a role {1} do usuário {1}", loggedUser.Login, role.Name, user.Login));
                    return Ok();
                }
            }

            return BadRequest(new { error = "Não foi possível remover a role do usuário" });
        }
    }
}

