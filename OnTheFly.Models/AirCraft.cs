using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnTheFly.Models
{
    public class AirCraft
    {
        public string Rab { get; set; }
        public int Capacity { get; set; }
        public DateOnly DtRegistry { get; set; }
        public DateOnly? DtLastFlight { get; set; }
        public Company Company { get; set; }
    }
}
