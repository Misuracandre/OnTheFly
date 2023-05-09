namespace OnTheFlyApp.SaleService.config
{
    public interface ISaleServiceSettings
    {
        string SaleCollection { get; set; }
        string ReservationCollection { get; set; }
        string PassengerCollection { get; set; }
        string FlightCollection { get; set; }
        string SaleDisabledCollection { get; set; }
        string ConnectionString { get; set; }
        string Database { get; set; }
    }
}
