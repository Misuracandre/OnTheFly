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
        static readonly string endpointAirCraft = "https://localhost:5002/api/AirCraftsService/company/";
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

            foreach(var company in lstRestricted) { lstReturn.Add(companyDTO = new(company)); }

            return lstReturn;
        }

        public CompanyGetDTO GetByCompany(string cnpj)
        {
            CompanyGetDTO companyReturn = new();

            var company = _company.Find<Company>(c => c.Cnpj == cnpj).FirstOrDefault();
            var companyDisabled = _companyDisabled.Find<Company>(c => c.Cnpj == cnpj).FirstOrDefault();
            if (companyDisabled == null && company == null) return null;

            if(companyDisabled != null) { return companyReturn = new(companyDisabled); }
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
                throw new Exception();
            company.Status = false;
            _address.InsertOne(new Address(company.Address));
            _company.InsertOne(company);

            return company;
        }

        public async Task<CompanyGetDTO> Update(string cnpj, bool status)
        {
            var company = _companyRestricted.Find(c => c.Cnpj == cnpj).FirstOrDefaultAsync().Result;
            if(company != null)
            {
                HttpResponseMessage responseAirCraft = await CompaniesService.companyClient.GetAsync(endpointAirCraft + cnpj);
                if (!responseAirCraft.StatusCode.ToString().Equals("204") || !responseAirCraft.StatusCode.ToString().Equals("200"))
                    return null;

                CompanyGetDTO companyRestricted = new();
                if (status == true) { company.Status = true; _companyRestricted.ReplaceOne(c => c.Cnpj == cnpj, company); 
                    return companyRestricted = new(company); }
                else company.Status = false; _companyRestricted.ReplaceOne(c => c.Cnpj == cnpj, company); 
                    return companyRestricted = new(company);
            }

            if (status == true)
            {
                company = _companyDisabled.Find(c => c.Cnpj == cnpj).FirstOrDefaultAsync().Result;
                company.Status = status;
                CompanyGetDTO companyTrue = new(company);
                _companyDisabled.DeleteOne(c => c.Cnpj == cnpj);
                _company.InsertOne(company);
                HttpResponseMessage responseAirCraft = await CompaniesService.companyClient.GetAsync(endpointAirCraft + cnpj);
                if (!responseAirCraft.StatusCode.ToString().Equals("204") || !responseAirCraft.StatusCode.ToString().Equals("200"))
                    return null;

                return companyTrue;
            }
            else {
                var insertDisabled = _company.Find(c => c.Cnpj == cnpj).FirstOrDefaultAsync().Result;
                _company.DeleteOne(c => c.Cnpj == cnpj);
                insertDisabled.Status = status;
                await _companyDisabled.InsertOneAsync(insertDisabled);

                CompanyGetDTO companyFalse = new(insertDisabled);

                return companyFalse; 
            }
        }

        public async void UpdateRestricted(CompanyGetDTO company, string cnpj, bool status)
        {
            Company companyInsert = new(company);
            if (company.Status == true && status == true){ _company.DeleteOne(c => c.Cnpj == cnpj); 
                _companyRestricted.InsertOne(companyInsert); return;}
            
            if(company.Status == false && status == true){ _companyDisabled.DeleteOne(c => c.Cnpj == cnpj); 
                _companyRestricted.InsertOne(companyInsert); return;}

            CompanyGetDTO companyResult = new(await _companyRestricted.Find(c => c.Cnpj == cnpj).FirstOrDefaultAsync());
            if (companyResult == null) { throw new Exception(); }

            companyInsert = new(companyResult);
            if (companyInsert.Status == true) { _companyRestricted.DeleteOne(c => c.Cnpj == cnpj); _company.InsertOne(companyInsert); }
            else _companyRestricted.DeleteOne(c => c.Cnpj == cnpj); _companyDisabled.InsertOne(companyInsert); return;
        }

        public void Delete(string cnpj)
        {
            var deletedDisabled = _companyDisabled.Find(c => c.Cnpj == cnpj).FirstOrDefaultAsync().Result;
            var deletedCompany = _company.Find(c => c.Cnpj == cnpj).FirstOrDefaultAsync().Result;
            if (deletedDisabled == null && deletedCompany == null)
            {
                var deleteRestricted = _companyRestricted.Find(c => c.Cnpj == cnpj).FirstOrDefaultAsync().Result;
                if (deleteRestricted != null) { _companyRestricted.DeleteOne(cnpj); _companyDeleted.InsertOne(deletedDisabled); return; }
            }
            if(deletedDisabled != null) { _companyDisabled.DeleteOne(cnpj); _companyDeleted.InsertOne(deletedDisabled); return; }
            else _company.DeleteOne(cnpj); _companyDeleted.InsertOne(deletedCompany); return;
        }
    }
}