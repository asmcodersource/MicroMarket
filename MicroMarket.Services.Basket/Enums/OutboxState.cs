﻿namespace MicroMarket.Services.Basket.Enums
{
    public enum OutboxState
    {
        Executing,
        Completed,
        RolledBack,
        Failure
    }
}
