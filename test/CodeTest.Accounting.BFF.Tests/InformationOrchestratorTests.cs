using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeTest.Accounting.BFF.Core;
using CodeTest.Accounting.BFF.Models;
using CodeTest.Accounting.BFF.Tests.Fixture;
using CodeTest.Accounting.ServiceClients;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace CodeTest.Accounting.BFF.Tests
{
    public class InformationOrchestratorTests
    {
        private Mock<AccountsServiceClient> _accountsServiceClientMock;
        private Mock<CustomersServiceClient> _customersServiceClientMock;
        private Mock<TransactionsServiceClient> _transactionsServiceClientMock;

        private InformationOrchestrator _sut;

        [SetUp]
        public void Setup()
        {
            _accountsServiceClientMock = new Mock<AccountsServiceClient>(() => new AccountsServiceClient(new MockHttpClient()));
            _customersServiceClientMock = new Mock<CustomersServiceClient>(() => new CustomersServiceClient(new MockHttpClient()));
            _transactionsServiceClientMock = new Mock<TransactionsServiceClient>(() => new TransactionsServiceClient(new MockHttpClient()));

            var loggerMock = new Mock<ILogger<InformationOrchestrator>>();

            _sut = new InformationOrchestrator(
                loggerMock.Object,
                _accountsServiceClientMock.Object,
                _customersServiceClientMock.Object,
                _transactionsServiceClientMock.Object);
        }

        [Test]
        public async Task GetUserInfoAsync_WithValidCustomer_ShouldCallAllServices()
        {
            // Arrange
            var customerId = 1;

            var expectedCustomer = new Customer
            {
                FirstName = "Customer",
                Id = 1,
                Surname = "No_1"
            };

            var expectedAccounts = new List<Account>
            {
                new Account
                {
                    Id = 1,
                    Balance = 100,
                    CustomerId = 1
                }
            };

            var expectedAccountsIds = expectedAccounts.Select(a => a.Id);

            var expectedTransactions = new List<Transaction>
            {
                new Transaction
                {
                    Id = 1,
                    AccountId = 1,
                    Amount = 100
                }
            };

            _customersServiceClientMock.Setup(c => c.GetAsync(customerId))
                .ReturnsAsync(expectedCustomer);

            _accountsServiceClientMock.Setup(a => a.ListForCustomerAsync(customerId))
                .ReturnsAsync(expectedAccounts);

            _transactionsServiceClientMock.Setup(t => t.ListForAccountsAsync(expectedAccountsIds))
                .ReturnsAsync(expectedTransactions);

            // Act
            var actual = await _sut.GetUserInfoAsync(customerId);

            // Assert
            _customersServiceClientMock.Verify(c => c.GetAsync(customerId), Times.Once);
            _accountsServiceClientMock.Verify(a => a.ListForCustomerAsync(customerId), Times.Once);
            _transactionsServiceClientMock.Verify(a => a.ListForAccountsAsync(expectedAccountsIds), Times.Once);

            Assert.NotNull(actual);
            Assert.AreEqual(expectedCustomer.Surname, actual.Surname);
            Assert.AreEqual(expectedCustomer.FirstName, actual.Name);
            Assert.AreEqual(expectedAccounts.Count, actual.Accounts.Count);
            Assert.AreEqual(expectedTransactions.Count, actual.Transactions.Count);

            for (var i = 0; i < expectedAccounts.Count; i++)
            {
                Assert.AreEqual(expectedAccounts[i].Id, actual.Accounts[i].AccountId);
                Assert.AreEqual(expectedAccounts[i].Balance, actual.Accounts[i].Balance);
            }

            for (var i = 0; i < expectedTransactions.Count; i++)
            {
                Assert.AreEqual(expectedTransactions[i].Id, actual.Transactions[i].TransactionId);
                Assert.AreEqual(expectedTransactions[i].Amount, actual.Transactions[i].Amount);
                Assert.AreEqual(expectedTransactions[i].AccountId, actual.Transactions[i].AccountId);
            }
        }
    }
}