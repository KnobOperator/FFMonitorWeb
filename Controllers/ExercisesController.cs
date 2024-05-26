using FFMonitorWeb.Data;
using FFMonitorWeb.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace FFMonitorWeb.Controllers {
	[Route("api/[controller]")]
    [ApiController]
    public class ExercisesController : ControllerBase
    {
        private readonly MongoDbContext _context;

        public ExercisesController(MongoDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var exercises = _context.Exercises.Find(ex => true).ToList();
            return Ok(exercises);
        }

        [HttpPost]
        public IActionResult Create(Exercise exercise)
        {
            _context.Exercises.InsertOne(exercise);
            return Ok(exercise);
        }
    }
}
