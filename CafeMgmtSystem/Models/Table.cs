namespace CafeMgmtSystem.Models
{
    public class Table
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Seats { get; set; }
        public bool IsAvailable { get; set; } // Added to track availability
        public DateTime? ReservedUntil { get; set; } // To track when a reservation expires
    }

    public class MenuItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; }
    }
    public class Order
    {
        public string Id { get; set; }
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public int ReservationId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; }
        public OrderStatus OrderStatus { get; set; }
    }
    public class OrderRequest
    {
        public string CustomerId { get; set; }
        public int ReservationId { get; set; }
        public List<OrderItem> Items { get; set; }
    }

    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int MenuItemId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public Order Order { get; set; }
    }
    public class OrderStatus
    {
        public int Id { get; set; }
        public string Status { get; set; } 
    }

    public class Payment
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentStatus { get; set; } // Pending, Successful, Failed
        public DateTime PaymentDate { get; set; }
    }
    public class Reservation
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int TableId { get; set; }
        public bool IsConfirmed { get; set; }
        public DateTime ReservationDate { get; set; }
    }
}
