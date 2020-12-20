using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeTest.Accounting.Accounts.Controllers;
using CodeTest.Accounting.Accounts.Models;
using CodeTest.Accounting.Domain;
using CodeTest.Accounting.Persistence;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace CodeTest.Accounting.Accounts.Tests
{
    public class AccountControllerTests
    {
        private Mock<IRepository<Account>> _repositoryMock;
        private AccountController _sut;

        [SetUp]
        public void Setup()
        {
            _repositoryMock = new Mock<IRepository<Account>>();
            _sut = new AccountController(_repositoryMock.Object);
        }

        [Test]
        public async Task Get_WithExistingAccount_ShouldReturnOkWithAccount()
        {
            // Arrange
            var accountId = 1;
            var account = new Account
            {
                Id = accountId,
                Balance = 100,
                CustomerId = 1
            };

            _repositoryMock.Setup(r => r.GetAsync(accountId))
                .ReturnsAsync(account);

            // Act
            var actual = await _sut.Get(accountId);

            // Assert
            Assert.NotNull(actual?.Result);

            var okResult = actual.Result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(account, okResult.Value);
        }

        [Test]
        public async Task Get_WithNonExistingAccount_ShouldReturnBadRequest()
        {
            // Arrange
            var accountId = 1;

            // Act
            var actual = await _sut.Get(accountId);

            // Assert
            Assert.NotNull(actual?.Result);

            var statusResult = actual.Result as BadRequestObjectResult;
            Assert.NotNull(statusResult);
        }

        [Test]
        public async Task ListForCustomer_WithExistingAccounts_ShouldReturnOkWithAccounts()
        {
            // Arrange
            var customerId = 1;
            var accounts = new List<Account>
            {
                new Account {
                    Id = 1,
                    Balance = 100,
                    CustomerId = customerId
                },
                new Account {
                    Id = 2,
                    Balance = 100,
                    CustomerId = customerId
                },
                new Account {
                    Id = 2,
                    Balance = 100,
                    CustomerId = 3
                }
            };

            _repositoryMock.Setup(r => r.ListAllAsync())
                .ReturnsAsync(accounts);

            // Act
            var actual = await _sut.ListForCustomer(customerId);

            // Assert
            Assert.NotNull(actual?.Result);

            var okResult = actual.Result as OkObjectResult;
            Assert.NotNull(okResult);

            var listResult = okResult.Value as IEnumerable<Account>;
            Assert.NotNull(listResult);

            Assert.AreEqual(2, listResult.Count());
        }

        [Test]
        public async Task ListForCustomer_WithNoAccounts_ShouldReturnNoContent()
        {
            // Arrange
            var customerId = 1;

            // Act
            var actual = await _sut.ListForCustomer(customerId);

            // Assert
            Assert.NotNull(actual?.Result);

            var result = actual.Result as NoContentResult;
            Assert.NotNull(result);
        }

        [Test]
        public async Task Post_WithValidAccount_ShouldReturnOkWithNewAccount()
        {
            // Arrange
            var account = new AccountDto
            {
                CustomerId = 1,
                InitialCredit = 100
            };

            // Act
            var actual = await _sut.Post(account);

            // Assert
            Assert.NotNull(actual?.Result);

            var result = actual.Result as OkObjectResult;
            Assert.NotNull(result);

            var accountResult = result.Value as Account;
            Assert.NotNull(accountResult);

            Assert.AreEqual(0, accountResult.Id);
            Assert.AreEqual(account.CustomerId, accountResult.CustomerId);
            Assert.AreEqual(account.InitialCredit, accountResult.Balance);
        }

        [Test]
        public async Task Post_WithValidAccount_ShouldReturnOkWithNewAccountAndId()
        {
            // Arrange
            var accountId = 5;
            var account = new AccountDto
            {
                CustomerId = 1,
                InitialCredit = 100
            };

            _repositoryMock.Setup(r => r.SetAsync(It.IsAny<Account>()))
                .ReturnsAsync(accountId);

            // Act
            var actual = await _sut.Post(account);

            // Assert
            Assert.NotNull(actual?.Result);

            var result = actual.Result as OkObjectResult;
            Assert.NotNull(result);

            var accountResult = result.Value as Account;
            Assert.NotNull(accountResult);

            Assert.AreEqual(accountId, accountResult.Id);
            Assert.AreEqual(account.CustomerId, accountResult.CustomerId);
            Assert.AreEqual(account.InitialCredit, accountResult.Balance);
        }

        [Test]
        public async Task Post_WithInvalidAccountDto_ShouldReturnBadRequest()
        {
            // Arrange
            var account = new AccountDto();

            // Act
            var actual = await _sut.Post(account);

            // Assert
            Assert.NotNull(actual?.Result);
            Assert.True(actual.Result is BadRequestObjectResult);
        }
    }
}