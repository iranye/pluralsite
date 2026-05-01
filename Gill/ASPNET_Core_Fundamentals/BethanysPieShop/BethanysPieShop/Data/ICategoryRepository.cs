using BethanysPieShop.Models;

namespace BethanysPieShop.Data
{
    public interface ICategoryRepository
    {
        IEnumerable<Category> AllCategories { get; }
    }
}
