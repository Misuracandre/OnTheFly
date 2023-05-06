namespace OnTheFlyApp.PassengerService.config
{
    public interface IPassengerServiceSettings
    {
        string PassengerCollectionName { get; set; }
        string PassengerDeactivatedCollectionName { get; set; }
        string PassengerAddressCollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}
