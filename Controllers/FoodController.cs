﻿using FFMonitorWeb.Data;
using FFMonitorWeb.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;

namespace FFMonitorWeb.Controllers
{
    public class FoodController : Controller
    {
        private readonly MongoDbContext _context;
        private readonly ILogger<FoodController> _logger;

        public FoodController(MongoDbContext context, ILogger<FoodController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index(string sortOrder)
        {
            _logger.LogInformation("Index action called with sortOrder: {SortOrder}", sortOrder);

            ViewData["NameSortParm"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["TypeSortParm"] = sortOrder == "Type" ? "type_desc" : "Type";
            ViewData["CaloriesSortParm"] = sortOrder == "Calories" ? "calories_desc" : "Calories";
            ViewData["CarbohydratesSortParm"] = sortOrder == "Carbohydrates" ? "carbohydrates_desc" : "Carbohydrates";
            ViewData["LipidSortParm"] = sortOrder == "Lipids" ? "lipid_desc" : "Lipids";
            ViewData["ProteinSortParm"] = sortOrder == "Proteins" ? "protein_desc" : "Proteins";

            var foods = _context.Foods.AsQueryable();

            switch (sortOrder)
            {
                case "name_desc":
                    _logger.LogInformation("Sorting by name descending");
                    foods = foods.OrderByDescending(f => f.Name);
                    break;
                case "Type":
                    _logger.LogInformation("Sorting by type ascending");
                    foods = foods.OrderBy(f => f.FoodType);
                    break;
                case "type_desc":
                    _logger.LogInformation("Sorting by type descending");
                    foods = foods.OrderByDescending(f => f.FoodType);
                    break;
                case "Calories":
                    _logger.LogInformation("Sorting by calories ascending");
                    foods = foods.OrderBy(f => f.Calories);
                    break;
                case "calories_desc":
                    _logger.LogInformation("Sorting by calories descending");
                    foods = foods.OrderByDescending(f => f.Calories);
                    break;
                case "Carbohydrates":
                    _logger.LogInformation("Sorting by carbohydrates ascending");
                    foods = foods.OrderBy(f => f.Carbohydrates);
                    break;
                case "carbohydrates_desc":
                    _logger.LogInformation("Sorting by carbohydrates descending");
                    foods = foods.OrderByDescending(f => f.Carbohydrates);
                    break;
                case "Lipids":
                    _logger.LogInformation("Sorting by lipids ascending");
                    foods = foods.OrderBy(f => f.Lipid);
                    break;
                case "lipid_desc":
                    _logger.LogInformation("Sorting by lipids descending");
                    foods = foods.OrderByDescending(f => f.Lipid);
                    break;
                case "Proteins":
                    _logger.LogInformation("Sorting by proteins ascending");
                    foods = foods.OrderBy(f => f.Protein);
                    break;
                case "protein_desc":
                    _logger.LogInformation("Sorting by proteins descending");
                    foods = foods.OrderByDescending(f => f.Protein);
                    break;
                default:
                    _logger.LogInformation("Sorting by name ascending");
                    foods = foods.OrderBy(f => f.Name);
                    break;
            }

            var foodList = await foods.ToListAsync();
            return View(foodList);
        }


        /* Adding food */
        [HttpGet]
        public IActionResult Add()
        {
            _logger.LogInformation("Add GET action called");
            ViewBag.FoodTypes = _context.FoodTypes.Find(_ => true).ToList().Select(ft => ft.Name).ToList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(Food food)
        {
            _logger.LogInformation("Add POST action called");

            // Remove ModelState errors related to Id as it will be generated by MongoDB
            ModelState.Remove("Id");
            food.Amount = 100;

            if (ModelState.IsValid)
            {
                _logger.LogInformation("Model state is valid, inserting food: {@Food}", food);
                await _context.Foods.InsertOneAsync(food);
                _logger.LogInformation("Food inserted successfully");
                return RedirectToAction(nameof(Index));
            }

            _logger.LogWarning("Model state is invalid");
            foreach (var state in ModelState)
            {
                if (state.Value.Errors.Any())
                {
                    _logger.LogWarning("Property: {Property}, Errors: {Errors}", state.Key, state.Value.Errors.Select(e => e.ErrorMessage));
                }
            }

            ViewBag.FoodTypes = _context.FoodTypes.Find(_ => true).ToList().Select(ft => ft.Name).ToList();
            return View(food);
        }


        /* Editing food */

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var food = await _context.Foods.Find(f => f.Id == id).FirstOrDefaultAsync();
            if (food == null)
            {
                return NotFound();
            }

            _logger.LogInformation("Editing food item: {@Food}", food);
            _logger.LogInformation("Food Details: Id={Id}, Name={Name}, FoodType={FoodType}, Calories={Calories}, Carbohydrates={Carbohydrates}, Lipid={Lipid}, Protein={Protein}", 
                                    food.Id, food.Name, food.FoodType, food.Calories, food.Carbohydrates, food.Lipid, food.Protein);

            ViewBag.FoodTypes = _context.FoodTypes.Find(_ => true).ToList().Select(ft => ft.Name).ToList();
            return View(food);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(string id, Food food)
        {
            _logger.LogInformation("POST Edit action called with Id={Id}", id);
            food.Amount = 100;

            // Log the incoming food object details
            _logger.LogInformation("Received food object: {@Food}", food);

            if (id != food.Id)
            {
                _logger.LogWarning("Id mismatch: Route Id={RouteId}, Model Id={ModelId}", id, food.Id);
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                _logger.LogInformation("Model state is valid. Updating food item: {@Food}", food);

                var filter = Builders<Food>.Filter.Eq(f => f.Id, id);
                var update = Builders<Food>.Update
                    .Set(f => f.Name, food.Name)
                    .Set(f => f.FoodType, food.FoodType)
                    .Set(f => f.Calories, food.Calories)
                    .Set(f => f.Carbohydrates, food.Carbohydrates)
                    .Set(f => f.Lipid, food.Lipid)
                    .Set(f => f.Protein, food.Protein);

                await _context.Foods.UpdateOneAsync(filter, update);

                _logger.LogInformation("Food item updated successfully: {@Food}", food);
                return RedirectToAction(nameof(Index));
            }

            _logger.LogWarning("Model state is invalid: {@ModelState}", ModelState);

            foreach (var state in ModelState)
            {
                if (state.Value.Errors.Any())
                {
                    _logger.LogWarning("Property: {Property}, Errors: {Errors}", state.Key, state.Value.Errors.Select(e => e.ErrorMessage));
                }
            }

            ViewBag.FoodTypes = _context.FoodTypes.Find(_ => true).ToList().Select(ft => ft.Name).ToList();
            return View(food);
        }

        /* Deelting food */

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var food = await _context.Foods.Find(f => f.Id == id).FirstOrDefaultAsync();
            if (food == null)
            {
                return NotFound();
            }

            return View(food);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var food = await _context.Foods.Find(f => f.Id == id).FirstOrDefaultAsync();
            if (food != null)
            {
                await _context.Foods.DeleteOneAsync(f => f.Id == id);
                _logger.LogInformation("Food item deleted: {Id}", id);
            }
            return RedirectToAction(nameof(Index));
        }

        /* Searching food */

        [HttpGet]
        public async Task<IActionResult> Search(string query)
        {
            var foods = await _context.Foods
                .Find(f => f.Name.ToLower().Contains(query.ToLower()))
                .Limit(10)
                .ToListAsync();

            return Json(foods.Select(f => new { id = f.Id, name = f.Name }));
        }

    }
}
