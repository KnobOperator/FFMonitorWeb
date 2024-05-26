using FFMonitorWeb.Data;
using FFMonitorWeb.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace FFMonitorWeb.Controllers {
	[Route("api/[controller]")]
    [ApiController]
    public class FoodTypesController : ControllerBase
    {
        private readonly MongoDbContext _context;

        public FoodTypesController(MongoDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var foodTypes = _context.FoodTypes.Find(ft => true).ToList();
            return Ok(foodTypes);
        }

        [HttpPost]
        public IActionResult Create(FoodType foodType)
        {
            _context.FoodTypes.InsertOne(foodType);
            return Ok(foodType);
        }
    }
}
