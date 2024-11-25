using Contracts.Constants;
using Microsoft.AspNetCore.Authorization;
using WebMVC.Models;
using WebMVC.Services;

namespace WebMVC.Controllers
{
    [Authorize(Roles = Roles.Admin)]
    public class AdminController : Controller
    {
        private readonly ICatalogService _catalogService;

        public AdminController(ICatalogService catalogService)
        {
            _catalogService = catalogService;
        }

        [HttpGet]
        public IActionResult AddPlate()
        {
            return View(new CreatePlateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPlate(CreatePlateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _catalogService.CreatePlateAsync(model);

            if (result.IsFailure)
            {
                model.Errors = result.Errors.ToList();
                return View(model);
            }

            TempData["Success"] = "Plate created successfully";
            return RedirectToAction(nameof(AddPlate));
        }
    }
}
