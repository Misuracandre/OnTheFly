using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using OnTheFly.Models;
using OnTheFly.Models.Dto;
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

        [HttpGet("GetAll")]
        public ActionResult<List<FlightGetDTO>> Get() => _flightsService.GetAll();

        [HttpGet("GetDeleted")]
        public ActionResult<List<Flight>> GetDeleted() => _flightsService.GetDeleted();

        [HttpGet("GetByIdentifier/{rab}/{schedule}", Name = "GetByRabAndSchedule")]
        public async Task<ActionResult<FlightGetDTO>> GetFlightByRabAndSchedule(string rab, DateTime schedule) => await _flightsService.GetFlightByRabAndSchedule(rab, schedule);

        [HttpPost("CreateFlight")]
        public async Task <ActionResult<FlightDTO>> CreateFlight(FlightInsertDTO flightInsertDTO) => await _flightsService.CreateFlight(flightInsertDTO);

        [HttpPut("UpdateStatus/{rab}/{schedule}", Name = "UpdateStatus")]
        public IActionResult UpdateFlight(string rab, DateTime schedule, bool status)
        {
            _flightsService.UpdateFlight(rab, schedule, status);

            return Ok();
        }

        [HttpDelete("DeleteFlight/{rab}/{schedule}", Name = "DeleteFlightByRabAndSchedule")]
        public async Task<IActionResult> DeleteFlight(string rab, DateTime schedule)
        {
            var flight = _flightsService.GetFlightByRabAndSchedule(rab, schedule);

            if (flight == null)
            {
                return new ContentResult() { Content = "Voo não encontrado.", StatusCode = StatusCodes.Status404NotFound };
            }

            return await _flightsService.DeleteFlight(rab, schedule);           
        }

    }
}
