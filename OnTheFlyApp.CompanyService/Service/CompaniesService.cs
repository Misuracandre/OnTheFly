using System.Net;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using OnTheFly.Models;
using OnTheFly.Models.Dto;
using OnTheFlyApp.CompanyService.Config;
using ZstdSharp;

namespace OnTheFlyApp.CompanyService.Service
{
    public class CompaniesService
    {
        private readonly IMongoCollection<Company> _company;
        private readonly IMongoCollection<Company> _companyRestricted;
        private readonly IMongoCollection<Company> _companyDeleted;
        private readonly IMongoCollection<Company> _companyDisabled;
        private readonly IMongoCollection<Address> _address;
        static readonly HttpClient companyClient = new HttpClient();
        static readonly string endpointAirCraft = "https://localhost:5002/api/AirCraftsService/cnpjCompany?cnpj=";
        public CompaniesService() { }
        public CompaniesService(ICompanyServiceSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _company = database.GetCollection<Company>(settings.CompanyCollection);
            _companyDisabled = database.GetCollection<Company>(settings.CompanyDisabledCollection);
            _address = database.GetCollection<Address>(settings.CompanyAddressCollection);
            _companyRestricted = database.GetCollection<Company>(settings.CompanyRestrictedCollection);
            _companyDeleted = database.GetCollection<Company>(settings.CompanyDeletedCollection);
        }

        public List<CompanyGetDTO> GetAll()
        {
            List<Company> companieslst = new();
            companieslst = _company.Find(c => true).ToList();

            List<CompanyGetDTO> lstReturn = new();
            foreach (var company in companieslst) { CompanyGetDTO companyDTO = new(); lstReturn.Add(companyDTO = new(company)); }

            return lstReturn;
        }

        public List<CompanyGetDTO> GetDisable()
        {
            List<Company> lstDisable = _companyDisabled.Find(c => true).ToList();
            if (lstDisable == null) return null;

            List<CompanyGetDTO> lstReturn = new();
            foreach (var company in lstDisable) { CompanyGetDTO companyDTO = new(); lstReturn.Add(companyDTO = new(company)); }

            return lstReturn;
        }

        public List<CompanyGetDTO> GetRestricted()
        {
            CompanyGetDTO companyDTO = new CompanyGetDTO();
            List<Company> lstRestricted = _companyRestricted.Find(c => true).ToList();
            List<CompanyGetDTO> lstReturn = new();

            foreach (var company in lstRestricted) { lstReturn.Add(companyDTO = new(company)); }

            return lstReturn;
        }

        public CompanyGetDTO GetByCompany(string cnpj)
        {
            CompanyGetDTO companyReturn = new();

            var company = _company.Find<Company>(c => c.Cnpj == cnpj).FirstOrDefault();
            var companyDisabled = _companyDisabled.Find<Company>(c => c.Cnpj == cnpj).FirstOrDefault();
            if (companyDisabled == null && company == null) return null;

            if (companyDisabled != null) { return companyReturn = new(companyDisabled); }
            else return companyReturn = new(company);

        }

        public async Task<CompanyGetDTO> GetByCompanyRestricted(string cnpj)
        {
            Company company = await _companyRestricted.Find(c => c.Cnpj == cnpj).FirstOrDefaultAsync();
            if (company == null) { return null; }

            CompanyGetDTO companyReturn = new(company);

            return companyReturn;
        }

        public Company Create(Company company)
        {
            if (_address.Find(a => a.Number == company.Address.Number && a.ZipCode == company.Address.ZipCode).FirstOrDefault() != null)
            {
                company.Status = false;
                _companyDisabled.InsertOne(company);

                return company;
            }

            company.Status = false;
            _address.InsertOne(new Address(company.Address));
            _companyDisabled.InsertOne(company);

            return company;
        }

        public async Task<CompanyGetDTO> Update(string cnpj, bool status)
        {
            if (status)
            {
                var companyRestricted = await _companyRestricted.Find(c => c.Cnpj == cnpj).FirstOrDefaultAsync();
                var companyDisabled = await _companyDisabled.Find(c => c.Cnpj == cnpj).FirstOrDefaultAsync();

                HttpResponseMessage responseAirCraft = await CompaniesService.companyClient.GetAsync(endpointAirCraft + cnpj);
                if (!responseAirCraft.StatusCode.ToString().Equals("OK"))
                { throw new Exception(); }

                if (responseAirCraft.Content.Headers.ContentLength == 2) { return null; }

                if (companyRestricted != null)
                {
                    CompanyGetDTO companyActivated = new();

                    _companyRestricted.DeleteOne(c => c.Cnpj == cnpj);
                    companyRestricted.Status = true; _company.InsertOne(companyRestricted);
                    return companyActivated = new(companyRestricted);
                }
                else
                {
                    CompanyGetDTO companyActivated = new();

                    _companyDisabled.DeleteOne(c => c.Cnpj == cnpj);
                    companyDisabled.Status = true; _company.InsertOne(companyDisabled);
                    return companyActivated = new(companyDisabled);
                }
            }
            else
            {
                var companyRestricted = _companyRestricted.Find(c => c.Cnpj == cnpj).FirstOrDefaultAsync().Result;
                var companyActivated = _company.Find(c => c.Cnpj == cnpj).FirstOrDefaultAsync().Result;

                CompanyGetDTO companyDisabled = new();
                if (companyRestricted != null)
                {
                    companyRestricted.Status = status; _companyRestricted.ReplaceOne(c => c.Cnpj == cnpj, companyRestricted);
                    return companyDisabled = new(companyRestricted);
                }

                else
                {
                    companyActivated.Status = status; _company.DeleteOne(c => c.Cnpj == cnpj);
                    _companyDisabled.InsertOne(companyActivated);
                    return companyDisabled = new(companyActivated);
                }
            }
        }

        public async void UpdateRestricted(CompanyGetDTO company, string cnpj, bool status)
        {
            Company companyInsert = new(company);
            if (company.Status == true && status == true)
            {
                _company.DeleteOne(c => c.Cnpj == cnpj);
                _companyRestricted.InsertOne(companyInsert); return;
            }

            if (company.Status == false && status == true)
            {
                _companyDisabled.DeleteOne(c => c.Cnpj == cnpj);
                _companyRestricted.InsertOne(companyInsert); return;
            }

            CompanyGetDTO companyResult = new(await _companyRestricted.Find(c => c.Cnpj == cnpj).FirstOrDefaultAsync());
            if (companyResult == null) { throw new Exception(); }

            companyInsert = new(companyResult);
            if (companyInsert.Status == true) { _companyRestricted.DeleteOne(c => c.Cnpj == cnpj); _company.InsertOne(companyInsert); }
            else _companyRestricted.DeleteOne(c => c.Cnpj == cnpj); _companyDisabled.InsertOne(companyInsert); return;
        }

        public async void Delete(string cnpj)
        {
            var deletedCompany = _company.Find(c => c.Cnpj == cnpj).FirstOrDefaultAsync().Result;
            var deletedDisabled = _companyDisabled.Find(c => c.Cnpj == cnpj).FirstOrDefaultAsync().Result;
            if (deletedDisabled == null && deletedCompany == null)
            {
                var deleteRestricted = _companyRestricted.Find(c => c.Cnpj == cnpj).FirstOrDefaultAsync().Result;
                if (deleteRestricted != null) { _companyRestricted.DeleteOne(c => c.Cnpj == cnpj); _companyDeleted.InsertOne(deleteRestricted); return; }
            }
            if (deletedDisabled != null) { _companyDisabled.DeleteOne(c => c.Cnpj == cnpj); _companyDeleted.InsertOne(deletedDisabled); }
            else { _company.DeleteOne(c => c.Cnpj == cnpj); _companyDeleted.InsertOne(deletedCompany); }
        }
    }
}