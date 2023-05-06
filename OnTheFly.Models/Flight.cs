using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnTheFly.Models
{
    public class Flight
    {
        public int Sales { get; set; }
        public DateTime Departure { get; set; }
        public bool Status { get; set; }
        public Airport Destiny { get; set; }
        public AirCraft Plane { get; set; }
    }
}
