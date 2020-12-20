using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeTest.Accounting.BFF.Models;
using CodeTest.Accounting.ServiceClients;
using Microsoft.Extensions.Logging;

namespace CodeTest.Accounting.BFF.Core
{
    public class InformationOrchestrator
    {
        private readonly ILogger<InformationOrchestrator> _logger;
        private readonly AccountsServiceClient _accountsServiceClient;
        private readonly CustomersServiceClient _customersServiceClient;
        private readonly TransactionsServiceClient _transactionsServiceClient;

        public InformationOrchestrator(
            ILogger<InformationOrchestrator> logger,
            AccountsServiceClient accountsServiceClient,
            CustomersServiceClient customersServiceClient,
            TransactionsServiceClient transactionsServiceClient)
        {
            _logger = logger;
            _accountsServiceClient = accountsServiceClient;
            _customersServiceClient = customersServiceClient;
            _transactionsServiceClient = transactionsServiceClient;
        }

        public async Task<UserInfoViewModel> GetUserInfoAsync(int customerId)
        {
            var customer = await GetCustomerAsync(customerId);

            if (customer == null)
            {
                return null;
            }

            var accounts = await GetAccountsAsync(customerId);

            var accountIds = accounts.Select(a => a.Id).ToList();
            var transactions = await GetTransactionsAsync(accountIds);

            UserInfoViewModel model = new UserInfoViewModel
            {
                Name = customer.FirstName,
                Surname = customer.Surname,
                Accounts = accounts.Select(a => new UserAccountViewModel
                {
                    Balance = a.Balance,
                    AccountId = a.Id
                }).ToList(),
                Transactions = transactions.Select(t => new UserAccountTransactionViewModel
                {
                    Amount = t.Amount,
                    AccountId = t.AccountId,
                    TransactionId = t.Id
                }).ToList()
            };

            return model;
        }

        // note: these methods can be replaced bt Ambassadors
        // see: https://docs.microsoft.com/en-us/azure/architecture/patterns/ambassador
        private async Task<Customer> GetCustomerAsync(int customerId)
        {
            try
            {
                var customer = await _customersServiceClient.GetAsync(customerId);
                return customer;
            }
            catch (AccountApiException e)
            {
                _logger.LogError(e, "Customer service returned error.");
                return null;
            }
        }

        private async Task<IList<Account>> GetAccountsAsync(int customerId)
        {
            try
            {
                var accounts = await _accountsServiceClient.ListForCustomerAsync(customerId);
                return accounts.ToList();
            }
            catch (AccountApiException e)
            {
                _logger.LogError(e, "Accounts service returned error.");
                return Enumerable.Empty<Account>().ToList();
            }
        }

        private async Task<IList<Transaction>> GetTransactionsAsync(IList<int> accountIds)
        {
            if (accountIds == null || !accountIds.Any())
            {
                return Enumerable.Empty<Transaction>().ToList();
            }

            try
            {
                var transactions = await _transactionsServiceClient.ListForAccountsAsync(accountIds);
                return transactions.ToList();
            }
            catch (AccountApiException e)
            {
                _logger.LogError(e, "Transactions service returned error.");
                return Enumerable.Empty<Transaction>().ToList();
            }
        }
    }
}
