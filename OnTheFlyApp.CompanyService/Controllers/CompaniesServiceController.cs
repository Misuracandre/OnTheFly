using System.Data;
using System.Globalization;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using OnTheFly.Models;
using OnTheFly.Models.Dto;
using OnTheFlyApp.CompanyService.Service;
using Utility;

namespace OnTheFlyApp.CompanyService.Controllers
{
    [Route("api/CompanyService")]
    [ApiController]
    public class CompaniesServiceController : ControllerBase
    {
        private readonly CompaniesService _companyService;
        private readonly Util _util;
        public CompaniesServiceController(CompaniesService companyService, Util util)
        {
            _companyService = companyService;
            _util = util;
        }

        [HttpGet(Name = "GetAll")]
        public ActionResult<List<CompanyGetDTO>> GetAll() => _companyService.GetAll();

        [HttpGet("disable/", Name = "GetDisable")]
        public ActionResult<List<CompanyGetDTO>> GetDisable() => _companyService.GetDisable();

        [HttpGet("cnpj/", Name = "GetCnpj")]
        public ActionResult<CompanyGetDTO> GetCnpj(string cnpj)
        {
            try
            {
                var companyreturn = _companyService.GetByCompany(cnpj);
                return companyreturn;
            }
            catch (Exception)
            {
                var companyReturn = _companyService.GetByCompanyRestricted(cnpj).Result;
                if (companyReturn.Cnpj != null) { return Ok("RESTRICTED"); }
                return NotFound();
            }
        }

        [HttpGet("restricted/", Name = "GetRestricted")]
        public ActionResult<List<CompanyGetDTO>> GetRestricted() => _companyService.GetRestricted();

        [HttpPost]
        public ActionResult<CompanyGetDTO> Create(CompanyGetDTO companydto)
        {
            companydto.Cnpj = _util.JustDigits(companydto.Cnpj);

            if (_util.VerifyCnpj(companydto.Cnpj) == false)
                return BadRequest("Cnpj inválido");

            if (_companyService.GetByCompany(companydto.Cnpj) != null)
            {
                return BadRequest("Cnpj já está registrado");
            }
            else if (_companyService.GetByCompanyRestricted(companydto.Cnpj).Result != null)
            { return BadRequest("Cnpj já está registrado"); }

            if (_companyService.GetByCompanyRestricted(companydto.Cnpj).Result != null)
            { return BadRequest("Cnpj está cadastrado e restrito"); }

            if (companydto.NameOpt == "string" || companydto.NameOpt == string.Empty) { companydto.NameOpt = companydto.Name; }

            if (companydto.Name == "string" || companydto.Name == string.Empty)
                return BadRequest("Campo nome vazio");

            Company company = new(companydto);
            if (!DateTime.TryParse(companydto.DtOpen, out DateTime dateCmp))
                return BadRequest("Data de registro inválida -> dd/mm/yyyy");

            company.DtOpen = dateCmp;

            //Método consumindo api busca cep
            var newAddress = _util.GetAddress(company.Address.ZipCode).Result;

            if (newAddress == null || newAddress.ZipCode == null || company.Address.Number == 0)
                return BadRequest("Endereço inválido");

            newAddress.Number = companydto.Address.Number;
            newAddress.Complement = companydto.Address.Complement;
            company.Address = new(newAddress);
            CompanyGetDTO companyReturn = new();

            try
            {
                companyReturn = new(_companyService.Create(company));
            }
            catch (Exception)
            {
                return BadRequest("Endereço já cadastrado");
            }
            return companyReturn;
        }

        [HttpPut("cnpj/", Name = "Status")]
        public async Task<ActionResult<CompanyGetDTO>> Update(string cnpj)
        {
            var companyStatus = _companyService.GetByCompany(cnpj);

            if (companyStatus == null) { companyStatus = await _companyService.GetByCompanyRestricted(cnpj); }
            if (companyStatus.Cnpj == null) return NotFound("Companhia não encontrada");

            CompanyGetDTO companyReturn = new();

            if (companyStatus.Status == true)
            {
                companyReturn = await _companyService.Update(cnpj, false);
            }
            else
            {
                companyReturn = await _companyService.Update(cnpj, true);
            }

            if (companyReturn == null) { return BadRequest("Companhia não possui avião"); }

            return Ok(companyReturn);
        }

        [HttpPut("restricted/")]
        public async Task<ActionResult> UpdateRestricted(string cnpj)
        {
            CompanyGetDTO company = new();

            company = _companyService.GetByCompany(cnpj);
            if (company != null)
            {
                _companyService.UpdateRestricted(company, cnpj, true);
                return Ok("Companhia foi restrita");
            }
            else
            {
                company = await _companyService.GetByCompanyRestricted(cnpj);
                if (company != null)
                {
                    _companyService.UpdateRestricted(company, cnpj, false);
                    return Ok("COmpanhia sem restrição");
                }
                return NotFound("Companhia não foi encontrada");
            }
        }

        [HttpDelete]
        public HttpStatusCode Delete(string cnpj)
        {
            var companyResult = _companyService.GetByCompany(cnpj);
            if (companyResult == null)
            {
                companyResult = _companyService.GetByCompanyRestricted(cnpj).Result;
                if (companyResult == null) return HttpStatusCode.NotFound;
            }

            _companyService.Delete(cnpj);

            return HttpStatusCode.OK;
        }
    }
}
