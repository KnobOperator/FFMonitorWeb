using FFMonitorWeb.Data;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Linq;
using System.Threading.Tasks;
using FFMonitorWeb.Models;

namespace FFMonitorWeb.Controllers
{
    public class MealsController : Controller
    {
        private readonly MongoDbContext _context;
        private readonly ILogger<MealsController> _logger;

        public MealsController(MongoDbContext context, ILogger<MealsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index(string sortOrder)
        {
            _logger.LogInformation("Index action called with sortOrder: {SortOrder}", sortOrder);

            ViewData["MealTypeSortParm"] = string.IsNullOrEmpty(sortOrder) ? "mealType_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";
            ViewData["TimeSortParm"] = sortOrder == "Time" ? "time_desc" : "Time";

            var meals = await _context.Users.AsQueryable()
                .SelectMany(u => u.Meals)
                .ToListAsync();

            foreach (var meal in meals)
            {
                double totalCalories = 0;
                double totalCarbohydrates = 0;
                double totalLipids = 0;
                double totalProteins = 0;

                foreach (var food in meal.Foods)
                {
                    double amount = food.Amount;
                    _logger.LogInformation($"Food: {food.Name}, Amount: {amount}, Calories: {food.Calories}, Carbohydrates: {food.Carbohydrates}, Lipids: {food.Lipid}, Proteins: {food.Protein}");

                    totalCalories += food.Calories * amount / 100;
                    totalCarbohydrates += food.Carbohydrates * amount / 100;
                    totalLipids += food.Lipid * amount / 100;
                    totalProteins += food.Protein * amount / 100;
                }

                meal.TotalCalories = Math.Round(totalCalories, 2);
                meal.TotalCarbohydrates = Math.Round(totalCarbohydrates, 2);
                meal.TotalLipids = Math.Round(totalLipids, 2);
                meal.TotalProteins = Math.Round(totalProteins, 2);

                _logger.LogInformation($"Meal: {meal.MealType}, Total Calories: {meal.TotalCalories}, Total Carbohydrates: {meal.TotalCarbohydrates}, Total Lipids: {meal.TotalLipids}, Total Proteins: {meal.TotalProteins}");
            }

            switch (sortOrder)
            {
                case "mealType_desc":
                    meals = meals.OrderByDescending(m => m.MealType).ToList();
                    break;
                case "Date":
                    meals = meals.OrderBy(m => m.Date).ToList();
                    break;
                case "date_desc":
                    meals = meals.OrderByDescending(m => m.Date).ToList();
                    break;
                case "Time":
                    meals = meals.OrderBy(m => m.Time).ToList();
                    break;
                case "time_desc":
                    meals = meals.OrderByDescending(m => m.Time).ToList();
                    break;
                default:
                    meals = meals.OrderBy(m => m.MealType).ToList();
                    break;
            }

            return View(meals);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(Meal meal)
        {
            if (ModelState.IsValid)
            {
                await _context.Meals.InsertOneAsync(meal);
                return RedirectToAction(nameof(Index));
            }
            return View(meal);
        }

        /* Summary of meals */
        public async Task<IActionResult> Summary()
        {
            var meals = await _context.Users.AsQueryable()
                .SelectMany(u => u.Meals)
                .ToListAsync();

            foreach (var meal in meals)
            {
                double mealTotalCalories = 0;
                double mealTotalCarbohydrates = 0;
                double mealTotalLipids = 0;
                double mealTotalProteins = 0;

                foreach (var food in meal.Foods)
                {
                    double amount = food.Amount;
                    _logger.LogInformation($"Food: {food.Name}, Amount: {amount}, Calories: {food.Calories}, Carbohydrates: {food.Carbohydrates}, Lipids: {food.Lipid}, Proteins: {food.Protein}");

                    mealTotalCalories += food.Calories * amount / 100;
                    mealTotalCarbohydrates += food.Carbohydrates * amount / 100;
                    mealTotalLipids += food.Lipid * amount / 100;
                    mealTotalProteins += food.Protein * amount / 100;
                }

                meal.TotalCalories = Math.Round(mealTotalCalories, 2);
                meal.TotalCarbohydrates = Math.Round(mealTotalCarbohydrates, 2);
                meal.TotalLipids = Math.Round(mealTotalLipids, 2);
                meal.TotalProteins = Math.Round(mealTotalProteins, 2);

                _logger.LogInformation($"Meal: {meal.MealType}, Total Calories: {meal.TotalCalories}, Total Carbohydrates: {meal.TotalCarbohydrates}, Total Lipids: {meal.TotalLipids}, Total Proteins: {meal.TotalProteins}");
            }

            var summaryData = meals
                .GroupBy(m => m.Date.Date)
                .Select(g => new 
                {
                    Date = g.Key.ToString("yyyy-MM-dd"), // Format date as string
                    TotalCalories = g.Sum(m => m.TotalCalories),
                    TotalCarbohydrates = g.Sum(m => m.TotalCarbohydrates),
                    TotalLipids = g.Sum(m => m.TotalLipids),
                    TotalProteins = g.Sum(m => m.TotalProteins)
                })
                .OrderBy(s => s.Date)
                .ToList();

            double overallTotalCarbohydrates = summaryData.Sum(s => s.TotalCarbohydrates);
            double overallTotalLipids = summaryData.Sum(s => s.TotalLipids);
            double overallTotalProteins = summaryData.Sum(s => s.TotalProteins);
            double overallTotalCalories = summaryData.Sum(s => s.TotalCalories);

            ViewBag.TotalCarbohydrates = overallTotalCarbohydrates;
            ViewBag.TotalLipids = overallTotalLipids;
            ViewBag.TotalProteins = overallTotalProteins;
            ViewBag.TotalCalories = overallTotalCalories;
            ViewBag.SummaryData = summaryData;

            return View();
        }       
    }
}
