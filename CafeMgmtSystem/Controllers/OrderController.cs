using CafeManagementSystem.Models;
using CafeMgmtSystem.Models;
using CafeMgmtSystem.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace CafeMgmtSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly UserManager<ApplicationUser> _userManager;
        public OrderController(IOrderService orderService, UserManager<ApplicationUser> userManager)
        {
            _orderService = orderService;
            _userManager = userManager;
        }
        [HttpPost("PlaceOrder")]
        public async Task<IActionResult> PlaceOrder([FromBody] OrderRequest request)
        {
            var claimsList = User.Claims.ToList();
            var customerID = string.Empty;
            int index = 0;
            if (index < claimsList.Count)
            {
                var claim = claimsList[index];
                customerID = claim.Value;
            }
            var userDetails = await _userManager.FindByIdAsync(customerID);
            if (userDetails == null)
            {
                return NotFound();
            }
            var userFullName = userDetails.UserName;
            if (request.Items == null || !request.Items.Any())
            {
                return BadRequest("Order items are required.");
            }

            int orderId = _orderService.CreateOrder(customerID, request.ReservationId, request.Items, userFullName);
            return Ok(new { orderId = orderId });
        }
        [HttpPost("UpdateOrderStatus")]
        public IActionResult UpdateOrderStatus([FromQuery] int orderId, [FromQuery] int status)
        {
            bool isUpdated = _orderService.UpdateOrderStatus(orderId, status);
            if (isUpdated)
            {
                return Ok(new { orderId = orderId, status = status });
            }
            else
            {
                return BadRequest("Failed to update order status.");
            }
        }
    }
}
