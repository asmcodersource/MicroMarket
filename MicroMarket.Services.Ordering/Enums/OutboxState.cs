namespace MicroMarket.Services.Ordering.Enums
{
    public enum OutboxState
    {
        Executing,
        Completed,
        Cancelled,
        Failure
    }
}
