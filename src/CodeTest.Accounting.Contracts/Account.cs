namespace CodeTest.Accounting.Contracts
{
    public class Account : IEntity
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }

        public decimal Balance { get; set; }
    }
}
