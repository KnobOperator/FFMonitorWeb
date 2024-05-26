using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FFMonitorWeb.Models {
	public class Exercise
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("exerciseType")]
        public string ExerciseType { get; set; }

        [BsonElement("calBurned")]
        public int CalBurned { get; set; }
    }
}
