using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroMarket.Services.SharedCore.MessageBus.MessageContracts
{
    public class AddItemToBasket
    {
        public Guid ItemProductId { get; set; }
        public int RequiredQuantity { get; set; }
    }
}
