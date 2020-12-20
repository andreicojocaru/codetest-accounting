using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeTest.Accounting.Domain;
using CodeTest.Accounting.Persistence;
using CodeTest.Accounting.Transactions.Controllers;
using CodeTest.Accounting.Transactions.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace CodeTest.Accounting.Transactions.Tests
{
    public class TransactionsControllerTests
    {
        private Mock<IRepository<Transaction>> _repositoryMock;
        private TransactionController _sut;

        [SetUp]
        public void Setup()
        {
            _repositoryMock = new Mock<IRepository<Transaction>>();
            _sut = new TransactionController(_repositoryMock.Object);
        }

        [Test]
        public async Task Get_WithExistingTransaction_ShouldReturnOkWithTransaction()
        {
            // Arrange
            var transactionId = 1;
            var transaction = new Transaction
            {
                Id = transactionId,
                AccountId = 1,
                Amount = 100
            };

            _repositoryMock.Setup(r => r.GetAsync(transactionId))
                .ReturnsAsync(transaction);

            // Act
            var actual = await _sut.Get(transactionId);

            // Assert
            Assert.NotNull(actual?.Result);

            var okResult = actual.Result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(transaction, okResult.Value);
        }

        [Test]
        public async Task Get_WithNonExistingTransaction_ShouldReturnBadRequest()
        {
            // Arrange
            var transactionId = 1;

            // Act
            var actual = await _sut.Get(transactionId);

            // Assert
            Assert.NotNull(actual?.Result);

            var statusResult = actual.Result as BadRequestObjectResult;
            Assert.NotNull(statusResult);
        }

        [Test]
        public async Task ListForCustomer_WithExistingTransactions_ShouldReturnOkWithTransactions()
        {
            // Arrange
            var accountId = 1;
            var transactions = new List<Transaction>
            {
                new Transaction {
                    Id = 1,
                    AccountId = accountId,
                    Amount = 100
                },
                new Transaction {
                    Id = 2,
                    AccountId = accountId,
                    Amount = 50
                },
                new Transaction {
                    Id = 2,
                    AccountId = 100,
                    Amount = 10
                }
            };

            _repositoryMock.Setup(r => r.ListAllAsync())
                .ReturnsAsync(transactions);

            // Act
            var actual = await _sut.ListForAccounts(new List<int>() { accountId });

            // Assert
            Assert.NotNull(actual?.Result);

            var okResult = actual.Result as OkObjectResult;
            Assert.NotNull(okResult);

            var listResult = okResult.Value as IEnumerable<Transaction>;
            Assert.NotNull(listResult);

            Assert.AreEqual(2, listResult.Count());
        }

        [Test]
        public async Task ListForAccounts_WithNoTransactions_ShouldReturnNoContent()
        {
            // Arrange
            var accountId = 1;

            // Act
            var actual = await _sut.ListForAccounts(new List<int>() { accountId });

            // Assert
            Assert.NotNull(actual?.Result);

            var result = actual.Result as NoContentResult;
            Assert.NotNull(result);
        }

        [Test]
        public async Task Post_WithValidTransaction_ShouldReturnOkWithNewTransaction()
        {
            // Arrange
            var transaction = new TransactionDto
            {
                AccountId = 1,
                Amount = 100
            };

            // Act
            var actual = await _sut.Post(transaction);

            // Assert
            Assert.NotNull(actual);

            var result = actual as CreatedAtActionResult;
            Assert.NotNull(result);
        }

        [Test]
        public async Task Post_WithInvalidTransactionDto_ShouldReturnBadRequest()
        {
            // Arrange
            var transaction = new TransactionDto();

            // Act
            var actual = await _sut.Post(transaction);

            // Assert
            Assert.NotNull(actual);
            Assert.True(actual is BadRequestObjectResult);
        }
    }
}