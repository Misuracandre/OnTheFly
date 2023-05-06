namespace OnTheFlyApp.PassengerService.config
{
    public class PassengerServiceSettings : IPassengerServiceSettings
    {
        public string PassengerCollectionName { get; set; }
        public string PassengerDeactivatedCollectionName { get; set; }
        public string PassengerAddressCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}
