using CafeMgmtSystem.Models;
namespace CafeMgmtSystem.Repository
{
    public interface IPaymentRepository
    {
        Task<bool> UpdateOrderStatusAsync(int orderId, LatestOrderStatus status);
    }
}
