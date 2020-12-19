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

            if (!await CheckCustomerValidity(input))
            {
                return BadRequest("Customer not valid!");
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

            return Ok();
        }

        private async Task<bool> CheckCustomerValidity(OpenAccountDto input)
        {
            try
            {
                var customer = await _customersServiceClient.GetAsync(input.CustomerId);

                // we can do more validity checks here

                if (customer == null)
                {
                    return false;
                }

                return true;
            }
            catch (CustomerApiException e)
            {
                return false;
            }
        }
    }
}
