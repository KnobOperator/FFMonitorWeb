using FFMonitorWeb.Data;
using FFMonitorWeb.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace FFMonitorWeb.Controllers {
	[Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly MongoDbContext _context;

        public UsersController(MongoDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var users = _context.Users.Find(u => true).ToList();
            return Ok(users);
        }

        [HttpPost]
        public IActionResult Create(User user)
        {
            _context.Users.InsertOne(user);
            return Ok(user);
        }
    }
}
