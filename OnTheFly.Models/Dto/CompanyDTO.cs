using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OnTheFly.Models.Dto
{
    public class CompanyDTO
    {
        [StringLength(18)]
        public string Cnpj { get; set; }
        [StringLength(30)]
        public string? Name { get; set; }
        [StringLength(30)]
        public string NameOpt { get; set; }
        public DateTime DtOpen { get; set; }
        public bool? Status { get; set; }
        public Address Address { get; set; }

        public CompanyDTO() { }
        public CompanyDTO(Company company)
        {
            Cnpj = company.Cnpj;
            Name = company.Name;
            NameOpt = company.NameOpt;
            DtOpen = company.DtOpen;
            Status = company.Status;
            Address = company.Address;
        }
    }
}
