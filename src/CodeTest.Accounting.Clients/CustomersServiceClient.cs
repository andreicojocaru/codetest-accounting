using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using CodeTest.Accounting.Contracts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CodeTest.Accounting.ServiceClients
{
    public class CustomersServiceClient
    {
        private readonly ILogger<CustomersServiceClient> _logger;
        private readonly IOptions<ServiceUrls> _serviceUrlOptions;
        private readonly IHttpClientFactory _clientFactory;

        public CustomersServiceClient(
            ILogger<CustomersServiceClient> logger,
            IOptions<ServiceUrls> serviceUrlOptions,
            IHttpClientFactory clientFactory)
        {
            _logger = logger;
            _serviceUrlOptions = serviceUrlOptions;
            _clientFactory = clientFactory;
        }

        public async Task<Customer> GetCustomerAsync(int id)
        {
            var uri = new Uri($"{_serviceUrlOptions.Value.Customers}/api/customer/{id}");

            try
            {
                using var client = _clientFactory.CreateClient();
                var response = await client.GetStreamAsync(uri);
                var customer = await JsonSerializer.DeserializeAsync<Customer>(response);

                return customer;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error from Customer Service.");
                return null;
            }
        }
    }
}
