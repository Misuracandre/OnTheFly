namespace OnTheFlyApp.FlightService.Config
{
    public class FlightServiceSettings : IFlightServiceSettings
    {
        public string FlightCollection { get; set; }
        public string FlightDeactivatedCollection { get; set; }
        public string FlightAirportCollection { get; set; }
        public string FlightAirCraftCollection { get; set; }
        public string ConnectionString { get; set; }
        public string Database { get; set; }
    }
}
