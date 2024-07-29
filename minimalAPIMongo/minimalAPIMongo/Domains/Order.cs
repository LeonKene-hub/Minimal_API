using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace minimalAPIMongo.Domains
{
    public class Order
    {
        [BsonId]
        [BsonElement("_id"),BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("date")]
        public DateOnly Date { get; set; }

        [BsonElement("status")]
        public bool Status { get; set; }

        [BsonElement("product")]
        public Product? Product { get; set; }

        [BsonElement("client")]
        public Client? Client { get; set; }
    }
}
