using FreeBilling.Data.Entities;
using FreeBilling.Web.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FreeBilling.Web.Pages;

public class IndexModel : PageModel
{
    private readonly IBillingRepository billingRepository;

    public List<Customer> Customers { get; set; } = new();

    public IndexModel(IBillingRepository billingRepository)
    {
        this.billingRepository = billingRepository;
    }

    public async Task OnGet()
    {
        Customers = (await billingRepository.GetCustomers()).ToList();
    }
}
