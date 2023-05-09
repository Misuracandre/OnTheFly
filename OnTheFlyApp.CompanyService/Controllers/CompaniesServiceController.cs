using System.Data;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using OnTheFly.Models;
using OnTheFly.Models.Dto;
using OnTheFlyApp.CompanyService.Service;
using OnTheFlyApp.Services;
using Utility;

namespace OnTheFlyApp.CompanyService.Controllers
{
    //https://localhost:7219/api/CompaniesService
    [Route("api/[controller]")]
    [ApiController]
    public class CompaniesServiceController : ControllerBase
    {
        private readonly CompaniesService _companyService;
        static readonly PostOfficesService _addressCep = new PostOfficesService();
        private readonly Util _util;
        public CompaniesServiceController(CompaniesService companyService, Util util)
        {
            _companyService = companyService;
            _util = util;
        }

        [HttpGet(Name = "GetAll")]
        public ActionResult<List<CompanyDTO>> GetAll() => _companyService.GetAll();

        [HttpGet("activated/", Name = "GetActivated")]
        public ActionResult<List<CompanyDTO>> GetActivated() => _companyService.GetActivated();



        [HttpGet("disable/", Name = "GetDisable")]
        public ActionResult<List<CompanyDTO>> GetDisable() => _companyService.GetDisable();


        [HttpGet("cnpj/", Name = "GetCnpj")]
        public ActionResult<CompanyDTO> GetCnpj(string cnpj) => _companyService.GetByCompany(cnpj);

        [HttpPost]
        public ActionResult<CompanyDTO> Create(CompanyDTO companydto)
        {
            Company company = new(companydto);
            company.Id = "";
            company.Cnpj = _util.JustDigits(company.Cnpj);

            try
            {
                if (_util.VerifyCnpj(company.Cnpj) == false)
                    throw new Exception();
            }
            catch (Exception) { return BadRequest("Cnpj inválido"); }

            try
            {
                if (_companyService.GetByCompany(company.Cnpj) != null)
                    throw new Exception();
            }
            catch (Exception) { return BadRequest("Companhia já cadastrada"); }

            if (company.NameOpt == "string" || company.NameOpt == string.Empty)
                { company.NameOpt = company.Name; }

            try
            {
                if (company.Name == "string" || company.Name == string.Empty)
                    throw new Exception();
            }
            catch (Exception) { return BadRequest("Campo nome vazio"); }

            var newAddress = _addressCep.GetAddress(company.Address.ZipCode).Result;

            try
            {
                if (newAddress == null || company.Address.Number == 0)
                    throw new Exception();
            }
            catch (Exception) { return BadRequest("Endereço inválido"); }

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


            if (_companyService.GetByCompany(cnpj) == null) throw new Exception("Companhia não encontrada");


            CompanyDTO companyReturn = new();
            try
            {
                companyReturn = new(await _companyService.Update(cnpj, status));
            }
            catch (Exception)
            {
                throw new Exception("Companhia sem avião");
            }

            return Ok(companyReturn);
        }

        [HttpDelete]
        public HttpStatusCode Delete(string cnpj)
        {
            if (_companyService.GetByCompany(cnpj) == null)
                throw new Exception("Não encontrada");

            _companyService.Delete(cnpj);

            return HttpStatusCode.OK;
        }
    }
}