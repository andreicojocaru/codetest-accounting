﻿using System.ComponentModel.DataAnnotations;

namespace CodeTest.Accounting.Contracts
{
    public class Customer
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string Surname { get; set; }
    }
}