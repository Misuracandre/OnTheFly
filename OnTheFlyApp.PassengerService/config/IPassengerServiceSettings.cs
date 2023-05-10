namespace OnTheFlyApp.PassengerService.config
{
    public interface IPassengerServiceSettings
    {
        string PassengerCollection { get; set; }
        string PassengerDisabledCollection { get; set; }
        string PassengerRestrictedCollection { get; set; }
        string PassengerDeletedCollection { get; set; }
        string PassengerAddressCollection { get; set; }
        string ConnectionString { get; set; }
        string Database { get; set; }
    }
}
