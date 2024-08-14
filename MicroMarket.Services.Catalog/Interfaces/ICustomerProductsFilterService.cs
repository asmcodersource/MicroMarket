using MicroMarket.Services.Catalog.Models;

namespace MicroMarket.Services.Catalog.Interfaces
{
    public interface ICustomerProductsFilterService
    {
        public IQueryable<Product> Filter(IQueryable<Product> products, CustomerProductFilterOptions options);
    }
}
