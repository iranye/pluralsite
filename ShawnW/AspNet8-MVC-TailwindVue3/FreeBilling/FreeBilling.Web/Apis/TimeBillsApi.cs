using FluentValidation;
using FreeBilling.Data.Entities;
using FreeBilling.Web.Data;
using FreeBilling.Web.Models;
using FreeBilling.Web.Validators;
using Mapster;
using System.Security.Claims;

namespace FreeBilling.Web.Apis;

public static class TimeBillsApi
{
    public static void Register(WebApplication app)
    {
        // --01 before grouping
        // app.MapGet("{id:int}", GetTimeBill).WithName("GetTimeBill");
        // app.MapPost("", PostTimeBill);

        // --02 after grouping
        var group = app.MapGroup("/api/timebills");
        group.MapGet("{id:int}", GetTimeBill).WithName("GetTimeBill")
            .RequireAuthorization("ApiPolicy");

        group.MapPost("", PostTimeBill)
            .AddEndpointFilter<ValidateEndpointFilter<TimeBillModel>>()
            .RequireAuthorization("ApiPolicy");
    }

    public static async Task<IResult> GetTimeBill(IBillingRepository repository, int id)
    {
        var timeBill = await repository.GetTimeBill(id);
        if (timeBill == null) return Results.NotFound();
        return Results.Ok(timeBill);
    }

    public static async Task<IResult> PostTimeBill(IBillingRepository repository, TimeBillModel model, ClaimsPrincipal user)
    {
        var newEntity = model.Adapt<TimeBill>();

        var employeeEmail = user.Identity?.Name!;
        if (String.IsNullOrWhiteSpace(employeeEmail)) return Results.BadRequest("Missing Employee Name or Email");

        var employee = await repository.GetEmployee(employeeEmail);
        if (employee == null) return Results.BadRequest($"No employee found with email: {employeeEmail}");
        newEntity.EmployeeId = employee.Id;

        repository.AddEntity(newEntity);

        if (await repository.SaveChanges())
        {
            var newTimeBill = await repository.GetTimeBill(newEntity.Id);
            return Results.CreatedAtRoute("GetTimeBill", new { id = newEntity.Id }, newEntity);
        }
        return Results.BadRequest();
    }
}
