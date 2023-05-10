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
        public string DtOpen { get; set; }
        public bool? Status { get; set; }
        public AddressDTO Address { get; set; }

        public CompanyDTO() { }
        public CompanyDTO(Company company)
        {
            Cnpj = company.Cnpj;
            Name = company.Name;
            NameOpt = company.NameOpt;
            DtOpen = company.DtOpen.ToString("dd/MM/yyyy");
            Status = company.Status;
            Address = new AddressDTO()
            {
                ZipCode = company.Address.ZipCode,
                Street = company.Address.Street,
                Number = company.Address.Number,
                Complement = company.Address.Complement,
                City = company.Address.City,
                State = company.Address.State,
            };
        }
    }
}
