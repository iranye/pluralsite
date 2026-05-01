using BethanysPieShop.Models;

namespace BethanysPieShop.Data
{
    public interface IPieRepository
    {
        IEnumerable<Pie> AllPies { get; }

        IEnumerable<Pie> PiesOfTheWeek { get; }

        Pie? GetPieById(int pieId);

        IEnumerable<Pie> SearchPies(string searchQuery);
    }
}
