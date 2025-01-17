﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FFMonitorWeb.Models {
	public class MealType
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }
    }
}
