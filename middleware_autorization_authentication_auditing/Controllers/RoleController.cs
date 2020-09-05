using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using Newtonsoft.Json.Linq;

namespace middleware.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/roles")]
    [Produces("Application/json")]
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<RoleController> _logger;
        public RoleController(ILogger<RoleController> logger, RoleManager<IdentityRole> roleManager)
        {
            _logger = logger;
            _roleManager = roleManager;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            String _login = User.Claims.FirstOrDefault(c => c.Type == "Login").Value;
            _logger.LogInformation(String.Format("O Usuário {0} visualizou a lista de roles", _login));
            return Ok(_roleManager.Roles.ToList());
        }

        [Route("{id?}")]
        [HttpGet]
        public async Task<IActionResult> FindById(String id)
        {
            IdentityRole role = await _roleManager.FindByIdAsync(id);
            String _login = User.Claims.FirstOrDefault(c => c.Type == "Login").Value;
            _logger.LogInformation(String.Format("O Usuário {0} visualizou a Role {1}", _login, id));
            return Ok(role);
        }

        [Route("{id?}")]
        [HttpPut]
        public async Task<IActionResult> Update(String id, [FromBody] JObject json)
        {
            IdentityRole role = await _roleManager.FindByIdAsync(id);
            role.Name = Strings.StrConv(json.GetValue("name")?.ToString(), VbStrConv.ProperCase);

            IdentityResult result = await _roleManager.UpdateAsync(role);
            String _login = User.Claims.FirstOrDefault(c => c.Type == "Login").Value;

            if (result.Succeeded)
            {
                _logger.LogInformation(String.Format("O Usuário {0} Atualizou a role {1}", _login, id));
                return Ok(role);
            }
            _logger.LogError(String.Format("Erro ao atualizar role: O usuário {0} tentou atualizar a role {1}", _login, id));
            return BadRequest(new { error = "Não foi possível atualizar a role" });
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole([FromBody] IdentityRole role)
        {
            IdentityRole createRole = new IdentityRole();
            createRole.Name = role.Name;

            IdentityResult result = await _roleManager.CreateAsync(createRole);
            String _login = User.Claims.FirstOrDefault(c => c.Type == "Login").Value;

            if (result.Succeeded)
            {
                _logger.LogInformation(String.Format("O Usuário {0} Criou a role {1}", _login, role.Name));
                return CreatedAtAction(nameof(CreateRole), role);
            }
            _logger.LogError(String.Format("Erro ao criar role: O usuário {0} tentou criar uma nova role }", _login));
            return BadRequest(new { error = "Não foi possível criar Role" });
        }

        [Route("{id?}")]
        [HttpDelete]
        public async Task<IActionResult> Delete(String id)
        {
            IdentityRole role = await _roleManager.FindByIdAsync(id);
            IdentityResult result = await _roleManager.DeleteAsync(role);
            String _login = User.Claims.FirstOrDefault(c => c.Type == "Login").Value;
            if (result.Succeeded)
            {
                _logger.LogInformation(String.Format("O Usuário {0} Deletou a role {1}", _login, role.Name));
                return Ok();
            }

            _logger.LogError(String.Format("Erro ao criar role: O usuário {0} tentou deletar a role {0} }", _login, id));
            return BadRequest(new { error = "Não foi possível excluir role" });
        }

        [Route("Claim/{id?}")]
        [HttpPost]
        public async Task<IActionResult> AddClaimPermission(String id)
        {
            //IdentityRole role = await _roleManager.FindByIdAsync(id);
            //_roleManager.AddClaimAsync(role, new Claim())

            // Finalizar amanhã
        }
    }
}
