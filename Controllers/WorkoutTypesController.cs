using FFMonitorWeb.Data;
using FFMonitorWeb.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace FFMonitorWeb.Controllers {
	[Route("api/[controller]")]
    [ApiController]
    public class WorkoutTypesController : ControllerBase
    {
        private readonly MongoDbContext _context;

        public WorkoutTypesController(MongoDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var workoutTypes = _context.WorkoutTypes.Find(wt => true).ToList();
            return Ok(workoutTypes);
        }

        [HttpPost]
        public IActionResult Create(WorkoutType workoutType)
        {
            _context.WorkoutTypes.InsertOne(workoutType);
            return Ok(workoutType);
        }
    }
}
