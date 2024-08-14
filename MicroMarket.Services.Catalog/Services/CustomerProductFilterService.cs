using MicroMarket.Services.Catalog.Interfaces;
using MicroMarket.Services.Catalog.Models;
using Microsoft.EntityFrameworkCore;

namespace MicroMarket.Services.Catalog.Services
{
    public class CustomerProductFilterService : ICustomerProductsFilterService
    {
        public IQueryable<Product> Filter(IQueryable<Product> products, CustomerProductFilterOptions options)
        {
            if (options.FilterByName is not null)
                products = products.Where(p => EF.Functions.Like(p.Name, options.FilterByName));
            if (options.FilterByPriceRangeBegin is not null)
                products = products.Where(p => p.Price >= options.FilterByPriceRangeBegin);
            if (options.FilterByPriceRangeEnd is not null)
                products = products.Where(p => p.Price <= options.FilterByPriceRangeEnd);
            return products;
        }
    }
}
