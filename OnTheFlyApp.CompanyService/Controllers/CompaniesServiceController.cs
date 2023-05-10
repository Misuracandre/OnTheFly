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
        public ActionResult<List<CompanyDTO>> GetAll() => _companyService.GetAll();

        [HttpGet("disable/", Name = "GetDisable")]
        public ActionResult<List<CompanyDTO>> GetDisable() => _companyService.GetDisable();

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
                var companyReturn = _companyService.GetByCompanyRestricted(cnpj);
                if (companyReturn.Cnpj != null) { return Ok("RESTRICTED"); }
                return NotFound();
            }
        }

        [HttpGet("restricted/", Name = "GetRestricted")]
        public ActionResult<List<CompanyGetDTO>> GetRestricted() => _companyService.GetRestricted();

        [HttpPost]
        public ActionResult<CompanyDTO> Create(CompanyDTO companydto)
        {
            companydto.Cnpj = _util.JustDigits(companydto.Cnpj);

            if (_util.VerifyCnpj(companydto.Cnpj) == false)
                return BadRequest("Cnpj inválido");

            if (_companyService.GetByCompany(companydto.Cnpj) != null)
                return BadRequest("Cnpj já está registrado");

            if (companydto.NameOpt == "string" || companydto.NameOpt == string.Empty) { companydto.NameOpt = companydto.Name; }

            if (companydto.Name == "string" || companydto.Name == string.Empty)
                return BadRequest("Campo nome vazio");

            if (!DateTime.TryParse(companydto.DtOpen, out DateTime dBirth))
                return BadRequest("Data de registro inválida -> dd/mm/yyyy");

            Company company = new(companydto);

            //Método consumindo api busca cep
            var newAddress = _util.GetAddress(company.Address.ZipCode).Result;

            if (newAddress == null || newAddress.ZipCode == null || company.Address.Number == 0)
                return BadRequest("Endereço inválido");

            newAddress.Number = company.Address.Number;
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

        [HttpPut("{cnpj}")]
        public async Task<ActionResult<CompanyGetDTO>> Update(string cnpj)
        {
            var companyStatus = _companyService.GetByCompany(cnpj);

            if (companyStatus == null)
                return NotFound("Companhia não encontrada");

            CompanyDTO companyReturn = new();
            try
            {
                if (companyStatus.Status == true) { companyReturn = await _companyService.Update(cnpj, false); }

                else companyReturn = await _companyService.Update(cnpj, true);
            }
            catch (Exception)
            {
                throw new Exception("");
            }

            return Ok(companyReturn);
        }

        [HttpPut("restricted/{cnpj}")]
        public async Task<ActionResult> UpdateRestricted(string cnpj)
        {
            CompanyGetDTO company = new();
            try
            {
                company =  _companyService.GetByCompany(cnpj);
                _companyService.UpdateRestricted(company, cnpj, true);
                return Ok("Companhia foi restrita");
            }
            catch (Exception)
            {
                try
                {
                    _companyService.UpdateRestricted(company, cnpj, false);
                    return Ok("Companhia sem restrição");
                }
                catch (Exception) { return NotFound("Companhia não foi encontrada"); }
            }
        }

        [HttpDelete]
        public HttpStatusCode Delete(string cnpj)
        {
            var companyResult = _companyService.GetByCompany(cnpj);
            if (companyResult == null)
                return BadRequest("Não encontrada");
            if (companyResult.Status == true)
                return BadRequest("A companhia precisa estar desativada");

            _companyService.Delete(cnpj);

            return HttpStatusCode.OK;
        }
    }
}