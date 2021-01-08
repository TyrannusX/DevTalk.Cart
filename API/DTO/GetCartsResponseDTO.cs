using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTO
{
    public class GetCartsResponseDTO
    {
        public List<GetCartProductsResponseDTO> Carts { get; set; }

        public GetCartsResponseDTO(IReadOnlyCollection<Cart> carts)
        {
            Carts = new List<GetCartProductsResponseDTO>();
            carts.ToList().ForEach(x =>
            {
                Carts.Add(new GetCartProductsResponseDTO(x));
            });
        }
    }
}
