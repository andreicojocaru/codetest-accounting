using CodeTest.Accounting.ServiceClients;

namespace CodeTest.Accounting.BFF.Models
{
    public class AccountingViewModel
    {
        public CustomerDto CreateCustomerDto { get; set; }

        public OpenAccountDto OpenAccountDto { get; set; }

        public UserInfoViewModel UserInfo { get; set; }
    }
}
