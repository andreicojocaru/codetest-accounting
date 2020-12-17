using CodeTest.Accounting.Contracts;

namespace CodeTest.Accounting.Customers.Database
{
    public interface ICustomerDatabase
    {
        Customer GetCustomer(int id);

        void CreateCustomer(Customer customer);
    }
}
