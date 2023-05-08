using System.Net;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using OnTheFly.Models;
using OnTheFly.Models.Dto;
using OnTheFlyApp.CompanyService.Config;
using OnTheFlyApp.Services;

namespace OnTheFlyApp.CompanyService.Service
{
    public class CompaniesService
    {
        private readonly IMongoCollection<Company> _company;
        private readonly IMongoCollection<Company> _companyDeactivated;
        private readonly IMongoCollection<Address> _address;
        static readonly HttpClient companyClient = new HttpClient();
        static readonly string endpointAirCraft = "https://localhost:7117/api/AirCraftsService/company/";
        public CompaniesService() { }
        public CompaniesService(ICompanyServiceSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _company = database.GetCollection<Company>(settings.CompanyCollectionName);
            _companyDeactivated = database.GetCollection<Company>(settings.CompanyDeactivatedCollectionName);
            _address = database.GetCollection<Address>(settings.CompanyAddressCollectionName);
        }

        public List<CompanyDTO> GetAll()
        {
            List<Company> companieslst = new();
            companieslst = _company.Find(c => true).ToList();
            companieslst.AddRange(_companyDeactivated.Find(cd => true).ToList());

            List<CompanyDTO> lstReturn = new();
            foreach (var company in companieslst) { CompanyDTO companyDTO = new(); lstReturn.Add(companyDTO = new(company)); }

            return lstReturn;
        }

        public List<CompanyDTO> GetActivated()
        {
            List<Company> lstActivated = _company.Find(c => true).ToList();
            if (lstActivated == null) return null;

            List<CompanyDTO> lstReturn = new();
            foreach (var company in lstActivated)
            {
                CompanyDTO companyDTO = new();
                lstReturn.Add(companyDTO = new(company));
            }

            return lstReturn;
        }

        public List<CompanyDTO> GetDisable()
        {
            List<Company> lstDisable = _companyDeactivated.Find(c => true).ToList();
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
            if (company == null) { company = _companyDeactivated.Find<Company>(c => c.Cnpj == cnpj).FirstOrDefault(); }
            if (company == null) return null;

            companyReturn = new(company);

            return companyReturn;
        }

        public Company Create(Company company)
        {
            if (_address.Find(a => a.Number == company.Address.Number && a.ZipCode == company.Address.ZipCode).FirstOrDefault() != null)
                throw new Exception();
            _address.InsertOne(company.Address);
            company.Status = false;
            _companyDeactivated.InsertOne(company);

            return company;
        }

        public async Task<Company> Update(string cnpj, bool status)
        {
            var options = new FindOneAndUpdateOptions<Company, Company> { ReturnDocument = ReturnDocument.After };
            var update = Builders<Company>.Update.Set("Status", status);

            if (status == true)
            {
                _companyDeactivated.DeleteOne(c => c.Cnpj == cnpj);
                HttpResponseMessage responseAirCraft = await CompaniesService.companyClient.GetAsync(endpointAirCraft + cnpj);
                if (responseAirCraft.StatusCode.ToString().Equals("400")) 
                    throw new ArgumentException("Companhia sem avião.");
                var companyTrue = _company.FindOneAndUpdate<Company>(c => c.Cnpj == cnpj, update, options);

                return companyTrue;
            }
            var insertDesactivated = _company.Find(c => c.Cnpj == cnpj).FirstOrDefault();
            insertDesactivated.Status = status;
            _companyDeactivated.InsertOne(insertDesactivated);

            var companyFalse = _company.FindOneAndUpdate<Company>(c => c.Cnpj == cnpj, update, options);

            return companyFalse;
        }

        public void Delete(string cnpj) => _company.DeleteOne(c => c.Cnpj == cnpj);
    }
}
