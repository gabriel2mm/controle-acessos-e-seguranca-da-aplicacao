using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Middleware.Domain.Interfaces;
using Middleware.Domain.Models;
using Middleware.Infrastructure.Respositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Application.Controllers
{
    [ApiController]
    [Route("api/modules")]
    [Produces("Application/json")]
    public class ModuleController : ControllerBase
    {
        private readonly IRepository<Module> _repository;
        private readonly IRepository<Organization> _orgRepository;
        private readonly IRepository<User> _userRepository;
        public ModuleController(IRepository<Module> repository, IRepository<Organization> orgRepository, IRepository<User> userRepository)
        {
            _repository = repository;
            _orgRepository = orgRepository;
            _userRepository = userRepository;
        }

        [HttpGet]
        [Authorize]
        public IActionResult GetAll()
        {
            Guid.TryParse(User.Claims.Where(c => c.Type.Equals(ClaimTypes.NameIdentifier)).FirstOrDefault().Value, out Guid UserID);
            User loggedUser = _userRepository.Find(UserID);
            IList<Module> modules = _repository.GetAll().Where(u => u.Organization == loggedUser.Organization && u.Active == true).ToList();
            return Ok(modules);
        }

        [HttpGet]
        [Route("all")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetAllAdmin()
        {
            Guid.TryParse(User.Claims.Where(c => c.Type.Equals(ClaimTypes.NameIdentifier)).FirstOrDefault().Value, out Guid UserID);
            User loggedUser = _userRepository.Find(UserID);
            IList<Module> modules = _repository.GetAll().ToList();
            return Ok(modules);
        }

        [Route("{id}")]
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Find(String id)
        {
            if (Guid.TryParse(id, out Guid result))
            {
                Module module = _repository.Find(result);
                return Ok(module);
            }

            return BadRequest(new { error = "Não foi possível processar a solicitação" });
        }

        [Route("{id}")]
        [HttpPut]
        [Authorize(Roles = "Admin")]
        public IActionResult Update(String id, [FromBody] Module obj)
        {
            if (Guid.TryParse(id, out Guid result))
            {
                Module module = _repository.Find(result);
                Organization organization = _orgRepository.Find(obj.Organization.Id);
                if (organization != null)
                {
                    module.Organization = organization;
                }
                
                module.Name = obj.Name;
                module.Permission = obj.Permission;
                module.Active = obj.Active;

                _repository.Update(module);
                _repository.SaveAll();
                return Ok(module);
            }

            return BadRequest(new { error = "Não foi possível processar a solicitação" });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Create([FromBody] Module module)
        {
            try
            {
                Organization organization = _orgRepository.Find(module.Organization.Id);
                if (organization != null)
                {
                    module.Organization = organization;
                }
                module.Active = true;
                _repository.Add(module);
                _repository.SaveAll();
                return Ok(module);
            }
            catch (Exception)
            {
                return BadRequest(new { error = "Não foi possível processar a solicitação" });
            }
        }

        [Route("{id}")]
        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(String id)
        {
            Guid result = new Guid();
            if (Guid.TryParse(id, out result))
            {
                Module module = _repository.Find(result);
                _repository.Delete(p => p.Id.Equals(module.Id));
                return Ok();
            }

            return BadRequest(new { error = "Não foi possível processar a solicitação" });
        }
    }
}
