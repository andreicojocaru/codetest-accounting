using System.Threading.Tasks;
using CodeTest.Accounting.Contracts;
using CodeTest.Accounting.Persistence;
using CodeTest.Accounting.ServiceClients;
using Microsoft.AspNetCore.Mvc;

namespace CodeTest.Accounting.Accounts.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IRepository<Account> _accountRepository;
        private readonly CustomersServiceClient _customersServiceClient;
        private readonly TransactionsServiceClient _transactionsServiceClient;

        public AccountController(
            IRepository<Account> accountRepository,
            CustomersServiceClient customersServiceClient,
            TransactionsServiceClient transactionsServiceClient)
        {
            _customersServiceClient = customersServiceClient;
            _transactionsServiceClient = transactionsServiceClient;
            _accountRepository = accountRepository;
        }

        [HttpGet]
        public ActionResult Get(int id)
        {
            var account = _accountRepository.Get(id);

            if (account != null)
            {
                return Ok(account);
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult> Post(Account account)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // first, check for Consumer account using the Consumer Service
            var customer = await _customersServiceClient.GetCustomerAsync(account.CustomerId);

            if (customer == null)
            {
                return BadRequest($"Customer {account.CustomerId} not found!");
            }

            _accountRepository.Set(account.Id, account);

            // if the account balance is positive, create a new transaction
            if (account.Balance > 0)
            {
                await _transactionsServiceClient.CreateNewTransaction(account.Id, account.Balance);
            }

            return CreatedAtAction(nameof(Get), new { id = account.Id });
        }
    }
}
