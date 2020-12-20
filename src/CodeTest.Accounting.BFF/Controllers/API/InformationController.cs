using System.Threading.Tasks;
using CodeTest.Accounting.BFF.Core;
using Microsoft.AspNetCore.Mvc;

namespace CodeTest.Accounting.BFF.Controllers.API
{
    [ApiController]
    [Route("api/information")]
    public class InformationController : ControllerBase
    {
        private readonly InformationOrchestrator _informationOrchestrator;

        public InformationController(InformationOrchestrator informationOrchestrator)
        {
            _informationOrchestrator = informationOrchestrator;
        }

        [HttpGet]
        [Route("user/{customerId}")]
        public async Task<ActionResult> GetUserInfo(int customerId)
        {
            if (customerId == default)
            {
                return BadRequest();
            }

            var model = await _informationOrchestrator.GetUserInfoAsync(customerId);

            return Ok(model);
        }
    }
}
