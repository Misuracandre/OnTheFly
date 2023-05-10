namespace OnTheFlyApp.FlightService.Config
{
    public interface IFlightServiceSettings
    {
        string FlightCollection { get; set; }
        string DisabledCollection { get; set; }
        string DeletedCollection { get; set; }
        string AirportCollection{ get; set; }
        string AirCraftCollection { get;  set; }
        string ConnectionString { get; set; }
        string Database { get; set; }
    }
}
