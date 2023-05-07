using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnTheFly.Models;
using OnTheFlyApp.Services;

namespace OnTheFlyApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AirCraftsController : ControllerBase
    {
        private readonly AirCraftService _aircraftService;

        public AirCraftsController()
        {
            _aircraftService = new AirCraftService();
        }

        [HttpGet]
        public async Task<ActionResult<List<AirCraft>>> Get() => await _aircraftService.FindAll();

        [HttpGet("{rab}")]
        public ActionResult<AirCraft> GetByRab(string rab) => new AirCraft();

        [HttpPost]
        public ActionResult<AirCraft> Create(AirCraft aircraft) => new AirCraft();

        [HttpPut("{rab}")]
        public ActionResult<AirCraft> Update(string rab, DateTime dtLastFlight)
        {

            return new AirCraft();
        }

        [HttpDelete("{rab}")]
        public IActionResult Delete(string rab)
        {
            return NoContent();
        }
    }
}
