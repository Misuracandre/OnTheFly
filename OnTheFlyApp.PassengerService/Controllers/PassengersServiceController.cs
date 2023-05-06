using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnTheFly.Models;
using OnTheFlyApp.PassengerService.Service;

namespace OnTheFlyApp.PassengerService.Controllers
{
    //localhost:5000/Api/passenger
    [Route("api/[controller]")]
    [ApiController]
    public class PassengersServiceController : ControllerBase
    {
        private readonly PassengersService _passengerService;

        public PassengersServiceController(PassengersService passengerService)
        {
            _passengerService = passengerService;
        }

        [HttpGet]
        public ActionResult<List<Passenger>> Get() => _passengerService.GetAll();

        [HttpGet("{cpf:length(11)}")]
        public ActionResult<Passenger> GetByCpf(string cpf) => _passengerService.GetByCpf(cpf);

        [HttpPost]
        public ActionResult<Passenger> Create(Passenger passenger) => _passengerService.Create(passenger);

        [HttpPut("{cpf:length(11)}")]
        public ActionResult<Passenger> Update(string cpf, bool status)
        {
            var pas = _passengerService.Update(cpf, status);
            if (pas == null) return NotFound("Passageiro não encontrado");
            return Ok(pas.Value);
        }

        [HttpDelete("{cpf:length(11)}")]
        public IActionResult Delete(string cpf)
        {
            if (_passengerService.Delete(cpf) != 1)
                return NotFound("Registro não encontrado");

            return NoContent();
        }

    }
}


