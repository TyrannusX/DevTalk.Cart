using API.Exceptions;
using Domain.Contracts;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace API.Application.Commands.DeleteProductFromCart
{
    public class DeleteProductFromCartCommandHandler : IRequestHandler<DeleteProductFromCartCommand, bool>
    {
        private readonly IDbContext _dbContext;

        public DeleteProductFromCartCommandHandler(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> Handle(DeleteProductFromCartCommand request, CancellationToken cancellationToken)
        {
            var cartProduct = _dbContext.CartProducts.SingleOrDefault(x => x.CartProductId == request.CartProductId);
            if (cartProduct == null)
            {
                throw new NotFoundException($"Cart product with ID {request.CartProductId} not found");
            }

            _dbContext.CartProducts.Remove(cartProduct);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);

            return true;
        }
    }
}
