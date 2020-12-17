using System;

namespace CodeTest.Accounting.Contracts.Exceptions
{
    public class CustomerAlreadyExistsException : Exception
    {
        public CustomerAlreadyExistsException(int customerId) : base($"Customer {customerId} already exists.")
        {
        }
    }
}
