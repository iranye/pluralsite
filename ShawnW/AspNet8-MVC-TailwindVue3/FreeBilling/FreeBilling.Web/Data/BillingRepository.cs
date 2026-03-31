using FreeBilling.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace FreeBilling.Web.Data;

public class BillingRepository : IBillingRepository
{
    private readonly BillingContext context;
    private readonly ILogger<BillingRepository> logger;

    public BillingRepository(BillingContext context, ILogger<BillingRepository> logger)
    {
        this.context = context;
        this.logger = logger;
    }

    public async Task<IEnumerable<Employee>> GetEmployees()
    {
        try
        {
            return await context.Employees
              .OrderBy(e => e.Name)
              .ToListAsync();

        }
        catch (Exception ex)
        {
            logger.LogError($"Could not get Employees: {ex.Message}");
            throw;
        }
    }

    public async Task<Employee?> GetEmployee(string name)
    {
        return await context.Employees.FirstOrDefaultAsync(x => x.Email == name);
    }

    public async Task<IEnumerable<Customer>> GetCustomers()
    {
        try
        {
            return await context.Customers
              .OrderBy(c => c.CompanyName)
              .ToListAsync();
        }
        catch (Exception ex)
        {
            logger.LogError($"Could not get Customers: {ex.Message}");
            throw;
        }
    }

    public async Task<IEnumerable<Customer>> GetCustomersWithAddressInfo()
    {
        try
        {
            return await context.Customers
                .Include(x => x.Address)
                .OrderBy(c => c.CompanyName)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            logger.LogError($"Could not get Customers: {ex.Message}");
            throw;
        }
    }

    public async Task<Customer?> GetCustomer(int id)
    {
        try
        {
            return await context.Customers
              .FirstOrDefaultAsync(c => c.Id == id);
        }
        catch (Exception ex)
        {
            logger.LogError($"Could not get Customer By Id: {id} - {ex.Message}");
            throw;
        }
    }

    public async Task<bool> SaveChanges()
    {
        try
        {
            return await context.SaveChangesAsync() > 0;
        }
        catch (Exception ex)
        {
            logger.LogError($"Could not Save to the Database: {ex.Message}");
            throw;
        }
    }

    public async Task<TimeBill?> GetTimeBill(int id)
    {
        try
        {
            var timeBill = await context.TimeBills
                .Include(b => b.Employee)
                .Include(b => b.Customer)
                .ThenInclude(c => c!.Address)
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();
            return timeBill;
        }
        catch (Exception ex)
        {
            logger.LogError($"Could not get Customer By Id: {id} - {ex.Message}");
            throw;
        }
    }

    public void AddEntity<T>(T entity) where T : notnull
    {
        context.Add(entity);
    }

    public async Task<IEnumerable<TimeBill>> GetTimeBillsForCustomer(int id)
    {
        return await context.TimeBills.Where(x => x.Customer != null && x.CustomerId == id)
            .Include(b => b.Customer)
            .Include(b => b.Employee)
            .ToListAsync();
    }

    public async Task<TimeBill?> GetTimeBillForCustomer(int id, int billId)
    {
        return await context.TimeBills.Where(x => x.Customer != null && x.CustomerId == id && x.Id == billId)
            .FirstOrDefaultAsync();
    }
}
