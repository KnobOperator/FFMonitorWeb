using FFMonitorWeb.Data;
using FFMonitorWeb.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace FFMonitorWeb.Controllers {
	[Route("api/[controller]")]
    [ApiController]
    public class ExerciseTypesController : ControllerBase
    {
        private readonly MongoDbContext _context;

        public ExerciseTypesController(MongoDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var exerciseTypes = _context.ExerciseTypes.Find(ex => true).ToList();
            return Ok(exerciseTypes);
        }

        [HttpPost]
        public IActionResult Create(ExerciseType exerciseType)
        {
            _context.ExerciseTypes.InsertOne(exerciseType);
            return Ok(exerciseType);
        }
    }
}
