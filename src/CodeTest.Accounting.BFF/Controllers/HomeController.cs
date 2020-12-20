using System;
using System.Diagnostics;
using System.Threading.Tasks;
using CodeTest.Accounting.BFF.Core;
using CodeTest.Accounting.BFF.Models;
using CodeTest.Accounting.ServiceClients;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CodeTest.Accounting.BFF.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly CustomersServiceClient _customersServiceClient;
        private readonly AccountsOrchestrator _accountsOrchestrator;
        private readonly InformationOrchestrator _informationOrchestrator;

        public HomeController(
            ILogger<HomeController> logger,
            CustomersServiceClient customersServiceClient,
            AccountsOrchestrator accountsOrchestrator,
            InformationOrchestrator informationOrchestrator)
        {
            _logger = logger;
            _customersServiceClient = customersServiceClient;
            _accountsOrchestrator = accountsOrchestrator;
            _informationOrchestrator = informationOrchestrator;
        }

        public IActionResult Index()
        {
            var model = new AccountingViewModel
            {
                CreateCustomerDto = new CustomerDto(),
                OpenAccountDto = new OpenAccountDto()
            };

            return View(model);
        }

        [Route("create-customer")]
        public async Task<IActionResult> CreateCustomer(AccountingViewModel input)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }

            try
            {
                var customer = await _customersServiceClient.PostAsync(input.CreateCustomerDto);
                return RedirectToAction(nameof(UserInformation), new { customerId = customer.Id });
            }
            catch (Exception e)
            {
                const string message = "Customer not valid!";

                _logger.LogError(e, message);
                return BadRequest(message);
            }
        }

        [Route("open-account")]
        public async Task<IActionResult> OpenAccount(AccountingViewModel input)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _accountsOrchestrator.OpenAccountAsync(input.OpenAccountDto);
            }
            catch (CustomerNotValidException e)
            {
                const string message = "Customer not valid!";

                _logger.LogError(e, message);
                return BadRequest(message);
            }

            return RedirectToAction(nameof(UserInformation), new { customerId = input.OpenAccountDto.CustomerId });
        }

        [Route("user-information")]
        public async Task<IActionResult> UserInformation(int customerId)
        {
            var viewModel = new AccountingViewModel
            {
                OpenAccountDto = new OpenAccountDto
                {
                    CustomerId = customerId
                },
                CreateCustomerDto = new CustomerDto()
            };

            if (customerId == default)
            {
                return BadRequest();
            }

            viewModel.UserInfo = await _informationOrchestrator.GetUserInfoAsync(customerId);

            return View("Index", viewModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


    }
}
