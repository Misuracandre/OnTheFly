using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using OnTheFly.Models.Dto;

namespace OnTheFly.Models
{
    public class Sale
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public bool Reserved { get; set; }
        public bool Sold { get; set; }
        public Flight Flight { get; set; }
        public List<PassengerDTO> Passengers { get; set; }

        public Sale() {}

        public Sale(SaleDTO sale) 
        {
            this.Id = "";
            this.Reserved = sale.Reserved;
            this.Sold = sale.Sold;
            this.Flight = new();
            this.Flight.Schedule = DateTime.Now;
            this.Flight.Plane = new();
            this.Passengers = sale.Passengers;
        }
    }
}
