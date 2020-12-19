using System.ComponentModel.DataAnnotations;

namespace CodeTest.Accounting.Accounts.Models
{
    public class AccountDto
    {
        [Required]
        public int CustomerId { get; set; }

        public decimal? InitialCredit { get; set; }
    }
}
