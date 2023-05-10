using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnTheFly.Models.Dto
{
    public class AirCraftDTO
    {
        public string Rab { get; set; }
        public int Capacity { get; set; }
        public DateTime DtRegistry { get; set; }
        public DateTime? DtLastFlight { get; set; }
        public CompanyInsertDTO Company { get; set; }

        public AirCraftDTO () { }

        public AirCraftDTO(AirCraft airCraft)
        {
            this.Rab = airCraft.Rab;
            this.Capacity = airCraft.Capacity;
            this.DtRegistry = airCraft.DtRegistry;
            this.DtLastFlight = airCraft.DtLastFlight;

            this.Company = airCraft.Company;

        }

        public AirCraftDTO(AirCraftInsertDTO airCraftInsertDTO)
        {
            this.Rab = airCraftInsertDTO.Rab;
            this.Capacity = airCraftInsertDTO.Capacity;
            this.DtRegistry = airCraftInsertDTO.DtRegistry;
            this.DtLastFlight = airCraftInsertDTO.DtLastFlight;
            this.Company = new CompanyGetDTO()
            {
                Cnpj = airCraftInsertDTO.Company
            };
        }
        
    }
}
