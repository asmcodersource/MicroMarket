using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroMarket.Services.SharedCore.MessageBus.MessageContracts
{
    public class ClaimedItemsResponse
    {
        public record ClaimedItem
        {
            public Guid ProductId { get; init; }
            public string ProductName { get; init; } = string.Empty;
            public string ProductDescription { get; init; } = string.Empty;
            public decimal ProductPrice { get; init; }
            public int ProductQuantity { get; init; }
        }

        public ICollection<ClaimedItem> ClaimedItems { get; set; } = new List<ClaimedItem>();
    }
}
