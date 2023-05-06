namespace OnTheFlyApp.AirCraftService.config
{
    public class AirCraftServiceSettings : IAirCraftServiceSettings
    {
        public string AirCraftCollectionName { get; set; }
        public string AircraftCompanyCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}
