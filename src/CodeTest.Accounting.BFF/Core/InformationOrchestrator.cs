using System.Linq;
using System.Threading.Tasks;
using CodeTest.Accounting.BFF.Models;
using CodeTest.Accounting.ServiceClients;

namespace CodeTest.Accounting.BFF.Core
{
    public class InformationOrchestrator
    {
        private readonly AccountsServiceClient _accountsServiceClient;
        private readonly CustomersServiceClient _customersServiceClient;
        private readonly TransactionsServiceClient _transactionsServiceClient;

        public InformationOrchestrator(
            AccountsServiceClient accountsServiceClient,
            CustomersServiceClient customersServiceClient,
            TransactionsServiceClient transactionsServiceClient)
        {
            _accountsServiceClient = accountsServiceClient;
            _customersServiceClient = customersServiceClient;
            _transactionsServiceClient = transactionsServiceClient;
        }

        public async Task<UserInfoViewModel> GetUserInfoAsync(int customerId)
        {
            // note: these service requests can be made in parallel
            // it then adds an overhead of unpacking the right task response
            // for the purpose of this small project, I just make sequential requests
            var customer = await _customersServiceClient.GetAsync(customerId);
            var accounts = await _accountsServiceClient.ListForCustomerAsync(customerId);

            var accountIds = accounts.Select(a => a.Id).ToList();
            var transactions = await _transactionsServiceClient.ListForAccountsAsync(accountIds);

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
    }
}
