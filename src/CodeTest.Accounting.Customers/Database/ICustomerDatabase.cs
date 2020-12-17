namespace CodeTest.Accounting.Customers.Database
{
    public interface ICustomerDatabase
    {
        Customer GetCustomer(int id);

        void CreateCustomer(string name);
    }
}
