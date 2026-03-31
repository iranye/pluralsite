using FreeBilling.Data.Entities;
using FreeBilling.Web.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FreeBilling.Web.Controllers;

// [Route("/api/[controller]]")] // <-- use this in case there's a class rename
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("/api/customers")]
public class CustomersController : ControllerBase
{
    private readonly ILogger<CustomersController> logger;
    private readonly IBillingRepository billingRepository;

    public CustomersController(ILogger<CustomersController> logger, IBillingRepository billingRepository)
    {
        this.logger = logger;
        this.billingRepository = billingRepository;
    }

    [AllowAnonymous]
    [HttpGet("")]
    public async Task<ActionResult<IEnumerable<Customer>>> Get(bool includeAddressInfo = false)
    {
        try
        {
            IEnumerable<Customer> results;
            if (includeAddressInfo)
            {
                results = await billingRepository.GetCustomersWithAddressInfo();
            }
            else
            {
                results = await billingRepository.GetCustomers();
            }
            return Ok(results);
        }
        catch (Exception ex)
        {
            logger.LogError($"Exception thrown when getting Customers: {ex.Message}");
            return Problem($"Failed to get Customers from the DB");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Customer>> GetCustomer(int id)
    {
        try
        {
            var result = await billingRepository.GetCustomer(id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError($"Exception thrown when getting Customer w id:{id}: {ex.Message}");
            return Problem($"Exception thrown: {ex.Message}");
        }
    }
}
