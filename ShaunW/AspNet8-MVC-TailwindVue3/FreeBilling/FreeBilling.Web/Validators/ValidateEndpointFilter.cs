using FluentValidation;

namespace FreeBilling.Web.Validators;

public class ValidateEndpointFilter<T> : IEndpointFilter where T : class
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        var model = context.Arguments
            .OfType<T>()
            .FirstOrDefault();

        if (model != null)
        {
            var validator = context.HttpContext.RequestServices
            .GetRequiredService<IValidator<T>>();

            var validation = validator.Validate(model);
            if (!validation.IsValid)
            {
                return Results.ValidationProblem(validation.ToDictionary());
            }
        }

        return await next(context);
    }
}
