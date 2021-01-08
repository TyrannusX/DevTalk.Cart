using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Customer
    {
        public Guid CustomerId { get; private set; }
        public string UserName { get; private set; }

        public Customer()
        {

        }

        public Customer(string userName)
        {
            UserName = userName;
        }
    }
}
