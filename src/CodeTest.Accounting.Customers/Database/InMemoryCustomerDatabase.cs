using System;
using CodeTest.Accounting.Contracts;
using CodeTest.Accounting.Contracts.Exceptions;
using Microsoft.Extensions.Caching.Memory;

namespace CodeTest.Accounting.Customers.Database
{
    public class InMemoryCustomerDatabase : ICustomerDatabase
    {
        private const string CacheKeyFormat = "customer-{0}";

        private readonly IMemoryCache _memoryCache;

        public InMemoryCustomerDatabase(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public Customer GetCustomer(int id)
        {
            var cacheKey = string.Format(CacheKeyFormat, id);

            if (_memoryCache.TryGetValue(cacheKey, out var customer))
            {
                if (customer is Customer validCustomer)
                {
                    return validCustomer;
                }

                // invalid cache entry, not Customer object
                _memoryCache.Remove(cacheKey);
            }

            throw new CustomerNotFoundException(id);
        }

        public void CreateCustomer(Customer customer)
        {
            var cacheKey = string.Format(CacheKeyFormat, customer.Id);

            if (_memoryCache.TryGetValue(cacheKey, out _))
            {
                throw new CustomerAlreadyExistsException(customer.Id);
            }

            _memoryCache.Set(cacheKey, customer);
        }

    }
}
