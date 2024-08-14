using MicroMarket.Services.Ordering.Interfaces;

namespace MicroMarket.Services.Ordering.Models
{
    public class ManagerFilterOptions
    {
        public string? FilterByOrderId { get; set; } = null;

        public string? FilterByCustomerId { get; set; } = null;

        public bool? IncludeDelivered { get; set; } = null;

        public bool? IncludePaid { get; set; } = null;

        public bool? IncludeClosed { get; set; } = null;

        public DateTime? FilterByOrderDateRangeBegin { get; set; } = DateTime.MinValue;

        public DateTime? FilterByOrderDateRangeEnd { get; set; } = DateTime.MaxValue;
    }
}
