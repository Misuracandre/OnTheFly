using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using OnTheFly.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnTheFly.Models
{
    public class AirCraft
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Rab { get; set; }
        public int Capacity { get; set; }
        public DateTime DtRegistry { get; set; }
        public DateTime? DtLastFlight { get; set; }
        public CompanyGetDTO Company { get; set; }

        public AirCraft() { }

        public AirCraft(AirCraftDTO airCraft)
        {
            this.Rab = airCraft.Rab;
            this.Capacity = airCraft.Capacity;
            this.DtRegistry = airCraft.DtRegistry;
            this.DtLastFlight = airCraft.DtLastFlight;
            this.Company = airCraft.Company;
        }

        /*public AirCraft(AirCraftInsertDTO airCraft)
        {
            this.Rab = airCraft.Rab;
            this.Capacity = airCraft.Capacity;
            this.DtRegistry = airCraft.DtRegistry;
            this.DtLastFlight = airCraft.DtLastFlight;
            this.Company = new Company(airCraft.Company);
        }*/
    }
}
