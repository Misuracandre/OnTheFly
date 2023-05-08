using System.Net;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using OnTheFly.Models;
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

        public List<Company> GetAll()
        {
            List<Company> companies = new();
            companies = _company.Find(c => true).ToList();
            companies.AddRange(_companyDeactivated.Find(cd => true).ToList());

            return companies;
        }

        public List<Company> GetActivated() => _company.Find(c => true).ToList();

        public List<Company> GetDisable() => _companyDeactivated.Find(c => true).ToList();

        public Company GetByCompany(string cnpj)
        {
            var company = _company.Find<Company>(c => c.Cnpj == cnpj).FirstOrDefault();
            if (company == null) { company = _companyDeactivated.Find<Company>(c => c.Cnpj == cnpj).FirstOrDefault(); }
            if (company == null) return company;

            return company;
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

        public async Task<ActionResult<Company>> Update(string cnpj, bool status)
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
