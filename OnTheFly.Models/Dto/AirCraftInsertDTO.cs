using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnTheFly.Models.Dto
{
    public class AirCraftInsertDTO
    {
        public string Rab { get; set; }
        public int Capacity { get; set; }
        public DateTime DtRegistry { get; set; }
        public DateTime? DtLastFlight { get; set; }
        public string Company { get; set; }

        public AirCraftInsertDTO() { }
    }
}
