using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnTheFly.Models
{
    public class Company
    {
        public string Cnpj { get; set; }
        public string Name { get; set; }
        public string NameOpt { get; set; }
        public DateOnly DtOpen { get; set; }
        public bool? Status { get; set; }
        public Address Address { get; set; }
    }
}
