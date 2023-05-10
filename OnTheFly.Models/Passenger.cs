using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization.Attributes;
using OnTheFly.Models.Dto;

namespace OnTheFly.Models
{
    public class Passenger
    {

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

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

        public Passenger() { }

        public Passenger(PassengerDTO passenger)
        {
            this.Id = string.Empty;
            this.Cpf = passenger.Cpf;
            this.Name = passenger.Name;
            this.Gender = passenger.Gender.ToUpper();
            this.Phone = passenger.Phone;
            //this.DtRegister = passenger.DtRegister;
            this.Status = passenger.Status;
            this.Address = passenger.Address;
        }

        public Passenger(PassengerInsert passenger)
        {
            this.Id = string.Empty;
            this.Cpf = passenger.Cpf;
            this.Name = passenger.Name;
            this.Gender = passenger.Gender.ToUpper();
            this.Phone = passenger.Phone;
            this.DtRegister = DateTime.Now;
            this.Status = passenger.Status;
            this.Address = new() { ZipCode = passenger.Address.ZipCode, Number = passenger.Address.Number, Complement = passenger.Address.Complement };
        }
    }
}
