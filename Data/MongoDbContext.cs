using FFMonitorWeb.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace FFMonitorWeb.Data
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;
        private readonly ILogger<MongoDbContext> _logger;

        public MongoDbContext(IOptions<MongoDbSettings> settings, ILogger<MongoDbContext> logger)
        {
            _logger = logger;
            _logger.LogInformation("Connecting to MongoDB with connection string: {ConnectionString}", settings.Value.ConnectionString);
            _logger.LogInformation("Using database: {DatabaseName}", settings.Value.DatabaseName);
            var client = new MongoClient(settings.Value.ConnectionString);
            _database = client.GetDatabase(settings.Value.DatabaseName);
            //EnsureIndexes();
            TestConnection();
        }

        
        public IMongoCollection<User> AppUsers => _database.GetCollection<User>("users");

        public void TestAppUsersCollection()
        {
            _logger.LogInformation("Testing AppUsers collection...");

            var collectionNames = _database.ListCollectionNames().ToList();
            _logger.LogInformation("Collections in database:");
            foreach (var name in collectionNames)
            {
                _logger.LogInformation("Collection: {Name}", name);
            }

            var allAppUsers = AppUsers.Find(_ => true).ToList();
            if (allAppUsers.Count == 0)
            {
                _logger.LogWarning("No users found in the AppUsers collection.");
            }
            else
            {
                _logger.LogInformation("Users found in the AppUsers collection: {Count}", allAppUsers.Count);
                foreach (var usr in allAppUsers)
                {
                    _logger.LogInformation("AppUser email: {Email}", usr.Email);
                }
            }
        }

        public async Task<User> FindUserByEmailAsync(string email)
        {
            _logger.LogInformation("Starting FindAppUserByEmailAsync for email: {Email}", email);

            // Log all users' emails
            var allUsers = await AppUsers.Find(_ => true).ToListAsync();
            if (allUsers.Count == 0)
            {
                _logger.LogWarning("No users found in the database.");
            }
            else
            {
                _logger.LogInformation("Listing all users in the database:");
                foreach (var usr in allUsers)
                {
                    _logger.LogInformation("AppUser email in database: {Email}", usr.Email);
                }
            }

            // Perform the case-insensitive comparison in memory
            var user = allUsers.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

            if (user != null)
            {
                _logger.LogInformation("User found: {User}", user);
            }
            else
            {
                _logger.LogWarning("No user found with email: {Email}", email);
            }

            return user;
        }



        /*private void EnsureIndexes()
        {
            var emailIndex = Builders<User>.IndexKeys.Ascending(user => user.Email);
            Users.Indexes.CreateOne(new CreateIndexModel<User>(emailIndex));
            AppUsers.Indexes.CreateOne(new CreateIndexModel<User>(emailIndex)); 
        }*/

        public void TestConnection()
        {
            try
            {
                _database.RunCommandAsync((Command<BsonDocument>)"{ping:1}").Wait();
                _logger.LogInformation("MongoDB connection is successful.");
            }
            catch (Exception ex)
            {
                _logger.LogError("MongoDB connection failed: {Message}", ex.Message);
                throw; // Re-throw the exception to stop the application
            }
        }

        
        public IMongoCollection<User> Users => _database.GetCollection<User>("users");
        public IMongoCollection<ExerciseType> ExerciseTypes => _database.GetCollection<ExerciseType>("exerciseTypes");
        public IMongoCollection<Exercise> Exercises => _database.GetCollection<Exercise>("exercises");
        public IMongoCollection<FoodType> FoodTypes => _database.GetCollection<FoodType>("foodTypes");
        public IMongoCollection<Food> Foods => _database.GetCollection<Food>("foods");
        public IMongoCollection<MealType> MealTypes => _database.GetCollection<MealType>("mealTypes");
        public IMongoCollection<Meal> Meals => _database.GetCollection<Meal>("Meals");
        public IMongoCollection<WorkoutType> WorkoutTypes => _database.GetCollection<WorkoutType>("workouttypes");
    }
}
