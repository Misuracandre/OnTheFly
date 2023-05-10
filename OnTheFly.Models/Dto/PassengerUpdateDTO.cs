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
    public class PassengerUpdateDTO
    {

        [StringLength(30)]
        public string Name { get; set; }

        [StringLength(1)]
        public string Gender { get; set; }

        [StringLength(14)]
        public string? Phone { get; set; }
        public string DtBirth { get; set; }

        public AddressUpdateDTO Address { get; set; }

        public PassengerUpdateDTO() { }

        public PassengerUpdateDTO(Passenger passenger)
        {
            this.Name = passenger.Name;
            this.Gender = passenger.Gender.ToUpper();
            this.Phone = passenger.Phone;
            this.DtBirth = passenger.DtBirth.ToShortDateString();
            this.Address = new()
            {
                ZipCode = passenger.Address.ZipCode,
                Street = passenger.Address.Street,
                Number = passenger.Address.Number,
                Complement = passenger.Address.Complement,
            };
        }
    }
}
