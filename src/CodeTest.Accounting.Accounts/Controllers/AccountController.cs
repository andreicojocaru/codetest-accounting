using System.ComponentModel.DataAnnotations;
using System.Net;
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
        [ProducesResponseType(typeof(Account), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
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
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> Post([Required] int customerId, decimal? initialCredit)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // first, check for Consumer account using the Consumer Service
            var customer = await _customersServiceClient.GetCustomerAsync(customerId);

            if (customer == null)
            {
                return BadRequest($"Customer {customerId} not found!");
            }

            var id = _accountRepository.Set(new Account
            {
                Balance = initialCredit ?? 0,
                CustomerId = customerId
            });

            // if the account balance is positive, create a new transaction
            if (initialCredit.HasValue && initialCredit.Value > 0)
            {
                await _transactionsServiceClient.CreateNewTransaction(id, initialCredit.Value);
            }

            return CreatedAtAction(nameof(Get), new { id });
        }
    }
}
