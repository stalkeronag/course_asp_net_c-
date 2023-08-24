using Dotnet.Web.Data;
using Dotnet.Web.Dto;
using Microsoft.AspNetCore.Mvc;
using Dotnet.Web.Models;
using System.Collections;
using Microsoft.AspNetCore.Authorization;
using Dotnet.Web.Interfaces;

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
        public async void PayOrder([FromRoute] int id)
        {
            await orderService.MoveOrderStatus(id, OrderStatus.Payed);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("Ship/{id}")]
        public async void ShipOrder([FromRoute] int id) 
        {
            await orderService.MoveOrderStatus(id, OrderStatus.Payed);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("Dispute/{id}")]
        public async void DisputeOrder([FromRoute] int id)
        {
            await orderService.MoveOrderStatus(id, OrderStatus.Payed);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("Complete/{id}")]
        public async void CompleteOrder([FromRoute] int id)
        {
            await orderService.MoveOrderStatus(id, OrderStatus.Payed);
        }

    }
}
