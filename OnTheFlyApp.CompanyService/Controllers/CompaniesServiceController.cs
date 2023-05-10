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

        [HttpGet("getall/activated/", Name = "GetAllActivated")]
        public ActionResult<List<CompanyGetDTO>> GetAll()
        {
            List<CompanyGetDTO> lstReturn = _companyService.GetAll();
            if (lstReturn.Count == 0) { return NotFound("Companhias ativas não encontradas"); }

            return lstReturn;
        }

        [HttpGet("getall/disabled/", Name = "GetDisable")]
        public ActionResult<List<CompanyGetDTO>> GetDisable()
        {
            List<CompanyGetDTO> lstReturn = _companyService.GetDisable();
            if (lstReturn.Count == 0) { return NotFound("Companhias desativadas não encontradas"); }

            return lstReturn;
        }

        [HttpGet("get/company/{cnpj}", Name = "GetCnpj")]
        public ActionResult<CompanyGetDTO> GetCnpj(string cnpj)
        {
            cnpj = _util.JustDigits(cnpj);
            var companyreturn = _companyService.GetByCompany(cnpj);
            if(companyreturn == null) { companyreturn = _companyService.GetByCompanyRestricted(cnpj).Result;
                if (companyreturn != null) { return Ok("RESTRICTED"); }
            }

            return companyreturn == null ? NotFound("Companhia não encontrada") : companyreturn;
        }

        [HttpGet("getall/restricted/", Name = "GetRestricted")]
        public ActionResult<List<CompanyGetDTO>> GetRestricted()
        {
            List<CompanyGetDTO> lstReturn = _companyService.GetRestricted();
            if (lstReturn.Count == 0) { return NotFound("Companhias restritas não encontradas"); }

            return lstReturn;
        }

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

        [HttpPut("patch/{cnpj}", Name = "Status")]
        public async Task<ActionResult<CompanyGetDTO>> Update(string cnpj)
        {
            var companyStatus = _companyService.GetByCompany(cnpj);

            if (companyStatus == null) { companyStatus = await _companyService.GetByCompanyRestricted(cnpj); }
            if (companyStatus.Cnpj == null) return NotFound("Companhia não encontrada");

            CompanyGetDTO companyReturn = new();

            if (companyStatus.Status == true)
            {
                companyReturn = _companyService.Update(cnpj, false).Result;
            }
            else
            {
                companyReturn = _companyService.Update(cnpj, true).Result;
            }

            if (companyReturn == null) { return BadRequest("Companhia não possui avião"); }

            return Ok(companyReturn);
        }

        [HttpPut("patch/restricted/{cnpj}")]
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
