using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Middleware.Domain.Interfaces;
using Middleware.Domain.Models;
using Middleware.Utils.Helpers;
using System;
using System.Linq;

namespace Middleware.Application.Controllers
{
    /// <summary>
    /// Request controller
    /// </summary>


    [ApiController]
    [Route("api/requests")]
    [Produces("application/json")]
    public class RequestController : Controller
    {
        private readonly IRepository<Request> _requestRepository;
        private readonly ILogger<RequestController> _logger;
        public RequestController(IRepository<Request> requestRepository, ILogger<RequestController> logger)
        {
            _requestRepository = requestRepository;
            _logger = logger;
        }

        /// <summary>
        /// Returns all registered Request Object
        /// </summary>
        /// <returns>Json Object request</returns>
        [HttpGet]
        [Authorize(policy: Permission.Requests.Manager)]
        public IActionResult GetAll()
        {
            String _login = User.Claims.FirstOrDefault(c => c.Type == "Login").Value;
            _logger.LogInformation(String.Format("O Usuário {0} consultou todas as requests", _login));
            return Ok(_requestRepository.GetAll().ToList());
        }

        /// <summary>
        /// Returns a specific Request Object
        /// </summary>
        /// <param name="id">Request identification string</param>
        /// <returns>Json Object request</returns>
        [HttpGet]
        [Route("/{id}")]
        [Authorize(policy: Permission.Requests.Viwer)]
        public IActionResult ViewRequest(String id)
        {
            Guid result = new Guid();
            if (Guid.TryParse(id, out result))
            {
                String _login = User.Claims.FirstOrDefault(c => c.Type == "Login").Value;
                Func<Request, bool> predicate = (r => _login.Equals(r.User.Login) && r.Id.Equals(result));
                Request request = _requestRepository.Get(predicate).FirstOrDefault();
                if (request != null)
                {
                    _logger.LogInformation(String.Format("O Usuário {0} consultou a request {1}", _login, request.Id));
                    return Ok(request);
                }
            }
            _logger.LogError(String.Format("Não foi possível localizar o request {0}", id));
            return BadRequest(new { error = "Não foi possível localizar request" });
        }

        /// <summary>
        /// Update Request
        /// </summary>
        /// <param name="id">Request identification string</param>
        /// <param name="request">Json object</param>
        /// <returns>Status code 200</returns>
        [HttpPost]
        [Route("/{id}")]
        [Authorize(policy: Permission.Requests.Viwer)]
        public IActionResult EditRequest(String id, [FromBody] Request request)
        {
            Guid result = new Guid();
            if (Guid.TryParse(id, out result))
            {
                String _login = User.Claims.FirstOrDefault(c => c.Type == "Login").Value;
                Func<Request, bool> predicate = (r => _login.Equals(r.User.Login) && r.Id.Equals(result));
                Request tempRequest = _requestRepository.Get(predicate).FirstOrDefault();
                if (tempRequest != null)
                {
                    tempRequest.Equipament = request.Equipament;
                    tempRequest.Description = request.Description;
                    tempRequest.Type = request.Type;
                    tempRequest.IsDptoPayment = false;
                    tempRequest.DescriptionDeclineApproval = String.Empty;
                    tempRequest.DescriptionsSupport = String.Empty;
                    _requestRepository.Update(tempRequest);
                    _requestRepository.SaveAll();

                    _logger.LogInformation(String.Format("O usuário {0} atualizou o request {1}", _login, tempRequest.Id));
                    return Ok();
                }
            }
            _logger.LogError(String.Format("Não foi possível localizar o request {0}", id));
            return BadRequest(new { error = "Não foi possível processar sua solicitação" });
        }
    }
}
