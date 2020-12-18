namespace CodeTest.Accounting.Domain
{
    public class Transaction : IEntity
    {
        public int Id { get; set; }

        public int AccountId { get; set; }

        public decimal Amount { get; set; }
    }
}
