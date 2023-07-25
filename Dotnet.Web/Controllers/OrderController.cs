using Dotnet.Web.Data;
using Dotnet.Web.Dto;
using Microsoft.AspNetCore.Mvc;


namespace Dotnet.Web.Controllers
{
    public class OrderController : DotnetControllerBase
    {
        private readonly AppDbContext context;

        public OrderController(AppDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        [ProducesResponseType(typeof(OrderDto), 200)]
        public IActionResult GetOrder()
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        [ProducesResponseType(typeof(int), 200)]
        public IActionResult AddOrder()
        {
            throw new NotImplementedException();
        }


        [HttpPut("Pay/{id}")]
        public IActionResult PayOrder(int id)
        {
            throw new NotImplementedException();
        }

        [HttpPut("Ship/{id}")]
        public IActionResult ShipOrder(int id) 
        {
            throw new NotImplementedException();
        }


        [HttpPut("Dispute/{id}")]
        public IActionResult DisputeOrder(int id)
        {
            throw new NotImplementedException();
        }

        [HttpPut("Complete/{id}")]
        public IActionResult CompleteOrder(int id)
        {
            throw new NotImplementedException();
        }

    }
}
