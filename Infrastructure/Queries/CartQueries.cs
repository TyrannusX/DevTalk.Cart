using Dapper;
using Domain.Contracts;
using Domain.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Queries
{
    public class CartQueries : IQueries<Cart>
    {
        private readonly IConfiguration _configuration;

        public CartQueries(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IReadOnlyCollection<Cart>> GetAllAsync()
        {
            var sql = @"SELECT c.CartId, cp.CartProductId, cp.CartId, cp.Name, cp.Price, cus.CustomerId, cus.UserName FROM Carts as c
                        JOIN CartProducts as cp ON c.Cartid = cp.CartId
                        JOIN Customers as cus ON c.CustomerId = cus.CustomerId";

            using (var connection = new SqlConnection(_configuration["CartsConnectionString"]))
            {
                var result = await connection.QueryAsync<Cart, CartProduct, Customer, Cart>(sql, (cart, cartProduct, customer) =>
                {
                    cart.CartProducts = cart.CartProducts ?? new List<CartProduct>();
                    cart.CartProducts.Add(cartProduct);
                    cart.Customer = cart.Customer ?? new Customer();
                    cart.Customer = customer;
                    return cart;
                }, splitOn: "CartProductId,CustomerId").ConfigureAwait(false);

                if(result == null || result.Count() == 0)
                {
                    var cartsWithNoProductsSql = @"SELECT c.CartId, cus.CustomerId, cus.UserName FROM Carts as c
                        JOIN Customers as cus ON c.CustomerId = cus.CustomerId";
                    result = await connection.QueryAsync<Cart, Customer, Cart>(cartsWithNoProductsSql, (cart, customer) =>
                    {
                        cart.Customer = cart.Customer ?? new Customer();
                        cart.Customer = customer;
                        return cart;
                    }, splitOn: "CustomerId").ConfigureAwait(false);
                    return result.ToList();
                }

                return result.ToList();
            }
        }

        public async Task<Cart> GetByIdAsync(Guid id)
        {
            var sql = @"SELECT c.CartId, cp.CartProductId, cp.ProductId, cp.CartId, cp.Name, cp.Price, cus.CustomerId, cus.UserName FROM Carts as c
                        JOIN CartProducts as cp ON c.Cartid = cp.CartId
                        JOIN Customers as cus ON c.CustomerId = cus.CustomerId
                        WHERE c.CartId = @Id";

            using (var connection = new SqlConnection(_configuration["CartsConnectionString"]))
            {
                var result = await connection.QueryAsync<Cart, CartProduct, Customer, Cart>(sql, (cart, cartProduct, customer) =>
                {

                    cart.CartProducts = cart.CartProducts ?? new List<CartProduct>();
                    cart.CartProducts.Add(cartProduct);
                    cart.Customer = cart.Customer ?? new Customer();
                    cart.Customer = customer;
                    return cart;
                }, param: new { Id = id}, splitOn: "CartProductId,CustomerId").ConfigureAwait(false);

                if (result == null || result.Count() == 0)
                {
                    var cartsWithNoProductsSql = @"SELECT c.CartId, cus.CustomerId, cus.UserName FROM Carts as c
                        JOIN Customers as cus ON c.CustomerId = cus.CustomerId
                        WHERE c.CartId = @Id";
                    result = await connection.QueryAsync<Cart, Customer, Cart>(cartsWithNoProductsSql, (cart, customer) =>
                    {
                        cart.Customer = cart.Customer ?? new Customer();
                        cart.Customer = customer;
                        return cart;
                    }, param: new { Id = id}, splitOn: "CustomerId").ConfigureAwait(false);
                    return result.ToList().FirstOrDefault();
                }

                bool skip = true;
                foreach(var entry in result)
                {
                    if(!skip)
                    {
                        result.FirstOrDefault().CartProducts.Add(entry.CartProducts.FirstOrDefault());
                    }
                    skip = false;
                }

                var finalResult = new List<Cart>()
                {
                    result.FirstOrDefault()
                };

                return finalResult.ToList().FirstOrDefault();
            }
        }
    }
}
