using BethanysPieShop.Models;

namespace BethanysPieShop.Repositories
{
    public interface ICategoryRepository
    {
        IEnumerable<Category> AllCategories { get; }
    }
}
