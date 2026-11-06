using BethanysPieShop.Data;
using BethanysPieShop.Models;

namespace BethanysPieShop.Repositories;

public class CategoryRepository : ICategoryRepository
{
	private readonly BethanysPieShopDbContext pieShopDbContext;

	public CategoryRepository(BethanysPieShopDbContext bethanysPieShopDbContext)
	{
		pieShopDbContext = bethanysPieShopDbContext;
	}

	public IEnumerable<Category> AllCategories => pieShopDbContext.Categories.OrderBy(p => p.CategoryName);
}
