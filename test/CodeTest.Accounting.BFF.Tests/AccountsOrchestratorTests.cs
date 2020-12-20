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
    public class AccountsOrchestratorTests
    {
        private Mock<AccountsServiceClient> _accountsServiceClientMock;
        private Mock<CustomersServiceClient> _customersServiceClientMock;
        private Mock<TransactionsServiceClient> _transactionsServiceClientMock;

        private AccountsOrchestrator _sut;

        [SetUp]
        public void Setup()
        {
            _accountsServiceClientMock = new Mock<AccountsServiceClient>(() => new AccountsServiceClient(new MockHttpClient()));
            _customersServiceClientMock = new Mock<CustomersServiceClient>(() => new CustomersServiceClient(new MockHttpClient()));
            _transactionsServiceClientMock = new Mock<TransactionsServiceClient>(() => new TransactionsServiceClient(new MockHttpClient()));

            var loggerMock = new Mock<ILogger<AccountsOrchestrator>>();

            _sut = new AccountsOrchestrator(
                loggerMock.Object,
                _accountsServiceClientMock.Object,
                _customersServiceClientMock.Object,
                _transactionsServiceClientMock.Object);
        }

        [Test]
        public async Task OpenAccountAsync_WithValidCustomer_ShouldCallAccountService()
        {
            // Arrange
            var input = new OpenAccountDto
            {
                CustomerId = 1,
                InitialCredit = 0
            };

            _customersServiceClientMock.Setup(c => c.GetAsync(input.CustomerId))
                .ReturnsAsync(new Customer());

            _accountsServiceClientMock.Setup(a => a.PostAsync(It.IsAny<AccountDto>()))
                .ReturnsAsync(new Account());

            // Act
            await _sut.OpenAccountAsync(input);

            // Assert
            _customersServiceClientMock.Verify(c => c.GetAsync(input.CustomerId), Times.Once);
            _accountsServiceClientMock.Verify(a => a.PostAsync(It.IsAny<AccountDto>()), Times.Once);
        }

        [Test]
        public void OpenAccountAsync_WithInValidCustomer_ShouldThrowException()
        {
            // Arrange
            var input = new OpenAccountDto
            {
                CustomerId = 1,
                InitialCredit = 0
            };

            _customersServiceClientMock.Setup(c => c.GetAsync(input.CustomerId))
                .ReturnsAsync((Customer)null);

            _accountsServiceClientMock.Setup(a => a.PostAsync(It.IsAny<AccountDto>()))
                .ReturnsAsync(new Account());

            // Act
            Assert.ThrowsAsync<CustomerNotValidException>(() => _sut.OpenAccountAsync(input));

            // Assert
            _customersServiceClientMock.Verify(c => c.GetAsync(input.CustomerId), Times.Once);
            _accountsServiceClientMock.Verify(a => a.PostAsync(It.IsAny<AccountDto>()), Times.Never);
        }

        [Test]
        public async Task OpenAccountAsync_WithInitialCredit_ShouldCallTransactionService()
        {
            // Arrange
            var input = new OpenAccountDto
            {
                CustomerId = 1,
                InitialCredit = 100
            };

            _customersServiceClientMock.Setup(c => c.GetAsync(input.CustomerId))
                .ReturnsAsync(new Customer());

            _accountsServiceClientMock.Setup(a => a.PostAsync(It.IsAny<AccountDto>()))
                .ReturnsAsync(new Account());

            _transactionsServiceClientMock.Setup(t => t.PostAsync(It.IsAny<TransactionDto>()))
                .Returns(Task.CompletedTask);

            // Act
            await _sut.OpenAccountAsync(input);

            // Assert
            _customersServiceClientMock.Verify(c => c.GetAsync(input.CustomerId), Times.Once);
            _accountsServiceClientMock.Verify(a => a.PostAsync(It.IsAny<AccountDto>()), Times.Once);
            _transactionsServiceClientMock.Verify(a => a.PostAsync(It.IsAny<TransactionDto>()), Times.Once);
        }
    }
}