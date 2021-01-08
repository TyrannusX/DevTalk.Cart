using API.DTO;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Application.Commands.AddProductToCart
{
    public class AddProductToCartCommand : IRequest<AddProductToCartResponseDTO>
    {
        public Guid CartId { get; set; }
        public Guid ProductId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }

        public AddProductToCartCommand()
        {

        }
    }
}
