using Common.Application.Events.PriceUpdated;
using Domain.Contracts;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Consumers
{
    public class PriceUpdatedEventConsumer : IConsumer<PriceUpdatedIntegrationEvent>
    {
        private readonly IDbContext _dbContext;

        public PriceUpdatedEventConsumer(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Consume(ConsumeContext<PriceUpdatedIntegrationEvent> context)
        {
            var products = _dbContext.CartProducts.Where(x => x.ProductId == context.Message.ProductId).ToList();
            products.ForEach(x =>
            {
                x.UpdatePrice(context.Message.Price);
            });
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
