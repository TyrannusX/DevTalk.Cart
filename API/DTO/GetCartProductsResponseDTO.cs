using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTO
{
    public class GetCartProductsResponseDTO
    {
        public Guid CartId { get; set; }
        public string UserName { get; set; }
        public List<GetCartProductResponseDTO> CartProducts { get; set; }

        public GetCartProductsResponseDTO(Cart cart)
        {
            CartId = cart.CartId;
            UserName = cart.Customer.UserName;
            CartProducts = new List<GetCartProductResponseDTO>();
            cart.CartProducts?.ToList().ForEach(x =>
            {
                CartProducts.Add(new GetCartProductResponseDTO(x));
            });
        }
    }
}
