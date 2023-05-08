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
    public class PassengerInsert
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


        public AddressInsert Address { get; set; }
    }
}
