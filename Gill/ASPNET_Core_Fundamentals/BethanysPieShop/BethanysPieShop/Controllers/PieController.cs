using BethanysPieShop.Repositories;
using BethanysPieShop.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BethanysPieShop.Controllers
{
    public class PieController : Controller
    {
        private readonly IPieRepository pieRepository;
        private readonly ICategoryRepository categoryRepository;

        public PieController(IPieRepository pieRepository, ICategoryRepository categoryRepository)
        {
            this.pieRepository = pieRepository ?? throw new ArgumentNullException(nameof(pieRepository));
            this.categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
		}

		public IActionResult List()
		{
			ViewBag.CurrentCategory = "Cheese cakes";

			PieListViewModel piesListViewModel = new PieListViewModel(pieRepository.AllPies, "Cheese cakes");
			return View(piesListViewModel);
		}

		public IActionResult Details(int id)
		{
			var pie = pieRepository.AllPies.FirstOrDefault(p => p.PieId == id);
			
			if (pie == null)
			{
				return NotFound();
			}

			return View(pie);
		}
	}
}
