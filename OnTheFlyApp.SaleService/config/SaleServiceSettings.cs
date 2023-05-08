namespace OnTheFlyApp.SaleService.config
{
    public class SaleServiceSettings : ISaleServiceSettings
    {
        public string SaleCollectionName { get; set; }
        public string ReservationCollectionName { get; set; }
        public string SaleDeactivedCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}
