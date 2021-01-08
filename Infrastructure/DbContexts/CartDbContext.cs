using Domain.Contracts;
using Domain.Entities;
using Infrastructure.EntityConfigurations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.DbContexts
{
    public class CartDbContext : DbContext, IDbContext
    {
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<CartProduct> CartProducts { get; set; }

        public CartDbContext(DbContextOptions<CartDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CartEntityConfiguration());
            modelBuilder.ApplyConfiguration(new CartProductEntityConfiguration());
            modelBuilder.ApplyConfiguration(new CustomerEntityConfiguration());
        }
    }
}
