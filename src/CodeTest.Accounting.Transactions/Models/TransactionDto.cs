using System.ComponentModel.DataAnnotations;

namespace CodeTest.Accounting.Transactions.Models
{
    public class TransactionDto
    {
        [Required]
        public int AccountId { get; set; }

        [Required]
        public decimal Amount { get; set; }
    }
}
