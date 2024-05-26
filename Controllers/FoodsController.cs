using FFMonitorWeb.Data;
using FFMonitorWeb.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace FFMonitorWeb.Controllers {
	[Route("api/[controller]")]
    [ApiController]
    public class FoodsController : ControllerBase
    {
        private readonly MongoDbContext _context;

        public FoodsController(MongoDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var foods = _context.Foods.Find(f => true).ToList();
            return Ok(foods);
        }

        [HttpPost]
        public IActionResult Create(Food food)
        {
            _context.Foods.InsertOne(food);
            return Ok(food);
        }
    }
}
