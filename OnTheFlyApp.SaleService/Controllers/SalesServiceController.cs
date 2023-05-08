using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnTheFly.Models.Dto;
using OnTheFly.Models;
using OnTheFlyApp.SaleService.Services;

namespace OnTheFlyApp.SaleService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalesServiceController : ControllerBase
    {
        private readonly SalesService _saleService;

        public SalesServiceController(SalesService saleService)
        {
            _saleService = saleService;
        }

        [HttpGet]
        public ActionResult<List<Sale>> Get()
        {
            var p = _saleService.GetAll();
            if (p.Count == 0) return NotFound("Nenhum passageiro cadastrado");
            return p;
        }

        [HttpGet("{rab}")]
        public ActionResult<Sale> GetByIdentifier(string rab)
        {
            //var p = _saleService.GetByCpf(cpf);
            //if (p == null) return NotFound("Passageiro não encontrado");
            //return Ok(p);
            return _saleService.GetByIdentifier(rab);
        }

        [HttpPost]
        public ActionResult<SaleDTO> Create(SaleDTO sale)
        {
            var s = _saleService.Create(sale);
            if (s == null)
                return BadRequest("Passageiro não cadastrado");
            return Ok(s);
            return new SaleDTO();
        }

        [HttpPut("{rab}/{schedule}/{cpf}")]
        public ActionResult<SaleDTO> Update(string rab, DateTime schedule, string cpf, bool status)
        {
            //var pas = _saleService.Update(cpf, status);
            //if (pas == null) return NotFound("Passageiro não encontrado");
            //return Ok(pas.Value);

            return new SaleDTO();
        }

        [HttpDelete("{rab}/{schedule}/{cpf}")]
        public async Task<ActionResult> Delete(string rab, DateTime schedule, string cpf)
        {
            //if (await _saleService.Delete(cpf) != 1)
            //    return NotFound("Passageiro não deletado");

            return Ok("Passageiro deletado");
        }
    }
}
