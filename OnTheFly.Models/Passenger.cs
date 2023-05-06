using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnTheFly.Models
{
    public class Passenger
    {
        public string Cpf { get; set; }
        public string Name { get; set; }
        public char Gender { get; set; }
        public string? Phone { get; set; }
        public DateTime DtBirth { get; set; }
        public DateTime DtRegister { get; set; }
        public bool? Status { get; set; }
        public Address Address { get; set; }
    }
}
