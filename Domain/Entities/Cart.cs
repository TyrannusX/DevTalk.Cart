using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Cart
    {
        public Guid CartId { get; private set; }
        public Guid CustomerId { get; private set; }
        
        //Navigation Properties
        public virtual Customer Customer { get; set; }
        public virtual ICollection<CartProduct> CartProducts { get; set; }

        public Cart()
        {

        }

        public Cart(Guid customerId)
        {
            CustomerId = customerId;
        }
    }
}
