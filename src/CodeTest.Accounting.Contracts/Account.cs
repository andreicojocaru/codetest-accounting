using System.ComponentModel.DataAnnotations;

namespace CodeTest.Accounting.Contracts
{
    public class Account
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public int CustomerId { get; set; }

        public decimal Balance { get; set; }
    }
}
