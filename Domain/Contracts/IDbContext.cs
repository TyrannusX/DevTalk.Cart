using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Domain.Contracts
{
    public interface IDbContext
    {
        DbSet<Cart> Carts { get; set; }
        DbSet<Customer> Customers { get; set; }
        DbSet<CartProduct> CartProducts { get; set; }
        DatabaseFacade Database { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    }
}
