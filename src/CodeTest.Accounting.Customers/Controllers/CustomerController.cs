using System.Net;
using CodeTest.Accounting.Contracts;
using CodeTest.Accounting.Customers.Database;
using Microsoft.AspNetCore.Mvc;

namespace CodeTest.Accounting.Customers.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerDatabase _database;

        public CustomerController(ICustomerDatabase database)
        {
            _database = database;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Customer), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public ActionResult Get(int id)
        {
            var customer = _database.GetCustomer(id);

            if (customer == null)
            {
                return NoContent();
            }

            return Ok(customer);
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public ActionResult Post(Customer customer)
        {
            if (string.IsNullOrWhiteSpace(customer?.FirstName))
            {
                return BadRequest("Customer name is required.");
            }

            _database.CreateCustomer(customer.FirstName);

            return Accepted();
        }
    }
}
