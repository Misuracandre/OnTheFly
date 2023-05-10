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
    public class CompanyInsertDTO
    {
        [StringLength(18)]
        public string Cnpj { get; set; }
        [StringLength(30)]
        public string? Name { get; set; }
        [StringLength(30)]
        public string NameOpt { get; set; }
        public string DtOpen { get; set; }
        public bool? Status { get; set; }
        public AddressInsert Address { get; set; }

        public CompanyInsertDTO() { }
        public CompanyInsertDTO(Company company)
        {
            Cnpj = company.Cnpj;
            Name = company.Name;
            NameOpt = company.NameOpt;
            DtOpen = company.DtOpen;
            Status = company.Status;
            Address = new AddressInsert(company.Address);
        }

    }
}
