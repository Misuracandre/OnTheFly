namespace OnTheFlyApp.PassengerService.config
{
    public class PassengerServiceSettings : IPassengerServiceSettings
    {
        public string PassengerCollection { get; set; }
        public string PassengerDisabledCollection { get; set; }
        public string PassengerRestrictedCollection { get; set; }
        public string PassengerDeletedCollection { get; set; }
        public string PassengerAddressCollection { get; set; }
        public string ConnectionString { get; set; }
        public string Database { get; set; }
    }
}
