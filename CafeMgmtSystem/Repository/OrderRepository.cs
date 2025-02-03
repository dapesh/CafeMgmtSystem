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

                // Insert item into order
                var result = connection.Execute(
                    "AddItemToOrder", // Ensure the stored procedure name
                    new
                    {
                        orderItem.OrderId,
                        orderItem.MenuItemId, // Ensure MenuItemId is valid
                        orderItem.ProductName,
                        orderItem.Quantity,
                        orderItem.Price
                    },
                    commandType: CommandType.StoredProcedure
                );

                return result; // Number of rows affected
            }
        }
        public int CreateOrder(Order order)
        {
            using (var connection = Connection)
            {
                connection.Open();

                // Declare the output parameter for the OrderId
                var parameters = new DynamicParameters();
                parameters.Add("TotalAmount", order.TotalAmount);
                parameters.Add("Status", order.OrderStatus.Status);
                parameters.Add("TableId", order.ReservationId);
                parameters.Add("CustomerId", order.CustomerId);
                parameters.Add("CustomerName", order.CustomerName);

                // Add TableId here
                // Assuming OrderStatus is passed with ID
                //parameters.Add("OrderId", dbType: DbType.Int32, direction: ParameterDirection.Output); // Output parameter

                // Execute the stored procedure
                //connection.Execute(
                //    "CreateOrder",
                //    parameters,
                //    commandType: CommandType.StoredProcedure
                //);
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

                // Fetch the order with its related items and status
                var query = @"SELECT o.*, os.Status AS OrderStatus 
                              FROM Orders o
                              LEFT JOIN OrderStatuses os ON o.OrderStatusId = os.Id
                              WHERE o.Id = @Id";

                var order = connection.QuerySingleOrDefault<Order>(
                    query,
                    new { Id = id }
                );

                return order; // Return the order with its items and status
            }
        }
        public int UpdateOrderStatus(int orderId, int statusId)
        {
            using (var connection = Connection)
            {
                connection.Open();

                // Update order status in the database
                var result = connection.Execute(
                    "UpdateOrderStatus", // Ensure the stored procedure name
                    new { OrderId = orderId, StatusId = statusId },
                    commandType: CommandType.StoredProcedure
                );

                return result; // Should return the number of rows affected (should be 1 if successful)
            }
        }
        public int AddPayment(Payment payment)
        {
            using (var connection = Connection)
            {
                connection.Open();

                var result = connection.Execute(
                    "AddPayment", // Ensure the stored procedure name
                    new
                    {
                        payment.OrderId,
                        payment.Amount,
                        payment.PaymentStatus,
                        payment.PaymentDate
                    },
                    commandType: CommandType.StoredProcedure
                );

                return result; // Returns the number of rows affected
            }
        }
        public int CreateReservation(Reservation reservation)
        {
            using (var connection = Connection)
            {
                connection.Open();

                var result = connection.Execute(
                    "CreateReservation", // Ensure the stored procedure name
                    new
                    {
                        reservation.UserId,
                        reservation.TableId,
                        reservation.IsConfirmed,
                        reservation.ReservationDate
                    },
                    commandType: CommandType.StoredProcedure
                );

                return result; // Returns the number of rows affected
            }
        }
    }
}
