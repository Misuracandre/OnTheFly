using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using OnTheFly.Models.Dto;

namespace OnTheFly.Models
{
    public class Address
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [JsonProperty("cep")]
        public string ZipCode { get; set; }

        [JsonProperty("logradouro")]
        public string? Street { get; set; }
        public int Number { get; set; }

        [JsonProperty("complemento")]
        public string? Complement { get; set; }

        [JsonProperty("localidade")]
        public string? City { get; set; }

        [JsonProperty("uf")]
        public string State { get; set; }

        public Address () { }
        public Address(AddressDTO address)
        {
            ZipCode = address.ZipCode;
            Street = address.Street;
            Number = address.Number;
            Complement = address.Complement;
            City = address.City;
            State = address.State;
        }
    }
}