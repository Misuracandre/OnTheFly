using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnTheFly.Models
{
    public class Sale
    {
        public bool Reserved { get; set; }
        public bool Sold { get; set; }
        public Flight Flight { get; set; }
        public List<Passenger> Passengers { get; set; }
    }
}
