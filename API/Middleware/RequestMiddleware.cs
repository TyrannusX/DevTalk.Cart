using API.Exceptions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace API.Middleware
{
    public class RequestMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                switch (ex)
                {
                    case BadRequestException _:
                        httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        break;
                    case NotFoundException _:
                        httpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
                        break;
                    default:
                        httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        break;
                }

                await httpContext.Response.WriteAsync(ex.Message);
            }
        }
    }
}
