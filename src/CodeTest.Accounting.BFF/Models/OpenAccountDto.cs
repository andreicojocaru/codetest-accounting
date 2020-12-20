using System.ComponentModel.DataAnnotations;

namespace CodeTest.Accounting.BFF.Models
{
    public class OpenAccountDto
    {
        [Required]
        public int CustomerId { get; set; }

        public decimal? InitialCredit { get; set; }
    }
}
