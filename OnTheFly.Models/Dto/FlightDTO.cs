using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OnTheFly.Models.Dto
{
    public class FlightDTO
    {
        public int Sales { get; set; }
        public DateTime Departure { get; set; }
        public bool Status { get; set; }
        public Airport Destiny { get; set; }
        public AirCraftDTO Plane { get; set; }
    }
}
