using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace FFMonitorWeb.Models
{
    public class Meal
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        
        [BsonElement("userId")]
        public string UserId { get; set; }

        [BsonElement("mealType")]
        public string MealType { get; set; }

        [BsonElement("date")]
        public DateTime Date { get; set; }

        [BsonElement("time")]
        public string Time { get; set; }

        [BsonElement("foods")]
        public List<Food> Foods { get; set; }

        [BsonIgnore]
        public double TotalCalories { get; set; }

        [BsonIgnore]
        public double TotalCarbohydrates { get; set; }

        [BsonIgnore]
        public double TotalLipids { get; set; }

        [BsonIgnore]
        public double TotalProteins { get; set; }
    }
}
