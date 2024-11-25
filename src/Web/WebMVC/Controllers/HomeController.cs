using Contracts.Plates;
using MassTransit;
using RTCodingExercise.Microservices.Models;
using System.Diagnostics;
using WebMVC.Services;
using Contracts.Constants;
using Microsoft.AspNetCore.Authorization;

namespace RTCodingExercise.Microservices.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IRequestClient<GetPlatesRequest> _client;
        private readonly ICatalogService _catalogService;

        public HomeController(
            ILogger<HomeController> logger, 
            IRequestClient<GetPlatesRequest> requestClient,
            ICatalogService catalogService)
        {
            _logger = logger;
            _client = requestClient;
            _catalogService = catalogService;
        }

        public async Task<IActionResult> Index(CancellationToken ct, PlatesFilter platesFilter)
        {
            AssignSortTerms(platesFilter);
            
            var isAdmin = User.IsInRole(Roles.Admin);
            var result = await _catalogService.GetPlatesAsync(platesFilter, isAdmin, ct);

            if (result.IsFailure)
            {
                TempData["ErrorMessage"] = result.Errors.First();
            }

            return View(result.Value);
        }

        private static void AssignSortTerms(PlatesFilter platesFilter)
        {
            if (platesFilter.SortBy?.Contains(':') ?? false)
            {
                var parts = platesFilter.SortBy.Split(':');
                platesFilter.SortBy = parts[0];
                platesFilter.OrderBy = parts[1];
            }
        }

        [HttpPost]
        [Authorize(Roles = Roles.Admin)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReservePlate(Guid plateId)
        {
            var result = await _catalogService.ReservePlateAsync(plateId);

            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "Plate reserved successfully";
            }
            else
            {
                TempData["ErrorMessage"] = string.Join(", ", result.Errors);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = Roles.User)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BuyPlate(Guid plateId, PlatesFilter platesFilter)
        {
            var result = await _catalogService.BuyPlateAsync(plateId, platesFilter.DiscountCode);

            if (result.IsFailure)
            {
                TempData["ErrorMessage"] = result.Errors.First();
            }
            else
            {
                TempData["SuccessMessage"] = "Plate purchased successfully!";
            }

            return RedirectToAction(nameof(Index), platesFilter);
        }

        public async Task<IActionResult> Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}