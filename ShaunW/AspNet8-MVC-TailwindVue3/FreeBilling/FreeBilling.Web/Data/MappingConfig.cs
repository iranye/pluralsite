using FreeBilling.Data.Entities;
using FreeBilling.Web.Models;
using Mapster;

namespace FreeBilling.Web.Data;

public class MappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<TimeBillModel, TimeBill>()
            .TwoWays()
            .Map(d => d.Hours, s => s.HoursWorked);
    }
}
