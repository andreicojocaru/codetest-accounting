using System;
using System.Net;
using CodeTest.Accounting.Contracts;
using CodeTest.Accounting.Contracts.Exceptions;
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
            try
            {
                var customer = _database.GetCustomer(id);
                return Ok(customer);
            }
            catch (CustomerNotFoundException e)
            {
                return BadRequest(e);
            }
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public ActionResult Post(Customer customer)
        {
            // note: we could use ModelValidator from ASP, or custom validation library
            // since the example is very small I prefer a manual check only
            if (string.IsNullOrWhiteSpace(customer?.FirstName))
            {
                return BadRequest("Customer name is required.");
            }

            if (string.IsNullOrWhiteSpace(customer.Surname))
            {
                return BadRequest("Customer surname is required");
            }

            try
            {
                _database.CreateCustomer(customer);
            }
            catch (CustomerAlreadyExistsException exception)
            {
                return BadRequest(exception);
            }

            return Accepted();
        }
    }
}
