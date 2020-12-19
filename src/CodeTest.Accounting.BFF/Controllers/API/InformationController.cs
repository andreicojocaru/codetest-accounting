using System.Linq;
using System.Threading.Tasks;
using CodeTest.Accounting.BFF.Models;
using CodeTest.Accounting.ServiceClients;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CodeTest.Accounting.BFF.Controllers.API
{
    [ApiController]
    [Route("api/information")]
    public class InformationController : ControllerBase
    {
        private readonly ILogger<InformationController> _logger;
        private readonly AccountsServiceClient _accountsServiceClient;
        private readonly CustomersServiceClient _customersServiceClient;
        private readonly TransactionsServiceClient _transactionsServiceClient;

        public InformationController(
            ILogger<InformationController> logger,
            AccountsServiceClient accountsServiceClient,
            CustomersServiceClient customersServiceClient,
            TransactionsServiceClient transactionsServiceClient)
        {
            _logger = logger;
            _accountsServiceClient = accountsServiceClient;
            _customersServiceClient = customersServiceClient;
            _transactionsServiceClient = transactionsServiceClient;
        }

        [HttpGet]
        [Route("user/{customerId}")]
        public async Task<ActionResult> GetUserInfo(int customerId)
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

            return Ok(model);
        }
    }
}
