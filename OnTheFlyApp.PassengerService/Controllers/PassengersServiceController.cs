using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnTheFly.Models;
using OnTheFly.Models.Dto;
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
        public ActionResult<List<PassengerDTO>> Get()
        {
            var p = _passengerService.GetAll();
            if (p.Count == 0) return NotFound("Nenhum passageiro cadastrado");
            return p;
        }

        [HttpGet("{cpf:length(11)}")]
        public ActionResult<PassengerDTO> GetByCpf(string cpf)
        {
            var p = _passengerService.GetByCpf(cpf);
            if(p == null) return NotFound("Passageiro não encontrado");
            return Ok(p);
        }

        [HttpPost]
        public ActionResult<PassengerDTO> Create(PassengerInsert passenger)
        {
            var p = _passengerService.Create(passenger);
            if (p == null)
                return BadRequest("Passageiro não cadastrado");
            return Ok(p);
        }

        [HttpPut("{cpf:length(11)}")]
        public ActionResult<Passenger> Update(string cpf, bool status)
        {
            var pas = _passengerService.Update(cpf, status);
            if (pas == null) return NotFound("Passageiro não encontrado");
            return Ok(pas.Value);
        }

        [HttpDelete("{cpf:length(11)}")]
        public async Task<ActionResult> Delete(string cpf)
        {
            if (await _passengerService.Delete(cpf) != 1)
                return NotFound("Passageiro não deletado");

            return Ok("Passageiro deletado");
        }

    }
}


