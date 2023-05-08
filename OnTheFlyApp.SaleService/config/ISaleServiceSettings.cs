namespace OnTheFlyApp.SaleService.config
{
    public interface ISaleServiceSettings
    {
        string SaleCollectionName { get; set; }
        string ReservationCollectionName { get; set; }
        string SaleDeactivedCollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}
