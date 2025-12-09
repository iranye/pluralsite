using FreeBilling.Data.Entities;

namespace FreeBilling.Web.Data;

public interface IBillingRepository
{
    Task<IEnumerable<Customer>> GetCustomers();
    Task<IEnumerable<Customer>> GetCustomersWithAddressInfo();
    Task<Customer?> GetCustomer(int id);
    Task<IEnumerable<Employee>> GetEmployees();
    Task<Employee?> GetEmployee(string name);

    Task<TimeBill?> GetTimeBill(int id);

    void AddEntity<T>(T entity) where T : notnull;
    Task<bool> SaveChanges();
    Task<IEnumerable<TimeBill>> GetTimeBillsForCustomer(int id);
    Task<TimeBill?> GetTimeBillForCustomer(int id, int billId);
}