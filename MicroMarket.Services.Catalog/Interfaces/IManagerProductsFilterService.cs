using MicroMarket.Services.Catalog.Models;

namespace MicroMarket.Services.Catalog.Interfaces
{
    public interface IManagerProductsFilterService
    {
        public IQueryable<Product> Filter(IQueryable<Product> products, ManagerProductFilterOptions options);
    }
}
