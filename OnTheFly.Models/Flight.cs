using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using OnTheFly.Models.Dto;

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

        public Flight() { }

        public Flight(FlightDTO flightDTO)
        {
            this.Id = string.Empty;
            this.Sales = flightDTO.Sales;
            this.Schedule = flightDTO.Schedule;
            this.Status = flightDTO.Status;
            this.Arrival = flightDTO.Arrival;
            this.Plane = new AirCraft(flightDTO.Plane);
        }

        public Flight(FlightInsertDTO flightInsertDTO)
        {
            this.Id = string.Empty;
            this.Sales = flightInsertDTO.Sales;
            this.Schedule = flightInsertDTO.Schedule;
            this.Status = flightInsertDTO.Status;
            this.Arrival = flightInsertDTO.Arrival;
            this.Plane = new AirCraft(flightInsertDTO.Plane);
        }

        public Flight(FlightGetDTO flightGetDTO)
        {
            this.Id = string.Empty;
            this.Sales = flightGetDTO.Sales;
            this.Schedule = flightGetDTO.Schedule;
            this.Status = flightGetDTO.Status;
            this.Arrival = flightGetDTO.Arrival;
            this.Plane = new AirCraft(flightGetDTO.Plane);
        }
    }
}
