
using MicroMarket.Services.Basket.DbContexts;
using MicroMarket.Services.Basket.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MicroMarket.Services.Basket.Services
{
    public class RollbackWorkerService : BackgroundService, IRollbackWorkerService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public RollbackWorkerService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<BasketDbContext>();
                    var operationsRollbackService = scope.ServiceProvider.GetRequiredService<IOperationsRollbackService>();
                    var transaction = await dbContext.Database.BeginTransactionAsync();
                    try
                    {
                        var operations = await dbContext.OutboxOperations
                            .Where(o => o.State == Enums.OutboxState.Failure)
                            .ToListAsync();
                        foreach (var operation in operations)
                        {
                            switch (operation.OperationType)
                            {
                                case Enums.OutboxOperationType.OrderCreating:
                                    await operationsRollbackService.OrderCreatingRollback(operation.CorrelationId);
                                    break;
                            }
                        }
                        await dbContext.SaveChangesAsync();
                        await transaction.CommitAsync();
                    }
                    catch
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
                await Task.Delay(1000);
            }
        }
    }
}
