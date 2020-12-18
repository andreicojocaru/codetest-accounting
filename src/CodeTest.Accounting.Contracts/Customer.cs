namespace CodeTest.Accounting.Domain
{
    public class Customer : IEntity
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string Surname { get; set; }
    }
}