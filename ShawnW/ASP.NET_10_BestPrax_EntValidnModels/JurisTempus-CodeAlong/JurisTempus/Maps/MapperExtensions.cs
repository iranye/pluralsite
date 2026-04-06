using JurisTempus.Models;
using Mapster;

namespace JurisTempus.Maps
{
	public static class MapperExtensions
	{
		public static WebApplicationBuilder AddMaps(this WebApplicationBuilder builder)
		{
			TypeAdapterConfig<Client, ClientModel>
				.NewConfig()
				// .TwoWays()
				.Map(c => c.CaseCount, e => e.Cases.Count())
				.Map(c => c.City, e => e.Address.CityTown);
			return builder;
		}
	}
}
