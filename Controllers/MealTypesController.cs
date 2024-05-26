using FFMonitorWeb.Data;
using FFMonitorWeb.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace FFMonitorWeb.Controllers {
	[Route("api/[controller]")]
    [ApiController]
    public class MealTypesController : ControllerBase
    {
        private readonly MongoDbContext _context;

        public MealTypesController(MongoDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var mealTypes = _context.MealTypes.Find(mt => true).ToList();
            return Ok(mealTypes);
        }

        [HttpPost]
        public IActionResult Create(MealType mealType)
        {
            _context.MealTypes.InsertOne(mealType);
            return Ok(mealType);
        }
    }
}
