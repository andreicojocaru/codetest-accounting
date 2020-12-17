using System;
using Microsoft.AspNetCore.Mvc;

namespace CodeTest.Accounting.BFF.Controllers.API
{
    [ApiController]
    [Route("api/account")]
    public class AccountController : ControllerBase
    {
        [HttpPost]
        [Route("open-account")]
        public ActionResult OpenAccount(int customerId, uint? initialCredit)
        {
            // todo
            // find customer using Customer Service
            // create account using Accounts Service
            // if initialCredit > 0, create transaction using Transactions Service

            // confirm account creation (synchronous) to client

            throw new NotImplementedException();
        }
    }
}
