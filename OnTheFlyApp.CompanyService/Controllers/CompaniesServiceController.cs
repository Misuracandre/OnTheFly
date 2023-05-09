using System.Data;
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
    //https://localhost:7219/api/CompaniesService
    [Route("api/[controller]")]
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
        public ActionResult<CompanyDTO> GetCnpj(string cnpj) => _companyService.GetByCompany(cnpj);

        [HttpPost]
        public ActionResult<CompanyDTO> Create(CompanyDTO companydto)
        {
            Company company = new(companydto);
            company.Cnpj = _util.JustDigits(company.Cnpj);


            if (_util.VerifyCnpj(company.Cnpj) == false)
                return BadRequest("Cnpj inválido");


            if (_companyService.GetByCompany(company.Cnpj) != null)
                return BadRequest("Companhia já cadastrada");


            if (company.NameOpt == "string" || company.NameOpt == string.Empty)
                { company.NameOpt = company.Name; }

            if (company.Name == "string" || company.Name == string.Empty)
                return BadRequest("Campo nome vazio");

            //Método consumindo api busca cep
            var newAddress = _util.GetAddress(company.Address.ZipCode).Result;

            if (newAddress == null || company.Address.Number == 0)
                return BadRequest("Endereço inválido");

            newAddress.Number = company.Address.Number;
            company.Address = newAddress;
            CompanyDTO companyReturn = new();

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
        public async Task<ActionResult<CompanyDTO>> Update(string cnpj, bool status)
        {
            if (_companyService.GetByCompany(cnpj) == null)
                throw new Exception("Companhia não encontrada");
            if (status == true && _companyService.GetByCompany(cnpj).Status == true)
                throw new Exception("Companhia já ativada");

            CompanyDTO companyReturn = new();
            try
            {
                companyReturn = await _companyService.Update(cnpj, status);
                if (companyReturn == null) throw new Exception("Companhia sem avião");
            }
            catch (Exception)
            {
                throw new Exception("");
            }

            return Ok(companyReturn);
        }

        [HttpDelete]
        public HttpStatusCode Delete(string cnpj)
        {
            var companyResult = _companyService.GetByCompany(cnpj);
            if (companyResult == null || companyResult.Status == false)
                throw new Exception("Não encontrada ou desativada");

            _companyService.Delete(cnpj);

            return HttpStatusCode.OK;
        }
    }
}