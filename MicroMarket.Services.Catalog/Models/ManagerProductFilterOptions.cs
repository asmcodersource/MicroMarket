namespace MicroMarket.Services.Catalog.Models
{
    public class ManagerProductFilterOptions
    {
        public string? FilterByName { get; set; } = null;
        public decimal? FilterByPriceRangeBegin { get; set; } = 0;
        public decimal? FilterByPriceRangeEnd { get; set; } = decimal.MaxValue;
    }
}
