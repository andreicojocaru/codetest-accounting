using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using CodeTest.Accounting.Contracts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CodeTest.Accounting.ServiceClients
{
    public class AccountsServiceClient
    {
        private readonly ILogger<AccountsServiceClient> _logger;
        private readonly IOptions<ServiceUrls> _serviceUrlOptions;
        private readonly IHttpClientFactory _clientFactory;

        public AccountsServiceClient(
            ILogger<AccountsServiceClient> logger,
            IOptions<ServiceUrls> serviceUrlOptions,
            IHttpClientFactory clientFactory)
        {
            _logger = logger;
            _serviceUrlOptions = serviceUrlOptions;
            _clientFactory = clientFactory;
        }

        public async Task<Account> CreateAccountAsync(int accountId, int customerId, decimal balance)
        {
            throw new NotImplementedException();
            //var uri = new Uri($"{_serviceUrlOptions.Value.Accounts}/api/account");

            //try
            //{
            //    using var client = _clientFactory.CreateClient();
            //    var response = await client.GetStreamAsync(uri);
            //    var customer = await JsonSerializer.DeserializeAsync<CustomerDto>(response);

            //    return customer;
            //}
            //catch (Exception e)
            //{
            //    _logger.LogError(e, "Error from Customer Service.");
            //    return null;
            //}
        }
    }
}
