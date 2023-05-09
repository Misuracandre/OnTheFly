namespace OnTheFlyApp.FlightService.Config
{
    public class FlightServiceSettings : IFlightServiceSettings
    {
        public string FlightCollection { get; set; }
        public string DisabledCollection { get; set; }
        public string AirportCollection { get; set; }
        public string AirCraftCollection { get; set; }
        public string ConnectionString { get; set; }
        public string Database { get; set; }
    }
}
