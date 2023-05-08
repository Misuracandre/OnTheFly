using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnTheFly.Models;
using OnTheFlyApp.Services;

namespace OnTheFlyApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PassangersController : ControllerBase
    {
        private readonly PassengerService _passengerService;

        public PassangersController()
        {
            _passengerService = new PassengerService();
        }

        [HttpGet]
        public async Task<ActionResult<List<Passenger>>> Get() => await _passengerService.FindAll();

        [HttpGet("{cpf:length(11)}")]
        public async Task<ActionResult<Passenger>> GetByCpf(string cpf) => await _passengerService.FindByCpf(cpf);

        [HttpPost]
        public ActionResult<Passenger> Create(Passenger passenger) => new Passenger();

        [HttpPut("{cpf:length(11)}")]
        public ActionResult<Passenger> Update(string cpf, bool status)
        {
            
            return new Passenger();
        }

        [HttpDelete("{cpf:length(11)}")]
        public async Task<ActionResult> Delete(string cpf)
        {
            if (await _passengerService.Delete(cpf) == null) return NotFound("Registro não encontrado para deletar");
            return NoContent();
        }
    }
}
