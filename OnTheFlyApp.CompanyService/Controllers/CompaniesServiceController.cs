using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using OnTheFly.Models;
using OnTheFlyApp.CompanyService.Service;

namespace OnTheFlyApp.CompanyService.Controllers
{
    //https://localhost:7219/api/CompaniesService
    [Route("api/[controller]")]
    [ApiController]
    public class CompaniesServiceController : ControllerBase
    {
        private readonly CompaniesService _companyService;

        public CompaniesServiceController(CompaniesService companyService)
        {
            _companyService = companyService;
        }

        [HttpGet(Name = "GetAll")]
        public ActionResult<List<Company>> GetAll() => _companyService.GetAll();

        [HttpGet("activated",Name ="GetActivated")]
        public ActionResult<List<Company>> GetActiveted() => _companyService.GetActiveted();

        [HttpGet("cnpj", Name = "GetCpnj")]
        public ActionResult<Company> GetActiveted(string cnpj) => _companyService.GetByCompany(cnpj);

        [HttpPost]
        public ActionResult<Company> Create(Company company)
        {
            company.Id = "";
            _companyService.Create(company);

            return company;
        }

        [HttpPut("{cnpj}")]
        public IActionResult Update(string cnpj, bool status)
        {
            var comp = _companyService.GetByCompany(cnpj); 

            if (comp == null)
            {
                return NotFound();
            }
            comp = _companyService.Update(cnpj, status);

            return Ok(comp);
        }

        [HttpDelete]
        public IActionResult Delete(string cnpj)
        {
            var company = _companyService.GetByCompany(cnpj);

            if (company == null)
            {
                return NotFound();
            }

            _companyService.Delete(cnpj);

            return Ok();
        }
    }
}
