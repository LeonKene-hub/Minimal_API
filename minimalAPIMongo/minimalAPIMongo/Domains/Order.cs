﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

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
        public string? Status { get; set; }

        //refenrecia aos produtos por meio de ids
        [BsonElement("productId")]
        [JsonIgnore]
        public List<string>? ProductId { get; set; }

        //referencia para que quando eu liste os pedidos, venha os dados de cada produto(lista)
        [BsonElement("product")]
        public List<Product>? Products { get; set; }

        //quem faz a compra
        [BsonElement("clientId")]
        public string? ClientId { get; set; }

        [BsonElement("client")]
        public Client? Client { get; set; }
    }
}
