using JurisTempus.Models;
using Mapster;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace JurisTempus.Pages;

public class IndexPage(BillingContext context) : PageModel
{
	public List<ClientModel> Clients { get; set; } = new();

	public async Task OnGetAsync()
	{
		var results = await context.Clients
		  .Include(c => c.Address)
		  .Include(c => c.Cases)
		  .OrderBy(c => c.Name)
		  .ProjectToType<ClientModel>()
		  .ToListAsync();

		Clients.AddRange(results);
	}
}
