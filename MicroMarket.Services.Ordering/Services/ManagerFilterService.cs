using MicroMarket.Services.Ordering.Interfaces;
using MicroMarket.Services.Ordering.Models;
using Microsoft.EntityFrameworkCore;

namespace MicroMarket.Services.Ordering.Services
{
    public class ManagerFilterService : IManagerFilterService
    { 
        public IQueryable<Order> Filter(IQueryable<Order> orders, ManagerFilterOptions options)
        {
            if (options.FilterByOrderId is not null)
                orders = orders.Where(o => o.Id.ToString().Contains(options.FilterByOrderId));
            if (options.FilterByCustomerId is not null)
                orders = orders.Where(o => o.CustomerId.ToString().Contains(options.FilterByCustomerId));
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
