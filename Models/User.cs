using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FFMonitorWeb.Models
{
    [CollectionName("users")]
    public class User : MongoIdentityUser<ObjectId>
    {
        [BsonElement("firstName")]
        [Required(ErrorMessage = "First name is required")]
        public string FirstName { get; set; }

        [BsonElement("lastName")]
        [Required(ErrorMessage = "Last name is required")]
        public string LastName { get; set; }

        [BsonElement("email")]
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public override string Email { get; set; }

        [BsonElement("userName")]
        [Required(ErrorMessage = "Username is required")]
        public override string UserName { get; set; }

        [BsonElement("password")]
        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        
        [BsonElement("passwordHash")]
        [Required(ErrorMessage = "Password is required")]
        public string PasswordHash { get; set; }

        [BsonIgnore]
        [Required(ErrorMessage = "Confirm Password is required")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [BsonElement("age")]
        [Range(0, 120, ErrorMessage = "Please enter a valid age")]
        public int Age { get; set; }

        [BsonElement("sex")]
        [Required(ErrorMessage = "Sex is required")]
        public string Sex { get; set; }

        [BsonElement("weight")]
        [Range(0, 1000, ErrorMessage = "Please enter a valid weight")]
        public int Weight { get; set; }

        [BsonElement("meals")]
        public List<Meal> Meals { get; set; }

        [BsonElement("workouts")]
        public List<Workout> Workouts { get; set; }
    }
}
