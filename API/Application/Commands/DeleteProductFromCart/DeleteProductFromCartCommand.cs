using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Application.Commands.DeleteProductFromCart
{
    public class DeleteProductFromCartCommand : IRequest<bool>
    {
        public Guid CartProductId { get; set; }

        public DeleteProductFromCartCommand()
        {

        }
    }
}
