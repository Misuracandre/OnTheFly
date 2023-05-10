using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnTheFly.Models.Dto
{
    public class FlightInsertDTO
    {
        public int Sales { get; set; }
        public DateTime Schedule { get; set; }
        public bool Status { get; set; }
        public Airport Arrival { get; set; }
        public AirCraftDTO Plane { get; set; }

        public FlightInsertDTO() { }

        public FlightInsertDTO(Flight flight)
        {
            this.Sales = flight.Sales;
            this.Schedule = flight.Schedule;
            this.Status = flight.Status;
            this.Arrival = flight.Arrival;
            this.Plane = new AirCraftDTO(flight.Plane);

        }
    }
}
