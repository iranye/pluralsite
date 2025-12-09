using FreeBilling.Web.Data;

namespace FreeBilling.Web.Apis;

public static class EmployeesApi
{
    public static void Register(WebApplication app)
    {
        app.MapGet("/api/employees", GetEmployees)
            .RequireAuthorization("ApiPolicy");
    }

    private static async Task<IResult> GetEmployees(IBillingRepository repository)
    {
        return Results.Ok(await repository.GetEmployees());
    }
}
