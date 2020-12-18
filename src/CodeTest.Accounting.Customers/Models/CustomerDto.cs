using System.ComponentModel.DataAnnotations;

namespace CodeTest.Accounting.Customers.Models
{
    public class CustomerDto
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string Surname { get; set; }
    }
}
