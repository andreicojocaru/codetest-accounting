using System.Threading.Tasks;
using CodeTest.Accounting.BFF.Core;
using CodeTest.Accounting.BFF.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CodeTest.Accounting.BFF.Controllers.API
{
    [ApiController]
    [Route("api/account")]
    public class AccountController : ControllerBase
    {
        private readonly ILogger<AccountController> _logger;
        private readonly AccountsOrchestrator _accountsOrchestrator;

        public AccountController(
            ILogger<AccountController> logger,
            AccountsOrchestrator accountsOrchestrator)
        {
            _logger = logger;
            _accountsOrchestrator = accountsOrchestrator;
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

            try
            {
                await _accountsOrchestrator.OpenAccountAsync(input);
            }
            catch (CustomerNotValidException e)
            {
                const string message = "Customer not valid!";

                _logger.LogError(e, message);
                return BadRequest(message);
            }

            return Ok();
        }
    }
}
