using System.Net;
using CodeTest.Accounting.Contracts;
using CodeTest.Accounting.Customers.Models;
using CodeTest.Accounting.Persistence;
using Microsoft.AspNetCore.Mvc;

namespace CodeTest.Accounting.Customers.Controllers
{
    [ApiController]
    [Route("api/customer")]
    public class CustomerController : ControllerBase
    {
        private readonly IRepository<Customer> _customerRepository;

        public CustomerController(IRepository<Customer> customerRepository)
        {
            _customerRepository = customerRepository;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Customer), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public ActionResult Get(int id)
        {
            var customer = _customerRepository.Get(id);

            if (customer != null)
            {
                return Ok(customer);
            }

            return NoContent();
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public ActionResult Post(CustomerDto customer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var id = _customerRepository.Set(new Customer
            {
                FirstName = customer.FirstName,
                Surname = customer.Surname
            });

            return CreatedAtAction(nameof(Get), new { id }, id);
        }
    }
}
