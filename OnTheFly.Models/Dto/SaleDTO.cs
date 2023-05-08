using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnTheFly.Models.Dto;

namespace OnTheFly.Models.Dto
{
    public class SaleDTO
    {
        public bool Reserved { get; set; }
        public bool Sold { get; set; }
        public FlightDTO Flight { get; set; }
        public List<PassengerDTO> Passengers { get; set; }
    }
}
