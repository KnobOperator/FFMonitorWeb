using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FFMonitorWeb.Models {
	public class WorkoutType
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
