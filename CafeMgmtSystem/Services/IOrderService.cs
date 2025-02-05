using CafeMgmtSystem.Models;

namespace CafeMgmtSystem.Services
{
    public interface IOrderService
    {
        int CreateOrder(string customerId,int TableID, List<OrderItem> items, string userFullName);
        bool UpdateOrderStatus(int orderId, int status);
        Task<bool> UpdateOrderStatusAsync(int orderId, int status);

    }
}
