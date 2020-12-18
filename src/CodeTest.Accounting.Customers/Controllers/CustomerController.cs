using System.ComponentModel.DataAnnotations;
using System.Net;
using CodeTest.Accounting.Contracts;
using CodeTest.Accounting.Persistence;
using Microsoft.AspNetCore.Mvc;

namespace CodeTest.Accounting.Customers.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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

            return BadRequest($"Customer not found for id: {id}");
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public ActionResult Post([Required] string firstName, [Required] string surname)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var id = _customerRepository.Set(new Customer
            {
                FirstName = firstName,
                Surname = surname
            });

            return CreatedAtAction(nameof(Get), new { id });
        }
    }
}
