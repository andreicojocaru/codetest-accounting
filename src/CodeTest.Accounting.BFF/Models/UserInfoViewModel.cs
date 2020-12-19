using System.Collections.Generic;

namespace CodeTest.Accounting.BFF.Models
{
    public class UserInfoViewModel
    {
        public string Name { get; set; }

        public string Surname { get; set; }

        public decimal Balance
        {
            get
            {
                decimal balance = 0;
                foreach (var account in Accounts)
                {
                    balance += account.Balance;
                }

                return balance;
            }
        }

        public IList<UserAccountViewModel> Accounts { get; set; }

        public IList<UserAccountTransactionViewModel> Transactions { get; set; }
    }

    public class UserAccountViewModel
    {
        public int AccountId { get; set; }

        public decimal Balance { get; set; }
    }

    public class UserAccountTransactionViewModel
    {
        public int AccountId { get; set; }

        public int TransactionId { get; set; }

        public decimal Amount { get; set; }
    }
}
