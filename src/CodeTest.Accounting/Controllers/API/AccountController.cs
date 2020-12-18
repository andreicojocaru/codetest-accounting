using System;
using CodeTest.Accounting.ServiceClients;
using Microsoft.AspNetCore.Mvc;

namespace CodeTest.Accounting.BFF.Controllers.API
{
    [ApiController]
    [Route("api/account")]
    public class AccountController : ControllerBase
    {
        private readonly AccountsServiceClient _accountsServiceClient;

        public AccountController(AccountsServiceClient accountsServiceClient)
        {
            _accountsServiceClient = accountsServiceClient;
        }

        [HttpPost]
        [Route("open-account")]
        public ActionResult OpenAccount(int customerId, uint? initialCredit)
        {
            
            throw new NotImplementedException();
        }
    }
}
