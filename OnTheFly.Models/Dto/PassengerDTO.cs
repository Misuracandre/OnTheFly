using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace OnTheFly.Models.Dto
{
    public class PassengerDTO
    {

        [StringLength(14)]
        public string Cpf { get; set; }

        [StringLength(30)]
        public string Name { get; set; }

        [StringLength(1)]
        public string Gender { get; set; }

        [StringLength(14)]
        public string? Phone { get; set; }


        public DateTime DtBirth { get; set; }

        public DateTime DtRegister { get; set; }
        public bool? Status { get; set; }


        public AddressDTO Address { get; set; }

        public PassengerDTO() { }

        public PassengerDTO(Passenger passenger)
        {
            this.Cpf = passenger.Cpf;
            this.Name = passenger.Name;
            this.Gender = passenger.Gender.ToUpper();
            this.Phone = passenger.Phone;
            this.DtBirth = passenger.DtBirth;
            this.DtRegister = passenger.DtRegister;
            this.Status = passenger.Status;
            this.Address = new()
            {
                ZipCode = passenger.Address.ZipCode,
                Street = passenger.Address.Street,
                Number = passenger.Address.Number,
                City = passenger.Address.City,
                State = passenger.Address.State,
            };
        }
    }
}
