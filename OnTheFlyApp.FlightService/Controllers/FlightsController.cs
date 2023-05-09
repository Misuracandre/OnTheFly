﻿using Microsoft.AspNetCore.Http;
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

        [HttpGet]
        public List<FlightDTO> Get() => _flightsService.GetAll();

        [HttpGet("activated", Name = "GetActivated")]
        public List<Flight> GetDisabled() => _flightsService.GetDisabled();

        [HttpGet("{rab}/{schedule}", Name = "GetByAirCraftAndSchedule")]
        public async Task<ActionResult<FlightDTO>> GetFlightByRabAndSchedule(string rab, DateTime schedule) => await _flightsService.GetFlightByRabAndSchedule(rab, schedule);


        //[HttpGet("AirCraftAndSchedule", Name = "GetByAirCraftAndSchedule")]
        //public async Task<ActionResult<FlightDTO>> GetFlightByRabAndSchedule(string rab, DateTime schedule) => await _flightsService.GetFlightByRabAndSchedule(rab, schedule);

        [HttpPost]
        public async Task<FlightDTO> CreateFlight(Flight flight) => await _flightsService.CreateFlight(flight);

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
