using CafeMgmtSystem.Models;
using CafeMgmtSystem.Services;
using Dapper;
using System.Data;

namespace CafeMgmtSystem.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;
        public OrderRepository(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }
        private IDbConnection Connection => _dbConnectionFactory.CreateConnection();
        public int AddItemToOrder(OrderItem orderItem)
        {
            using (var connection = Connection)
            {
                connection.Open();
                var result = connection.Execute(
                    "AddItemToOrder",
                    new
                    {
                        orderItem.OrderId,
                        orderItem.MenuItemId,
                        orderItem.ProductName,
                        orderItem.Quantity,
                        orderItem.Price
                    },
                    commandType: CommandType.StoredProcedure
                );

                return result; 
            }
        }
        public int CreateOrder(Order order)
        {
            using (var connection = Connection)
            {
                connection.Open();
                var parameters = new DynamicParameters();
                parameters.Add("TotalAmount", order.TotalAmount);
                parameters.Add("Status", order.OrderStatus.Status);
                parameters.Add("TableId", order.ReservationId);
                parameters.Add("CustomerId", order.CustomerId);
                parameters.Add("CustomerName", order.CustomerName);
                int orderId = connection.QuerySingle<int>(
                "CreateOrder",
                parameters,
                commandType: CommandType.StoredProcedure
                );
                return orderId;
            }
        }
        public Order GetOrderById(int id)
        {
            using (var connection = Connection)
            {
                connection.Open();
                var query = @"SELECT o.*, os.Status AS OrderStatus 
                              FROM Orders o
                              LEFT JOIN OrderStatuses os ON o.OrderStatusId = os.Id
                              WHERE o.Id = @Id";

                var order = connection.QuerySingleOrDefault<Order>(
                    query,
                    new { Id = id }
                );

                return order;
            }
        }
        public int UpdateOrderStatus(int orderId, int statusId)
        {
            using (var connection = Connection)
            {
                connection.Open();
                var result = connection.Execute(
                    "UpdateOrderStatus",
                    new { OrderId = orderId, StatusId = statusId },
                    commandType: CommandType.StoredProcedure
                );
                return result; 
            }
        }
        public int AddPayment(Payment payment)
        {
            using (var connection = Connection)
            {
                connection.Open();

                var result = connection.Execute(
                    "AddPayment", 
                    new
                    {
                        payment.OrderId,
                        payment.Amount,
                        payment.PaymentStatus,
                        payment.PaymentDate
                    },
                    commandType: CommandType.StoredProcedure
                );

                return result; 
            }
        }
        public int CreateReservation(Reservation reservation)
        {
            using (var connection = Connection)
            {
                connection.Open();
                var result = connection.Execute(
                    "CreateReservation", 
                    new
                    {
                        reservation.UserId,
                        reservation.TableId,
                        reservation.IsConfirmed,
                        reservation.ReservationDate
                    },
                    commandType: CommandType.StoredProcedure
                );
                return result;
            }
        }
    }
}
