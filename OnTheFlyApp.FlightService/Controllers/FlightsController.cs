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

        [HttpGet("AirCraftAndSchedule", Name = "GetByAirCraftAndSchedule")]
        public ActionResult<Flight> GetFlightByRabAndSchedule(string rab, DateTime schedule) => _flightsService.GetFlightByRabAndSchedule(rab, schedule);

        [HttpPost]
        public async Task<Flight> CreateFlight(Flight flight) => await _flightsService.CreateFlight(flight);

        [HttpPut("{rab}/{schedule}")]
        public IActionResult UpdateFlight(string rab, DateTime schedule, bool status)
        {
            _flightsService.UpdateFlight(rab, schedule, status);

            return Ok();
        }

        [HttpDelete("{rab}/{schedule}", Name = "DeleteFlightByRabAndSchedule")]
        public async Task<IActionResult> DeleteFlight(string rab, DateTime schedule)
        {
            var flight = _flightsService.GetFlightByRabAndSchedule(rab, schedule);

            if (flight == null)
            {
                return NotFound();
            }

            await _flightsService.DeleteFlight(rab, schedule);

            return Ok();
        }

    }
}
