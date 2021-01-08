using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class CartProduct
    {
        public Guid CartProductId { get; private set; }
        public Guid ProductId { get; private set; }
        public Guid CartId { get; private set; }
        public string Name { get; private set; }
        public decimal Price { get; private set; }

        public Cart Cart { get; set; }

        public CartProduct()
        {

        }

        public CartProduct(Guid productId, Guid cartId, string name, decimal price)
        {
            ProductId = productId;
            CartId = cartId;
            Name = name;
            Price = price;
        }

        public void UpdatePrice(decimal price)
        {
            Price = price;
        }
    }
}
