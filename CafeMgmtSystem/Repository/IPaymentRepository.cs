namespace CafeMgmtSystem.Repository
{
    public interface IPaymentRepository
    {
        Task<bool> UpdateOrderStatusAsync(int orderId, int status);
    }
}
