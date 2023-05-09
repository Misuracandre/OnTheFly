namespace OnTheFlyApp.SaleService.config
{
    public class SaleServiceSettings : ISaleServiceSettings
    {
        public string SaleCollection { get; set; }
        public string ReservationCollection { get; set; }
        public string PassengerCollection { get; set; }
        public string FlightCollection { get; set; }
        public string SaleDisabledCollection { get; set; }
        public string ConnectionString { get; set; }
        public string Database { get; set; }
    }
}
