using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTO
{
    public class GetCartProductResponseDTO
    {
        public Guid CartProductId { get; set; }
        public Guid ProductId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }

        public GetCartProductResponseDTO(CartProduct cartProduct)
        {
            CartProductId = cartProduct.CartProductId;
            ProductId = cartProduct.ProductId;
            Name = cartProduct.Name;
            Price = cartProduct.Price;
        }
    }
}
