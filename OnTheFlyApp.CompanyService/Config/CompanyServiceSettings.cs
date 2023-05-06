namespace OnTheFlyApp.CompanyService.Config
{
    public class CompanyServiceSettings : ICompanyServiceSettings
    {
        public string CompanyCollectionName { get; set; }
        public string CompanyDeactivatedCollectionName { get; set; }
        public string CompanyAddressCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}
