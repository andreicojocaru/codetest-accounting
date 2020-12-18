using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CodeTest.Accounting.Persistence;
using CodeTest.Accounting.ServiceClients;
using Microsoft.AspNetCore.Mvc;
using Account = CodeTest.Accounting.Domain.Account;
using AccountDto = CodeTest.Accounting.Accounts.Models.AccountDto;

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
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public ActionResult Get(int id)
        {
            var account = _accountRepository.Get(id);

            if (account != null)
            {
                return Ok(account);
            }

            return BadRequest($"Account not found for id: {id}");
        }

        [HttpGet]
        [Route("list-for-customer")]
        [ProducesResponseType(typeof(IList<Account>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public ActionResult ListForCustomer(int customerId)
        {
            var accounts = _accountRepository.ListAll().Where(a => a.CustomerId == customerId);

            if (accounts.Any())
            {
                return Ok(accounts);
            }

            return NoContent();
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> Post([FromBody] AccountDto account)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // first, check for Consumer account using the Consumer Service
            var customer = await _customersServiceClient.GetAsync(account.CustomerId);

            if (customer == null)
            {
                return BadRequest($"Customer {account.CustomerId} not found!");
            }

            var id = _accountRepository.Set(new Account
            {
                Balance = account.InitialCredit ?? 0,
                CustomerId = account.CustomerId
            });

            // if the account balance is positive, create a new transaction
            if (account.InitialCredit.HasValue && account.InitialCredit.Value > 0)
            {
                await _transactionsServiceClient.PostAsync(id, account.InitialCredit.Value);
            }

            return CreatedAtAction(nameof(Get), new { id }, id);
        }
    }
}
