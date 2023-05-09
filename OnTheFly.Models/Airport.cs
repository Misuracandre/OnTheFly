using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OnTheFly.Models
{
    public class Airport
    {
        [JsonPropertyName("iata")]
        [BsonRepresentation(BsonType.String)]
        public string Iata { get; set; }
        [JsonPropertyName("state")]
        public string State { get; set; }
        [JsonPropertyName("city")]
        public string City { get; set; }

        [JsonPropertyName("country_id")]
        public string Country_id { get; set; }
    }
}
