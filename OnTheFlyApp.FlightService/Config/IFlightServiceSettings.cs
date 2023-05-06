namespace OnTheFlyApp.FlightService.Config
{
    public interface IFlightServiceSettings
    {
        string FlightCollectionName { get; set; }
        string FlightDeactivatedCollectionName { get; set; }
        string FlightAirportCollectionName { get; set; }
        string FlightAirCraftCollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}
