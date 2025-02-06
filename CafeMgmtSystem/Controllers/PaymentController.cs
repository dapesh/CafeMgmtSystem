using CafeMgmtSystem.Models;
using CafeMgmtSystem.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
namespace CafeMgmtSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IHubContext<NotificationHub> _hubContext;
        public PaymentController(IOrderService orderService, IHubContext<NotificationHub> hubContext)
        {
            _orderService = orderService;
            _hubContext = hubContext;
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
                bool updated = await _orderService.UpdateOrderStatusAsync(request.OrderId, LatestOrderStatus.Paid);

                if (updated)
                {
                    await _hubContext.Clients.All.SendAsync("ReceiveMessage", "Your order has been completed!");
                    return Ok(new
                    {
                        message = "Payment processed successfully.",
                        orderId = request.OrderId,
                        status = LatestOrderStatus.Paid.ToString()
                    });
                }
                else
                {
                    return StatusCode(500, new { message = "Failed to update order status." });
                }
            }
            return BadRequest(new { message = "Payment failed." });
        }
        [HttpPost("prepare/{orderId}")]
        public async Task<IActionResult> MarkAsPreparing(int orderId)
        {
            bool updated = await _orderService.UpdateOrderStatusAsync(orderId, LatestOrderStatus.Processing);
            if (updated)
            {
                return Ok(new
                {
                    message = "Order is now being prepared.",
                    orderId = orderId,
                    status = LatestOrderStatus.Processing.ToString()
                });
            }

            return StatusCode(500, new { message = "Failed to update order status to Preparing." });
        }
        [HttpPost("ready/{orderId}")]
        public async Task<IActionResult> MarkAsReadyForPickup(int orderId)
        {
            bool updated = await _orderService.UpdateOrderStatusAsync(orderId, LatestOrderStatus.ReadyForPickup);
            if (updated)
            {
                return Ok(new
                {
                    message = "Order is ready for pickup.",
                    orderId = orderId,
                    status = LatestOrderStatus.ReadyForPickup.ToString()
                });
            }

            return StatusCode(500, new { message = "Failed to update order status to Ready for Pickup." });
        }
        [HttpPost("complete/{orderId}")]
        public async Task<IActionResult> MarkAsCompleted(int orderId)
        {
            bool updated = await _orderService.UpdateOrderStatusAsync(orderId, LatestOrderStatus.Completed);
            if (updated)
            {
                return Ok(new
                {
                    message = "Order has been completed.",
                    orderId = orderId,
                    status = LatestOrderStatus.Completed.ToString()
                });
            }

            return StatusCode(500, new { message = "Failed to update order status to Completed." });
        }
        [HttpPost("cancel/{orderId}")]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            bool updated = await _orderService.UpdateOrderStatusAsync(orderId, LatestOrderStatus.Cancelled);
            if (updated)
            {
                return Ok(new
                {
                    message = "Order has been cancelled.",
                    orderId = orderId,
                    status = LatestOrderStatus.Cancelled.ToString()
                });
            }

            return StatusCode(500, new { message = "Failed to cancel the order." });
        }
    }
}

