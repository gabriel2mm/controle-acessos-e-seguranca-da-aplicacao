using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Middleware.Domain.Interfaces;
using Middleware.Domain.Models;
using Middleware.Infrastructure.Respositories;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Controllers
{
    [ApiController]
    [Authorize(Roles= "Admin")]
    [Route("api/organizations")]
    [Produces("Application/json")]
    public class OrganizationController : ControllerBase
    {
        private readonly IRepository<Organization> _repository;
        public OrganizationController(IRepository<Organization> repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            IList<Organization> organizations = _repository.GetAll().ToList();
            return Ok(organizations);
        }

        [Route("{id}")]
        [HttpGet]
        public IActionResult Find(String id)
        {
            if (Guid.TryParse(id, out Guid result))
            {
                Organization organization = _repository.Find(result);
                return Ok(organization);
            }

            return BadRequest(new { error = "Não foi possível processar a solicitação" });
        }

        [HttpPost]
        public IActionResult Create([FromBody] Organization organization)
        {
            try
            {
                organization.Modules = new List<Module>();
                organization.Active = true;
                _repository.Add(organization);
                _repository.SaveAll();
                return Ok(organization);
            }
            catch (Exception)
            {
                return BadRequest(new { error = "Não foi possível processar a solicitação" });
            }
        }

        [Route("{id}")]
        [HttpDelete]
        public IActionResult Delete(String id)
        {
            Guid result = new Guid();
            if (Guid.TryParse(id, out result))
            {
                Organization organization = _repository.Find(result);
                _repository.Delete(p => p.Id.Equals(organization.Id));
                return Ok();
            }

            return BadRequest(new { error = "Não foi possível processar a solicitação" });
        }
    }
}
