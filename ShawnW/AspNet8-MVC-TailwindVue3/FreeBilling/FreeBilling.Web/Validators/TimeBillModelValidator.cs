using FluentValidation;
using FreeBilling.Web.Models;

namespace FreeBilling.Web.Validators;

public class TimeBillModelValidator : AbstractValidator<TimeBillModel>
{
    public TimeBillModelValidator()
    {
        RuleFor(x => x.CustomerId).NotEmpty();
        RuleFor(x => x.CustomerId).NotEmpty();
        RuleFor(x => x.BillingRate).InclusiveBetween(50.0, 300.0);

        RuleFor(x => x.HoursWorked).InclusiveBetween(.1, 12.0);

        RuleFor(x => x.WorkPerformed)
            .NotEmpty()
            .MinimumLength(15);
    }
}
