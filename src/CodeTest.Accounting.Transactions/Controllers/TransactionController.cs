using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CodeTest.Accounting.Domain;
using CodeTest.Accounting.Persistence;
using CodeTest.Accounting.Transactions.Models;
using Microsoft.AspNetCore.Mvc;

namespace CodeTest.Accounting.Transactions.Controllers
{
    [ApiController]
    [Route("api/transaction")]
    public class TransactionController : ControllerBase
    {
        private readonly IRepository<Transaction> _transactionsRepository;

        public TransactionController(IRepository<Transaction> transactionsRepository)
        {
            _transactionsRepository = transactionsRepository;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Transaction), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<ActionResult<Transaction>> Get(int id)
        {
            var transaction = await _transactionsRepository.GetAsync(id);

            if (transaction != null)
            {
                return Ok(transaction);
            }

            return BadRequest($"Transaction not found for id: {id}");
        }

        [HttpGet]
        [Route("list-for-accounts")]
        [ProducesResponseType(typeof(IList<Transaction>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<ActionResult<IList<Transaction>>> ListForAccounts(IList<int> accountIds)
        {
            var allTransactions = await _transactionsRepository.ListAllAsync();
            var transactions = allTransactions?.Where(t => accountIds.Contains(t.AccountId));

            if (transactions != null && transactions.Any())
            {
                return Ok(transactions);
            }

            return NoContent();
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> Post([FromBody] TransactionDto input)
        {
            if (!ModelState.IsValid || input.AccountId == default)
            {
                return BadRequest(ModelState);
            }

            var id = await _transactionsRepository.SetAsync(new Transaction
            {
                AccountId = input.AccountId,
                Amount = input.Amount
            });

            // todo (out-of-scope): we should notify the Accounts Service to update Account Balance for the new Transaction

            return CreatedAtAction(nameof(Get), new { id }, id);
        }
    }
}
