using Dotnet.Web.Data;
using Dotnet.Web.Dto;
using Microsoft.AspNetCore.Mvc;
using Dotnet.Web.Models;
using System.Collections;
using Microsoft.AspNetCore.Authorization;
using Dotnet.Web.Interfaces;
using Dotnet.Web.Admin.Exceptions;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Dotnet.Web.Controllers
{
    public class OrderController : DotnetControllerBase
    {

        private readonly IOrderService orderService;

        public OrderController(IOrderService orderService)
        {
            this.orderService = orderService;
        }

        [Authorize]
        [HttpGet]
        [ProducesResponseType(typeof(OrderDto), 200)]
        public async Task<OrderDto> GetOrder()
        {
           return await orderService.GetOrder();
        }

        [HttpPost]
        [ProducesResponseType(typeof(int), 200)]
        public async Task<int> AddOrder()
        {
           return await orderService.CreateOrder();
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("Pay/{id}")]
        public IActionResult PayOrder([FromRoute] int id)
        {
            if (id > 1000)
            {
                return StatusCode(422);
            }
            return Ok(orderService.MoveOrderStatus(id, OrderStatus.Payed));
            
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("Ship/{id}")]
        public IActionResult ShipOrder([FromRoute] int id) 
        {
            if (id > 1000)
            {
                return StatusCode(422);
            }
            return Ok(orderService.MoveOrderStatus(id, OrderStatus.Shipped));
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("Dispute/{id}")]
        public IActionResult DisputeOrder([FromRoute] int id)
        {
            if (id > 1000)
            {
                return StatusCode(422);
            }
            return Ok(orderService.MoveOrderStatus(id, OrderStatus.Disputed));
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("Complete/{id}")]
        public IActionResult CompleteOrder([FromRoute] int id)
        {
            if (id > 1000)
            {
                return StatusCode(422);
            }
            return Ok(orderService.MoveOrderStatus(id, OrderStatus.Completed));
        }

    }
}
