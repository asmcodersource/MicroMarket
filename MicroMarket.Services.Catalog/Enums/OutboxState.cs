namespace MicroMarket.Services.Catalog.Enums
{
    public enum OutboxState
    {
        Executing,
        Completed,
        RolledBack,
        Failure
    }
}
