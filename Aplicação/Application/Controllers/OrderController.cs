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
using System.Threading.Tasks;


namespace Middleware.Application.Controllers
{
    /// <summary>
    /// Order Controller
    /// </summary>
    [ApiController]
    [Route("api/orders")]
    [Produces("application/json")]
    public class OrderController : ControllerBase
    {
        protected readonly UserManager<User> _userManager;
        protected readonly IRepository<Order> _orderRepository;
        private readonly ILogger<OrderController> _logger;

        public OrderController(IRepository<Order> orderRepository, UserManager<User> userManager, ILogger<OrderController> logger)
        {
            _orderRepository = orderRepository;
            _userManager = userManager;
            _logger = logger;
        }

        /// <summary>
        /// Return all Orders
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(policy: Permission.Orders.Manager)]
        public IActionResult GetAll()
        {
            String _login = User.Claims.FirstOrDefault(c => c.Type == "Login").Value;
            _logger.LogInformation(String.Format("O usuário {0} consultou todas as orders", _login));
            return Ok(_orderRepository.GetAll().ToList());
        }

        /// <summary>
        /// Returns a specific order
        /// </summary>
        /// <param name="id">Order identification string</param>
        /// <returns>Json Object</returns>
        [Route("{id}")]
        [HttpGet]
        [Authorize(policy: Permission.Orders.Viwer)]
        public IActionResult ViewOrder(string id)
        {
            Guid result = new Guid();
            if (Guid.TryParse(id, out result))
            {
                String _login = User.Claims.FirstOrDefault(c => c.Type == "Login").Value;
                Func<Order, bool> predicate = (r => r.Id.Equals(result) && _login.Equals(r.User.Login));
                Order order = _orderRepository.Get(predicate).FirstOrDefault();
                if (order != null)
                {
                    _logger.LogInformation(String.Format("O usuário {0} consultou a order {1}", _login, id));
                    return Ok(order);
                }
            }
            _logger.LogError(String.Format("Não foi possível localizar a order {0}", id));
            return BadRequest(new { error = "Não foi possível processar sua solicitação" });
        }

        /// <summary>
        /// Update approve order
        /// </summary>
        /// <param name="id">Order identification string</param>
        /// <returns>json Object</returns>
        [Route("update/approve/{id}")]
        [HttpPatch]
        [Authorize(policy: Permission.Orders.Viwer)]
        public async Task<IActionResult> ApproveRequest(string id)
        {
            Guid result = new Guid();
            if (Guid.TryParse(id, out result))
            {
                String _login = User.Claims.FirstOrDefault(c => c.Type == "Login").Value;
                Func<Order, bool> predicate = (r => r.Id.Equals(result) && _login.Equals(r.User.Login));
                Order order = _orderRepository.Get(predicate).FirstOrDefault();

                order.Request.Approval = true;
                order.Request.IsDptoPayment = true;
                order.Request.Status = Domain.Enumerators.Status.Triagem;
                order.Queue = Domain.Enumerators.Queue.Triagem;
                order.Request.User = await _userManager.GetUserAsync(User);
                order.Request.DescriptionDeclineApproval = String.Empty;

                _orderRepository.Update(order);
                _orderRepository.SaveAll();

                _logger.LogInformation(String.Format("O usuário {0} alterou a order {1}", _login, id));
                return Ok(order);
            }

            _logger.LogError(String.Format("Não foi possível processar sua solicitação para order:  {0}", id));
            return BadRequest(new { error = "Não foi possível processar sua solicitação" });
        }

        /// <summary>
        /// Recuse approve into in order
        /// </summary>
        /// <remarks>
        /// simple request: {
        ///    "description" : "teste"
        /// }
        /// </remarks>
        /// <param name="id">Order identification string</param>
        /// <param name="json">json request</param>
        /// <returns>return json Object</returns>
        [Route("update/recuse/{id}")]
        [HttpPatch]
        [Authorize(policy: Permission.Orders.Viwer)]
        public async Task<IActionResult> RecuseRequest(string id, [FromBody] JObject json)
        {
            Guid result = new Guid();
            String description = json.GetValue("description")?.ToString();
            if (Guid.TryParse(id, out result) && !String.IsNullOrEmpty(description))
            {
                String _login = User.Claims.FirstOrDefault(c => c.Type == "Login").Value;
                Func<Order, bool> predicate = (r => r.Id.Equals(result) && _login.Equals(r.User.Login));
                Order order = _orderRepository.Get(predicate).FirstOrDefault();
                if (order != null)
                {
                    order.Request.Approval = false;
                    order.Request.DescriptionDeclineApproval = description;
                    order.Request.IsDptoPayment = true;
                    order.Request.Status = Domain.Enumerators.Status.Triagem;
                    order.Queue = Domain.Enumerators.Queue.Triagem;
                    order.Request.User = await _userManager.GetUserAsync(User);

                    _orderRepository.Update(order);
                    _orderRepository.SaveAll();

                    _logger.LogInformation(String.Format("O usuário {0} alterou a order {1}", _login, id));
                    return Ok(order);
                }
            }

            _logger.LogError(String.Format("Não foi possível processar sua solicitação para order:  {0}", id));
            return BadRequest(new { error = "Não foi possível processar sua solicitação" });
        }

        /// <summary>
        /// Update user technician into in order
        /// </summary>
        /// <remarks>
        /// simple request: {
        ///    "description" : "teste"
        /// }
        /// </remarks>
        /// <param name="id">Order identification string</param>
        /// <param name="json">json request</param>
        /// <returns>return json Object</returns>
        [Route("update/Technician/{id}")]
        [HttpPut]
        [Authorize(policy: Permission.Orders.Viwer)]
        public IActionResult UpdateTechnicianOrder(string id, [FromBody] JObject json)
        {
            Guid result = new Guid();
            String description = json.GetValue("description")?.ToString();
            if (Guid.TryParse(id, out result) && !String.IsNullOrEmpty(description))
            {
                String _login = User.Claims.FirstOrDefault(c => c.Type == "Login").Value;
                Func<Order, bool> predicate = (r => r.Id.Equals(result) && _login.Equals(r.User.Login));
                Order order = _orderRepository.Get(predicate).FirstOrDefault();
                if (order != null)
                {
                    order.Request.TechnicianDescription = description;
                    order.Queue = Domain.Enumerators.Queue.Requisitante;
                    order.Request.Status = Domain.Enumerators.Status.Fechado;
                    _orderRepository.Update(order);
                    _orderRepository.SaveAll();

                    _logger.LogInformation(String.Format("O usuário {0} alterou a order {1}", _login, id));
                    return Ok(order);
                }
            }

            _logger.LogError(String.Format("Não foi possível processar sua solicitação para order:  {0}", id));
            return BadRequest(new { error = "Não foi possível processar sua solicitação" });
        }

        /// <summary>
        /// Update user support into in order
        /// </summary>
        /// <remarks>
        /// simple request: {
        ///    "description" : "teste"
        /// }
        /// </remarks>
        /// <param name="id">Order identification string</param>
        /// <param name="json">json request</param>
        /// <returns>return json Object</returns>
        [Route("update/support/{id}")]
        [Authorize(policy: Permission.Orders.Viwer)]
        [HttpPut]
        public IActionResult UpdateSupporteOrder(string id, [FromBody] JObject json)
        {
            Guid result = new Guid();
            String description = json.GetValue("description")?.ToString();
            if (Guid.TryParse(id, out result) && !String.IsNullOrEmpty(description))
            {
                String _login = User.Claims.FirstOrDefault(c => c.Type == "Login").Value;
                Func<Order, bool> predicate = (r => r.Id.Equals(result) && _login.Equals(r.User.Login));

                Order order = _orderRepository.Get(predicate).FirstOrDefault();
                if (order != null)
                {
                    order.Queue = Domain.Enumerators.Queue.Tecnico;
                    order.Request.DescriptionsSupport = description;
                    order.Request.Status = Domain.Enumerators.Status.Tecnico;
                    _orderRepository.Update(order);
                    _orderRepository.SaveAll();

                    _logger.LogInformation(String.Format("O usuário {0} alterou a order {1}", _login, id));
                    return Ok(order);
                }
            }

            _logger.LogError(String.Format("Não foi possível processar sua solicitação para order:  {0}", id));
            return BadRequest(new { error = "Não foi possível processar sua solicitação" });
        }


        /// <summary>
        /// Update date from schedulling order
        /// </summary>
        /// <remarks>
        /// simple request: {
        ///    "schedulling" : "dd-MM-yyyyHH:mm:ss"
        /// }
        /// </remarks>
        /// <param name="id">Order identification string</param>
        /// <param name="json">json request</param>
        /// <returns>return json Object</returns>
        [HttpPut]
        [Route("update/schedulling/{id}")]
        [Authorize(policy: Permission.Orders.Viwer)]
        public IActionResult SchedullingOrder(String id, [FromBody] JObject json)
        {
            Guid result = new Guid();
            String schedulling = json.GetValue("schedulling")?.ToString();
            if (Guid.TryParse(id, out result) && !String.IsNullOrEmpty(schedulling))
            {
                String _login = User.Claims.FirstOrDefault(c => c.Type == "Login").Value;
                Func<Order, bool> predicate = (r => r.Id.Equals(result) && _login.Equals(r.User.Login));

                Order order = _orderRepository.Get(predicate).FirstOrDefault();
                order.Request.Status = Domain.Enumerators.Status.Agendado;
                order.Request.Scheduling = DateTime.Parse(schedulling);

                _orderRepository.Update(order);
                _orderRepository.SaveAll();

                _logger.LogInformation(String.Format("O usuário {0} alterou a order {1}", _login, id));
                return Ok(order);
            }

            _logger.LogError(String.Format("Não foi possível processar sua solicitação para order:  {0}", id));
            return BadRequest(new { error = "Não foi possível processar sua solicitação" });
        }
    }
}
