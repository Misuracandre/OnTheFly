namespace OnTheFlyApp.FlightService.Config
{
    public class FlightServiceSettings : IFlightServiceSettings
    {
        public string FlightCollectionName { get; set; }
        public string FlightDeactivatedCollectionName { get; set; }
        public string FlightAirportCollectionName { get; set; }
        public string FlightAirCraftCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}
