using CafeMgmtSystem.Models;
using CafeMgmtSystem.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CafeMgmtSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IOrderService _orderService;
        public PaymentController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        [HttpPost("process")]
        public async Task<IActionResult> ProcessPayment([FromBody] PaymentRequest request)
        {
            if (request == null || request.OrderId <= 0)
            {
                return BadRequest(new { message = "Invalid request. Order ID is required." });
            }

            // Simulate payment success
            bool paymentSuccess = true;

            if (paymentSuccess)
            {
                // Update order status to 'Paid' (Assume 2 is the status for 'Paid')
                bool updated = await _orderService.UpdateOrderStatusAsync(request.OrderId, 2);

                if (updated)
                {
                    return Ok(new
                    {
                        message = "Payment processed successfully.",
                        orderId = request.OrderId,
                        status = "Paid"
                    });
                }
                else
                {
                    return StatusCode(500, new { message = "Failed to update order status." });
                }
            }

            return BadRequest(new { message = "Payment failed." });
        }
    }
}

