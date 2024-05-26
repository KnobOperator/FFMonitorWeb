using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace FFMonitorWeb.Models
{
    public class Workout
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("userId")]
        public string UserId { get; set; }

        [BsonElement("workoutType")]
        public string WorkoutType { get; set; }

        [BsonElement("date")]
        public DateTime Date { get; set; }

        [BsonElement("time")]
        public string Time { get; set; }

        [BsonElement("exercises")]
        public List<Exercise> Exercises { get; set; } = new List<Exercise>();

        [BsonIgnore]
        public double TotalCaloriesBurned { get; set; }
    }
}
    
