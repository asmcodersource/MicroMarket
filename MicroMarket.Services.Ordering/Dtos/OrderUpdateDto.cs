using MicroMarket.Services.Ordering.Models;

namespace MicroMarket.Services.Ordering.Dtos
{
    public class OrderUpdateDto
    {
        public DeliveryAddress DeliveryAddress { get; set; } = null!;
        public bool IsDelivered { get; set; } = false;
        public bool IsPaid { get; set; } = false;
        public bool IsClosed { get; set; } = false;
    }
}
