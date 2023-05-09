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
            if (p.Count == 0) return NotFound("Nenhuma venda cadastrado");
            return p;
        }

        [HttpGet("{rab}/{schedule}/{cpf}")]
        public ActionResult<Sale> GetByIdentifier(string rab, DateTime schedule, string cpf)
        {
            var s = _saleService.GetByIdentifier(rab, schedule, cpf);
            if (s == null) return NotFound("Venda não encontrada");
            return Ok(s);
        }

        [HttpPost]
        public async Task<ActionResult> Create(SaleDTO sale) => await _saleService.Create(sale);


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

            return Ok("Venda deletada");
        }
    }
}
