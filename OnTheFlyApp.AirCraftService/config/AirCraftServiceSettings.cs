namespace OnTheFlyApp.AirCraftService.config
{
    public class AirCraftServiceSettings : IAirCraftServiceSettings
    {
        public string AircraftCollection { get; set; }
        public string AircraftCompanyCollection { get; set; }
        public string AircraftDisabledCollection { get; set; }
        public string ConnectionString { get; set; }
        public string Database { get; set; }
    }
}
