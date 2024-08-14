using MicroMarket.Services.Ordering.Interfaces;
using MicroMarket.Services.Ordering.Models;

namespace MicroMarket.Services.Ordering.Services
{
    public class CustomerFilterService : ICustomerFilterService
    {
        public IQueryable<Order> Filter(IQueryable<Order> orders, CustomerFilterOptions options)
        {
            if (options.IncludePaid is not null)
                orders = orders.Where(o => (!options.IncludePaid.Value) ^ o.IsPaid);
            if (options.IncludeDelivered is not null)
                orders = orders.Where(o => (!options.IncludeDelivered.Value) ^ o.IsPaid);
            if (options.IncludeClosed is not null)
                orders = orders.Where(o => (!options.IncludeClosed.Value) ^ o.IsPaid);
            if (options.FilterByOrderDateRangeBegin is not null)
                orders = orders.Where(o => o.CreatedAt >= options.FilterByOrderDateRangeBegin);
            if (options.FilterByOrderDateRangeEnd is not null)
                orders = orders.Where(o => o.CreatedAt <= options.FilterByOrderDateRangeEnd);
            return orders;
        }
    }
}
