using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CodeTest.Accounting.Persistence;
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

        public AccountController(IRepository<Account> accountRepository)
        {
            _accountRepository = accountRepository;
        }

        [HttpGet]
        [ProducesResponseType(typeof(Account), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<Account>> Get(int id)
        {
            var account = await _accountRepository.GetAsync(id);

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
        public async Task<ActionResult<IList<Account>>> ListForCustomer(int customerId)
        {
            var allAccounts = await _accountRepository.ListAllAsync();
            var accounts = allAccounts?.Where(a => a.CustomerId == customerId);

            if (accounts != null && accounts.Any())
            {
                return Ok(accounts);
            }

            return NoContent();
        }

        [HttpPost]
        [ProducesResponseType(typeof(Account), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<Account>> Post([FromBody] AccountDto account)
        {
            if (!ModelState.IsValid || account.CustomerId == default)
            {
                return BadRequest(ModelState);
            }

            var model = new Account
            {
                Balance = account.InitialCredit ?? 0,
                CustomerId = account.CustomerId
            };

            var id = await _accountRepository.SetAsync(model);

            // prepare the object for response
            model.Id = id;

            return Ok(model);
        }
    }
}
