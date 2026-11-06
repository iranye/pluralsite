using BethanysPieShop.Data;
using BethanysPieShop.Models;
using Microsoft.EntityFrameworkCore;

namespace BethanysPieShop.Repositories
{
	public class PieRepository : IPieRepository
	{
		private readonly BethanysPieShopDbContext pieShopDbContext;

		public PieRepository(BethanysPieShopDbContext bethanysPieShopDbContext)
		{
			pieShopDbContext = bethanysPieShopDbContext;
		}

		public IEnumerable<Pie> AllPies
		{
			get
			{
				return pieShopDbContext.Pies.Include(c => c.Category);
			}
		}

		public IEnumerable<Pie> PiesOfTheWeek
		{
			get
			{
				return pieShopDbContext.Pies.Include(c => c.Category).Where(p => p.IsPieOfTheWeek);
			}
		}

		public Pie? GetPieById(int pieId)
		{
			return pieShopDbContext.Pies.FirstOrDefault(p => p.PieId == pieId);
		}

		public IEnumerable<Pie> SearchPies(string searchQuery)
		{
			throw new NotImplementedException();
		}
	}
}
