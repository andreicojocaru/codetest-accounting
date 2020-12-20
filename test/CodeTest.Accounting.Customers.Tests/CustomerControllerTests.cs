using System.Threading.Tasks;
using CodeTest.Accounting.Customers.Controllers;
using CodeTest.Accounting.Customers.Models;
using CodeTest.Accounting.Domain;
using CodeTest.Accounting.Persistence;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace CodeTest.Accounting.Customers.Tests
{
    public class CustomerControllerTests
    {
        private Mock<IRepository<Customer>> _repositoryMock;
        private CustomerController _sut;

        [SetUp]
        public void Setup()
        {
            _repositoryMock = new Mock<IRepository<Customer>>();
            _sut = new CustomerController(_repositoryMock.Object);
        }

        [Test]
        public async Task Get_WithExistingCustomer_ShouldReturnOkWithCustomer()
        {
            // Arrange
            var customerId = 1;
            var customer = new Customer
            {
                Id = customerId,
                FirstName = "Customer",
                Surname = "No_1"
            };

            _repositoryMock.Setup(r => r.GetAsync(customerId))
                .ReturnsAsync(customer);

            // Act
            var actual = await _sut.Get(customerId);

            // Assert
            Assert.NotNull(actual?.Result);

            var okResult = actual.Result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(customer, okResult.Value);
        }

        [Test]
        public async Task Get_WithNonExistingCustomer_ShouldReturnBadRequest()
        {
            // Arrange
            var CustomerId = 1;

            // Act
            var actual = await _sut.Get(CustomerId);

            // Assert
            Assert.NotNull(actual?.Result);

            var statusResult = actual.Result as NoContentResult;
            Assert.NotNull(statusResult);
        }

        [Test]
        public async Task Post_WithValidCustomer_ShouldReturnOkWithNewCustomer()
        {
            // Arrange
            var customer = new CustomerDto
            {
                Surname = "No_1",
                FirstName = "Customer"
            };

            // Act
            var actual = await _sut.Post(customer);

            // Assert
            Assert.NotNull(actual);
            Assert.True(actual is CreatedAtActionResult);
        }
    }
}