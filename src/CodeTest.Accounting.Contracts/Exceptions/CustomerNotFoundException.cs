using System;

namespace CodeTest.Accounting.Contracts.Exceptions
{
    public class CustomerNotFoundException : Exception
    {
        public CustomerNotFoundException(int customerId) : base($"Customer {customerId} not found.")
        {
        }
    }
}
