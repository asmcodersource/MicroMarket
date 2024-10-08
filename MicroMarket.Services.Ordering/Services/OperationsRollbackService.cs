﻿using MicroMarket.Services.Ordering.DbContexts;
using MicroMarket.Services.Ordering.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MicroMarket.Services.Ordering.Services
{
    public class OperationsRollbackService : IOperationsRollbackService
    {
        private readonly OrderingDbContext _orderingDbContext;

        public OperationsRollbackService(OrderingDbContext orderingDbContext)
        {
            _orderingDbContext = orderingDbContext;
        }

        public async Task DraftOrderCreateRollback(Guid correlationId)
        {
            var operation = await _orderingDbContext.OutboxOperations
                .Where(o => o.CorrelationId == correlationId && o.State != Enums.OutboxState.RolledBack)
                .SingleOrDefaultAsync();
            if (operation is null)
                return;
            if (operation.OperationType != Enums.OutboxOperationType.CreateDraftOrder)
                throw new InvalidOperationException();
            var draftOrder = await _orderingDbContext.DraftOrders
                .SingleOrDefaultAsync(o => o.Id == operation.AggregationId);
            if( draftOrder is not null )
                _orderingDbContext.DraftOrders.Remove(draftOrder);
            operation.State = Enums.OutboxState.RolledBack;
            await _orderingDbContext.SaveChangesAsync();
        }
    }
}
