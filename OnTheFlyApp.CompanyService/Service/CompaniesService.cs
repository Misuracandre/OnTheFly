using System.Net;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using OnTheFly.Models;
using OnTheFly.Models.Dto;
using OnTheFlyApp.CompanyService.Config;

namespace OnTheFlyApp.CompanyService.Service
{
    public class CompaniesService
    {
        private readonly IMongoCollection<Company> _company;
        private readonly IMongoCollection<Company> _companyDisabled;
        private readonly IMongoCollection<Address> _address;
        static readonly HttpClient companyClient = new HttpClient();
        static readonly string endpointAirCraft = "https://localhost:7117/api/AirCraftsService/company/";
        public CompaniesService() { }
        public CompaniesService(ICompanyServiceSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _company = database.GetCollection<Company>(settings.CompanyCollection);
            _companyDisabled = database.GetCollection<Company>(settings.CompanyDisabledCollection);
            _address = database.GetCollection<Address>(settings.CompanyAddressCollection);
        }

        public List<CompanyDTO> GetAll()
        {
            List<Company> companieslst = new();
            companieslst = _company.Find(c => true).ToList();

            List<CompanyDTO> lstReturn = new();
            foreach (var company in companieslst) { CompanyDTO companyDTO = new(); lstReturn.Add(companyDTO = new(company)); }

            return lstReturn;
        }

        public List<CompanyDTO> GetDisable()
        {
            List<Company> lstDisable = _companyDisabled.Find(c => true).ToList();
            if (lstDisable == null) return null;

            List<CompanyDTO> lstReturn = new();
            foreach (var company in lstDisable)
            {
                CompanyDTO companyDTO = new();
                lstReturn.Add(companyDTO = new(company));
            }

            return lstReturn;
        }

        public CompanyDTO GetByCompany(string cnpj)
        {
            CompanyDTO companyReturn = new();

            var company = _company.Find<Company>(c => c.Cnpj == cnpj).FirstOrDefault();
            if (company == null) { company = _companyDisabled.Find<Company>(c => c.Cnpj == cnpj).FirstOrDefault(); }
            if (company == null) return null;

            companyReturn = new(company);

            return companyReturn;
        }

        public Company Create(Company company)
        {
            if (_address.Find(a => a.Number == company.Address.Number && a.ZipCode == company.Address.ZipCode).FirstOrDefault() != null)
                throw new Exception();
            company.Status = true;
            _address.InsertOne(company.Address);
            _company.InsertOne(company);

            return company;
        }

        public async Task<CompanyDTO> Update(string cnpj, bool status)
        {
            if (status == true)
            {
                var company = _companyDisabled.Find(c => c.Cnpj == cnpj).FirstOrDefaultAsync().Result;
                company.Status = status;
                CompanyDTO companyTrue = new(company);
                _companyDisabled.DeleteOne(c => c.Cnpj == cnpj);
                _company.InsertOne(company);
                HttpResponseMessage responseAirCraft = await CompaniesService.companyClient.GetAsync(endpointAirCraft + cnpj);
                if (responseAirCraft.StatusCode.ToString().Equals("400"))
                    return null;

                return companyTrue;
            }
            try
            {
                var insertDisabled = _company.Find(c => c.Cnpj == cnpj).FirstOrDefaultAsync().Result;
                _company.DeleteOne(c => c.Cnpj == cnpj);
                insertDisabled.Status = status;
                await _companyDisabled.InsertOneAsync(insertDisabled);

                CompanyDTO companyFalse = new(insertDisabled);

                return companyFalse;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Delete(string cnpj)
        {
            var insertDisable = _company.Find(c => c.Cnpj == cnpj).FirstOrDefaultAsync().Result;
            _address.DeleteOne(c => c.Number == insertDisable.Address.Number && c.ZipCode == insertDisable.Address.ZipCode);
            insertDisable.Status = false;
            _companyDisabled.InsertOne(insertDisable);
            _company.DeleteOne(c => c.Cnpj == cnpj);
        }
    }
}
