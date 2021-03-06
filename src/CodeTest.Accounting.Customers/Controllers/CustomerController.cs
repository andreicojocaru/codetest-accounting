﻿using System.Net;
using System.Threading.Tasks;
using CodeTest.Accounting.Customers.Models;
using CodeTest.Accounting.Domain;
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
        public async Task<ActionResult<Customer>> Get(int id)
        {
            var customer = await _customerRepository.GetAsync(id);

            if (customer != null)
            {
                return Ok(customer);
            }

            return BadRequest($"Customer not found for id: {id}");
        }

        [HttpPost]
        [ProducesResponseType(typeof(Customer), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<Customer>> Post([FromBody] CustomerDto customer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var model = new Customer
            {
                FirstName = customer.FirstName,
                Surname = customer.Surname
            };

            var id = await _customerRepository.SetAsync(model);
            model.Id = id;

            return Ok(model);
        }
    }
}
