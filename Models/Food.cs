using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FFMonitorWeb.Models {
	public class Food
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("foodType")]
        public string FoodType { get; set; }

        [BsonElement("cal")]
        public double Calories { get; set; }

        [BsonElement("ch")]
        public double Carbohydrates { get; set; }

        [BsonElement("lipid")]
        public double Lipid { get; set; }

        [BsonElement("protein")]
        public double Protein { get; set; }

        [BsonElement("amount")]
        public double Amount { get; set; }
    }
}
