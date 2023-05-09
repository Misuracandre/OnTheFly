namespace OnTheFlyApp.CompanyService.Config
{
    public interface ICompanyServiceSettings
    {
        string CompanyCollection { get; set; }
        string CompanyDisabledCollection { get; set; }
        string CompanyAddressCollection { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}
