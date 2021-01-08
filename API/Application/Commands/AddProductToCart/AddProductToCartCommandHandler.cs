using API.DTO;
using API.Exceptions;
using Domain.Contracts;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace API.Application.Commands.AddProductToCart
{
    public class AddProductToCartCommandHandler : IRequestHandler<AddProductToCartCommand, AddProductToCartResponseDTO>
    {
        private readonly IDbContext _dbContext;

        public AddProductToCartCommandHandler(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<AddProductToCartResponseDTO> Handle(AddProductToCartCommand request, CancellationToken cancellationToken)
        {
            var cart = _dbContext.Carts.Include(x => x.CartProducts).SingleOrDefault(x => x.CartId == request.CartId);
            if(cart == null)
            {
                throw new NotFoundException($"Cart with ID {request.CartId} not found");
            }

            var cartProduct = new CartProduct(request.ProductId, request.CartId, request.Name, request.Price);
            _dbContext.CartProducts.Add(cartProduct);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);

            return new AddProductToCartResponseDTO() { CartProductId = cartProduct.CartProductId};
        }
    }
}
