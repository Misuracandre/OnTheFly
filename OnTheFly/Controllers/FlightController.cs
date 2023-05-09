using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnTheFly.Models;
using OnTheFlyApp.Services;

namespace OnTheFlyApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightController : ControllerBase
    {
        private readonly FlightService _flightService;

        public FlightController()
        {
            _flightService = new FlightService();
        }

        [HttpGet]
        public async Task<ActionResult<List<Flight>>> Get() => await _flightService.FindAll();

        [HttpPost]
        public ActionResult<Flight> Create(Flight flight) => new Flight();

        [HttpPut]
        public ActionResult<Flight> Update(string rab, DateTime departure)
        {
            return new Flight();
        }

        [HttpDelete]
        public IActionResult Delete(string rab, DateTime departure) 
        {
            return NoContent();
        }
    }
}
