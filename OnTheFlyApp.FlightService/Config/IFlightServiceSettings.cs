namespace OnTheFlyApp.FlightService.Config
{
    public interface IFlightServiceSettings
    {
        string FlightCollection { get; set; }
        string FlightDeactivatedCollection { get; set; }
        string FlightAirportCollection{ get; set; }
        string FlightAirCraftCollection { get;  set; }
        string ConnectionString { get; set; }
        string Database { get; set; }
    }
}
