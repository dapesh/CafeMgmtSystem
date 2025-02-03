using CafeMgmtSystem.Models;

namespace CafeMgmtSystem.Repository
{
    public interface IOrderRepository
    {
        int CreateOrder(Order order);
        int AddItemToOrder(OrderItem orderItem);
        Order GetOrderById(int id);
        int UpdateOrderStatus(int orderId, int status);
        int AddPayment(Payment payment);
        int CreateReservation(Reservation reservation);
    }
}
