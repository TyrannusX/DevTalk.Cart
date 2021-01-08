using API.Application.Commands.AddProductToCart;
using API.Application.Commands.DeleteProductFromCart;
using API.DTO;
using API.Exceptions;
using Domain.Contracts;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    public class CartController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IQueries<Cart> _queries;

        public CartController(IMediator mediator, IQueries<Cart> queries)
        {
            _mediator = mediator;
            _queries = queries;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Carts()
        {
            var result = await _queries.GetAllAsync();
            return Ok(new GetCartsResponseDTO(result));
        }

        [HttpGet]
        [Route("{cartId}")]
        [Authorize]
        public async Task<IActionResult> Carts([FromRoute] Guid cartId)
        {
            var result = await _queries.GetByIdAsync(cartId);
            if(result == null)
            {
                throw new NotFoundException($"Cart with ID {cartId} not found");
            }
            return Ok(new GetCartProductsResponseDTO(result));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Cart([FromBody] AddProductToCartCommand addProductToCartCommand)
        {
            var result = await _mediator.Send(addProductToCartCommand);
            return StatusCode((int)HttpStatusCode.Created, result);
        }

        [HttpDelete]
        [Route("{cartProductId}")]
        [Authorize]
        public async Task<IActionResult> Cart([FromRoute] Guid cartProductId)
        {
            await _mediator.Send(new DeleteProductFromCartCommand() { CartProductId = cartProductId});
            return NoContent();
        }
    }
}
