using System;
using Microsoft.AspNetCore.Mvc;

namespace CodeTest.Accounting.BFF.Controllers.API
{
    [ApiController]
    [Route("api/information")]
    public class InformationController
    {
        [HttpGet]
        [Route("user")]
        public ActionResult GetUserInfo(int customerId)
        {
            // todo
            // get name and surname from Customer Service
            // get balance from Account Service
            // get transactions from the Transactions Service

            throw new NotImplementedException();
        }
    }
}
