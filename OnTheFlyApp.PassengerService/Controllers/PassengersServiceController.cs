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

        [HttpPost]
        public ActionResult<Passenger> Create(Passenger passenger) => _passengerService.Create(passenger);

        [HttpDelete("{cpf:length(11)}")]
        public IActionResult Delete(string cpf)
        {
            var passenger = _passengerService.GetByCpf(cpf);

            if (passenger == null)
            {
                return NotFound();
            }

            _passengerService.Delete(cpf);

            return NoContent();
        }

    }
}


