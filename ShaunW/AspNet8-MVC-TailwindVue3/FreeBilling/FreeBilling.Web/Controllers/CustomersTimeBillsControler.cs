using FreeBilling.Data.Entities;
using FreeBilling.Web.Data;
using Microsoft.AspNetCore.Mvc;

namespace FreeBilling.Web.Controllers;

[Route("/api/customers/{id:int}/timebills")]
public class CustomersTimeBillsControler : ControllerBase
{
    private readonly ILogger<CustomersTimeBillsControler> logger;
    private readonly IBillingRepository repository;

    public CustomersTimeBillsControler(ILogger<CustomersTimeBillsControler> logger, IBillingRepository repository)
    {
        this.logger = logger;
        this.repository = repository;
    }

    [HttpGet("")]
    public async Task<ActionResult<IEnumerable<TimeBill>>> GetCustomerTimeBills(int id)
    {
        var result = await repository.GetTimeBillsForCustomer(id);
        if (result == null)
        {
            return BadRequest();
        }
        return Ok(result);
    }

    [HttpGet("{billId:int}")]
    public async Task<ActionResult<IEnumerable<TimeBill>>> GetCustomerTimeBills(int id, int billId)
    {
        var result = await repository.GetTimeBillForCustomer(id, billId);
        if (result == null)
        {
            return BadRequest();
        }
        return Ok(result);
    }
}
