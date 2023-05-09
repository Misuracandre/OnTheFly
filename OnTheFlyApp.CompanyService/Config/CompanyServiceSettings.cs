namespace OnTheFlyApp.CompanyService.Config
{
    public class CompanyServiceSettings : ICompanyServiceSettings
    {
        public string CompanyCollection { get; set; }
        public string CompanyDisabledCollection { get; set; }
        public string CompanyAddressCollection { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}
