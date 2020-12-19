using System;
using System.Threading.Tasks;
using CodeTest.Accounting.BFF.Models;
using CodeTest.Accounting.ServiceClients;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CodeTest.Accounting.BFF.Controllers.API
{
    [ApiController]
    [Route("api/account")]
    public class AccountController : ControllerBase
    {
        private readonly AccountsServiceClient _accountsServiceClient;
        private readonly CustomersServiceClient _customersServiceClient;
        private readonly TransactionsServiceClient _transactionsServiceClient;

        public AccountController(
            AccountsServiceClient accountsServiceClient,
            CustomersServiceClient customersServiceClient,
            TransactionsServiceClient transactionsServiceClient)
        {
            _accountsServiceClient = accountsServiceClient;
            _customersServiceClient = customersServiceClient;
            _transactionsServiceClient = transactionsServiceClient;
        }

        [HttpPost]
        [Route("open-account")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> OpenAccount([FromBody] OpenAccountDto input)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // first, check for Consumer account using the Consumer Service
            var customer = await _customersServiceClient.GetAsync(input.CustomerId);

            if (customer == null)
            {
                return BadRequest($"Customer {input.CustomerId} not found!");
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

            throw new NotImplementedException();
        }
    }
}
