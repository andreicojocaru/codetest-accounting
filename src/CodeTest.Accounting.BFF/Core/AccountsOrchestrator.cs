using System.Threading.Tasks;
using CodeTest.Accounting.BFF.Models;
using CodeTest.Accounting.ServiceClients;
using Microsoft.Extensions.Logging;

namespace CodeTest.Accounting.BFF.Core
{
    public class AccountsOrchestrator
    {
        private readonly ILogger<AccountsOrchestrator> _logger;
        private readonly AccountsServiceClient _accountsServiceClient;
        private readonly CustomersServiceClient _customersServiceClient;
        private readonly TransactionsServiceClient _transactionsServiceClient;

        public AccountsOrchestrator(
            ILogger<AccountsOrchestrator> logger,
            AccountsServiceClient accountsServiceClient,
            CustomersServiceClient customersServiceClient,
            TransactionsServiceClient transactionsServiceClient)
        {
            _logger = logger;
            _logger = logger;
            _accountsServiceClient = accountsServiceClient;
            _customersServiceClient = customersServiceClient;
            _transactionsServiceClient = transactionsServiceClient;
        }

        public async Task OpenAccountAsync(OpenAccountDto input)
        {
            if (!await CheckCustomerValidity(input))
            {
                throw new CustomerNotValidException();
            }

            var account = await _accountsServiceClient.PostAsync(new AccountDto
            {
                CustomerId = input.CustomerId,
                InitialCredit = input.InitialCredit
            });

            // if the account balance is positive, create a new transaction
            if (input.InitialCredit.HasValue && input.InitialCredit.Value > 0)
            {
                await _transactionsServiceClient.PostAsync(new TransactionDto
                {
                    AccountId = account.Id,
                    Amount = input.InitialCredit.Value
                });
            }
        }

        private async Task<bool> CheckCustomerValidity(OpenAccountDto input)
        {
            try
            {
                var customer = await _customersServiceClient.GetAsync(input.CustomerId);

                // we can do more validity checks here

                if (customer == null)
                {
                    return false;
                }

                return true;
            }
            catch (CustomerApiException e)
            {
                _logger.LogError(e, "Error received from Consumers service");
                return false;
            }
        }
    }
}
