using MicroMarket.Services.Ordering.Models;

namespace MicroMarket.Services.Ordering.Interfaces
{
    public interface ICustomerFilterService
    {
        public IQueryable<Order> Filter(IQueryable<Order> orders, CustomerFilterOptions options);
    }
}
