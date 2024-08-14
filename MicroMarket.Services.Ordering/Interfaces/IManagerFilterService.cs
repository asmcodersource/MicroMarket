using MicroMarket.Services.Ordering.Models;

namespace MicroMarket.Services.Ordering.Interfaces
{
    public interface IManagerFilterService
    {
        public IQueryable<Order> Filter(IQueryable<Order> orders, ManagerFilterOptions options);
    }
}
