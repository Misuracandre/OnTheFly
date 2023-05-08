using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OnTheFly.Models
{
    public class Flight
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public int Sales { get; set; }
        public DateTime Schedule { get; set; }
        public bool Status { get; set; }
        public Airport Arrival { get; set; }
        public AirCraft Plane { get; set; }
    }
}
