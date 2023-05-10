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

        [HttpGet("{cpf}")]
        public ActionResult<PassengerDTO> GetByCpf(string cpf) =>  _passengerService.GetByCpf(cpf);

        [HttpPost]
        public ActionResult<PassengerDTO> Create(PassengerInsert passenger) => _passengerService.Create(passenger);

        [HttpPut("{cpf}")]
        public ActionResult<PassengerDTO> Update(string cpf, PassengerDTO passenger) => _passengerService.Update(cpf, passenger);

        [HttpDelete("{cpf}")]
        public async Task<ActionResult> Delete(string cpf) => await _passengerService.Delete(cpf);

        [HttpPatch("disable/{cpf}", Name = "disablePassenger")]
        public async Task<ActionResult> Disable(string cpf) => await _passengerService.Disable(cpf);

        [HttpPatch("restrict/{cpf}", Name = "restrictPassenger")]
        public async Task<ActionResult> Restrict(string cpf) => await _passengerService.Restrict(cpf);
    }
}


