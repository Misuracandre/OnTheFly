namespace OnTheFlyApp.CompanyService.Config
{
    public interface ICompanyServiceSettings
    {
        string CompanyCollectionName { get; set; }
        string CompanyDeactivatedCollectionName { get; set; }
        string CompanyAddressCollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}
