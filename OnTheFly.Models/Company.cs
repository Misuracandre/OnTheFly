using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using OnTheFly.Models.Dto;

namespace OnTheFly.Models
{
    public class Company
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [StringLength(18)]
        public string Cnpj { get; set; }
        [StringLength(30)]
        public string? Name { get; set; }
        [StringLength(30)]
        public string NameOpt { get; set; }
        public DateTime DtOpen { get; set; }
        public bool? Status { get; set; }
        public AddressDTO? Address { get; set; }

        public Company() { }
        public Company(CompanyInsertDTO company)
        {
            Cnpj = company.Cnpj;
            Name = company.Name;
            NameOpt = company.NameOpt;
            Status = company.Status;
            Address = new AddressDTO() { ZipCode = company.Address.ZipCode, Number =  company.Address.Number };
        }
        public Company(CompanyGetDTO company)
        {
            Cnpj = company.Cnpj;
            Name = company.Name;
            NameOpt = company.NameOpt;
            Status = company.Status;
            Address = company.Address;
        }
    }
}
