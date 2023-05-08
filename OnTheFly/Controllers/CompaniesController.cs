using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using OnTheFly.Models;
using OnTheFlyApp.Services;

namespace OnTheFlyApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private readonly CompanieServices _companyService;
        public CompaniesController()
        {
            _companyService = new CompanieServices();
        }

        [HttpGet]
        public async Task<IEnumerable<Company>> FindAll() => await _companyService.FindAll();

        [HttpGet("activated")]
        public async Task<IEnumerable<Company>> FindActivated() => await _companyService.FindActivated();

        [HttpGet("cnpj")]
        public async Task<Company> FindCnpj(string cnpj) => await _companyService.FindCnpj(cnpj);

        [HttpPost]
        public async Task<Company> PostCompany(Company company) => await _companyService.Insert(company);

        [HttpDelete]
        public async Task<HttpStatusCode> Delete(string cnpj) => await _companyService.Delete(cnpj);

        [HttpPut]
        public async Task<HttpStatusCode> Update(string cnpj, bool status) => await _companyService.Update(cnpj, status);

    }
}
