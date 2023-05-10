using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnTheFly.Models;
using OnTheFly.Models.Dto;
using OnTheFlyApp.AirCraftService.Service;
using Utility;

namespace OnTheFlyApp.AirCraftService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AirCraftsServiceController : ControllerBase
    {
        private readonly AirCraftsService _aircraftService;
        private readonly Util _util;

        public AirCraftsServiceController(AirCraftsService aircraftService)
        {
            _aircraftService = aircraftService;
        }

        [HttpGet]
        public ActionResult<List<AirCraftDTO>> Get() => _aircraftService.GetAll();

        [HttpGet("{rab}",Name = "GetByRab")]
        public ActionResult<AirCraftDTO> GetByRab(string rab)
        {
            var aircraft = _aircraftService.GetByRab(rab);

            if (aircraft == null)
            {
                return NotFound();
            }
            return aircraft;
        }

        [HttpGet("company/{cnpj}",Name = "GetByCompany")]
        public ActionResult<List<AirCraft>> GetByCompany(string cnpj)
        {
            var aircraft = _aircraftService.GetByCompany(cnpj);

            if (aircraft == null)
            {
                return NotFound();
            }
            return aircraft;
        }

        [HttpPut("{rab}")]
        public ActionResult<AirCraft> Update(string rab, DateTime dtLastFlight ) => _aircraftService.Update(rab, dtLastFlight);


        [HttpPost]
        public async Task<ActionResult<AirCraftDTO>> Create(AirCraftInsertDTO aircraft) => await _aircraftService.Create(aircraft);

        [HttpDelete("{rab}")]
        public IActionResult Delete(string rab)
        {
            var aircraft = _aircraftService.GetByRab(rab);

            if (aircraft == null)
            {
                return NotFound();
            }

            _aircraftService.Delete(rab);

            return NoContent();
        }
    }
}
