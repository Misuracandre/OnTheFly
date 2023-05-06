using MongoDB.Driver;
using OnTheFly.Models;
using OnTheFlyApp.CompanyService.Config;

namespace OnTheFlyApp.CompanyService.Service
{
    public class CompaniesService
    {
        private readonly IMongoCollection<Company> _company;
        private readonly IMongoCollection<Company> _companyDeactivated;
        private readonly IMongoCollection<Address> _address;

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
            companies = _company.Find<Company>(c => true).ToList();
            companies.AddRange(_companyDeactivated.Find(cd => true).ToList());

            return companies;
        }

        public List<Company> GetActiveted() => _company.Find(p => true).ToList();

        public Company GetByCompany(string cnpj)
        {
            var company = _company.Find<Company>(c => c.Cnpj == cnpj).FirstOrDefault();

            return company;
        }

        public Company Create(Company company)
        {
            _company.InsertOne(company);
            return company;
        }
        public Address CreateAddress(Address companyaddress)
        {
            _address.InsertOne(companyaddress);
            return companyaddress;
        }

        public void Update(string cnpj, Company company) => _company.ReplaceOne(c => c.Cnpj == cnpj, company);

        public void Delete(string cnpj) => _company.DeleteOne(c => c.Cnpj == cnpj);
    }
}
