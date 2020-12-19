using System.Collections.Generic;
using System.Linq;
using System.Net;
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
        public ActionResult Get(int id)
        {
            var transaction = _transactionsRepository.Get(id);

            if (transaction != null)
            {
                return Ok(transaction);
            }

            return BadRequest($"Transaction not found for id: {id}");
        }

        [HttpGet]
        [Route("list-for-account")]
        [ProducesResponseType(typeof(IList<Transaction>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public ActionResult ListForCustomer(int accountId)
        {
            var transactions = _transactionsRepository.ListAll().Where(a => a.AccountId == accountId);

            if (transactions.Any())
            {
                return Ok(transactions);
            }

            return NoContent();
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public ActionResult Post([FromBody] TransactionDto input)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var id = _transactionsRepository.Set(new Transaction
            {
                AccountId = input.AccountId,
                Amount = input.Amount
            });

            // todo (out-of-scope): we should notify the Accounts Service to update Account Balance for the new Transaction

            return CreatedAtAction(nameof(Get), new { id });
        }
    }
}
