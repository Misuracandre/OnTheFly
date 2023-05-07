using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using OnTheFly.Models;
using OnTheFlyApp.FlightService.Services;

namespace OnTheFlyApp.FlightService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightsController : ControllerBase
    {
        private readonly FlightsService _flightsService;

        public FlightsController(FlightsService flightsService)
        {
            _flightsService = flightsService;
        }

        [HttpGet]
        public ActionResult<List<Flight>> Get() => _flightsService.GetAll();

        [HttpGet("activated", Name = "GetActivated")]
        public ActionResult<List<Flight>> GetActivated() => _flightsService.GetActivated();

        [HttpGet("AirCraftAndDeparture", Name = "GetByAirCraftAndDeparture")]
        public ActionResult<Flight> GetByAirCraftAndDeparture(string rab, DateTime departure) => _flightsService.GetByAirCraftAndDeparture(rab, departure);

        [HttpPost]
        public Flight CreateFlight(Flight flight) => _flightsService.CreateFlight(flight);

        [HttpPut("{rab}/{departure}")]
        public IActionResult UpdateFlight(string rab, DateTime departure, bool status, Flight flight)
        {
            _flightsService.UpdateFlight(rab, departure, status, flight);

            return Ok();
        }

        [HttpDelete]
        public IActionResult DeleteFlight(string rab, DateTime departure)
        {
            var flight = _flightsService.GetByAirCraftAndDeparture(rab, departure);

            if (flight == null)
            {
                return NotFound();
            }

            _flightsService.DeleteFlight(rab, departure);

            return Ok();
        }

    }
}
