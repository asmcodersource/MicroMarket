using MicroMarket.Services.Ordering.Models;

namespace MicroMarket.Services.Ordering.Dtos
{
    public class OrderStateDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public OrderStateType Type { get; set; } = OrderStateType.Default;
    }
}
