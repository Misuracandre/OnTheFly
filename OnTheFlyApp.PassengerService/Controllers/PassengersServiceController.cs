using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnTheFly.Models;
using OnTheFly.Models.Dto;
using OnTheFlyApp.PassengerService.Service;
using Utility;

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
        public ActionResult<List<PassengerDTO>> Get() => _passengerService.GetAll();

        [HttpGet("{cpf:length(11)}")]
        public ActionResult<PassengerDTO> GetByCpf(string cpf) =>  _passengerService.GetByCpf(cpf);

        [HttpPost]
        public ActionResult<PassengerDTO> Create(PassengerInsert passenger) => _passengerService.Create(passenger);

        [HttpPut("{cpf:length(11)}")]
        public ActionResult<PassengerDTO> Update(string cpf, PassengerDTO passenger) => _passengerService.Update(cpf, passenger);

        [HttpDelete("{cpf:length(11)}")]
        public async Task<ActionResult> Delete(string cpf) => await _passengerService.Delete(cpf);
    }
}


