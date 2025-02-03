using CafeMgmtSystem.Models;
using CafeMgmtSystem.Repository;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace CafeMgmtSystem.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMenuRepository _menuRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OrderService(IOrderRepository orderRepository, IMenuRepository menuRepository, IHttpContextAccessor httpContextAccessor)
        {
            _orderRepository = orderRepository;
            _menuRepository = menuRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public int CreateOrder(string customerId,int TableID, List<OrderItem> items, string userFullName)
        {
            decimal totalAmount = 0;

            foreach (var item in items)
            {
                var menuItem = _menuRepository.GetMenuItemById(item.Id);
                if (menuItem == null)
                {
                    throw new Exception($"Menu item with ID {item.Id} not found.");
                }

                item.Price = menuItem.Price;
                totalAmount += item.Price * item.Quantity;
            }

            var order = new Order
            {
                CustomerId = customerId,
                OrderDate = DateTime.Now,
                TotalAmount = totalAmount,
                OrderStatus = new OrderStatus { Status= "Pending" },
                ReservationId= TableID,
                CustomerName = userFullName
            };

            int orderId = _orderRepository.CreateOrder(order);

            foreach (var item in items)
            {
                item.OrderId = orderId;
                _orderRepository.AddItemToOrder(item);
            }

            return orderId;
        }

        public bool UpdateOrderStatus(int orderId, int status)
        {
            int affectedRows = _orderRepository.UpdateOrderStatus(orderId, status);
            return affectedRows > 0;
        }
    }

}
