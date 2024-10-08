﻿namespace MicroMarket.Services.SharedCore.MessageBus.MessageContracts
{
    public class ItemInformationResponse
    {
        public Guid ItemProductId { get; set; }
        public string ItemProductName { get; set; } = string.Empty;
        public decimal ItemProductPrice { get; set; }
        public int AvailableQuantity { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}
