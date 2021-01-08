using Common.Application.Events.UserCreated;
using Domain.Contracts;
using Domain.Entities;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Consumers
{
    public class UserCreatedEventConsumer : IConsumer<UserCreatedIntegrationEvent>
    {
        private readonly IDbContext _dbContext;

        public UserCreatedEventConsumer(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Consume(ConsumeContext<UserCreatedIntegrationEvent> context)
        {
            using (var transactionScope = _dbContext.Database.BeginTransaction())
            {
                var customer = new Customer(context.Message.UserName);
                _dbContext.Customers.Add(customer);
                await _dbContext.SaveChangesAsync().ConfigureAwait(false);

                var cart = new Cart(customer.CustomerId);
                _dbContext.Carts.Add(cart);
                await _dbContext.SaveChangesAsync().ConfigureAwait(false);

                await transactionScope.CommitAsync().ConfigureAwait(false);
            }
        }
    }
}
