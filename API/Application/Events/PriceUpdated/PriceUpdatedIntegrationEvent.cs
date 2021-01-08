using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Common.Application.Events.PriceUpdated
{
    public class PriceUpdatedIntegrationEvent
    {
        public Guid ProductId { get; private set; }
        public decimal Price { get; private set; }

        public PriceUpdatedIntegrationEvent()
        {

        }

        public PriceUpdatedIntegrationEvent(Guid productId, decimal price)
        {
            ProductId = productId;
            Price = price;
        }
    }
}