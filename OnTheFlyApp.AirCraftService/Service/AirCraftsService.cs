using MongoDB.Driver;
using OnTheFly.Models;
using OnTheFlyApp.AirCraftService.config;

namespace OnTheFlyApp.AirCraftService.Service
{
    public class AirCraftsService
    {
        private readonly IMongoCollection<AirCraft> _aircraft;
        private readonly IMongoCollection<Company> _company;

        public AirCraftsService(IAirCraftServiceSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _aircraft = database.GetCollection<AirCraft>(settings.AirCraftCollectionName);
            _company = database.GetCollection<Company>(settings.AircraftCompanyCollectionName);
        }

        public List<AirCraft> GetAll()
        {
            List<AirCraft> aircrafts = new();
            aircrafts = _aircraft.Find<AirCraft>(a => true).ToList();

            return aircrafts;
        }

        public AirCraft GetByRab(string rab) => _aircraft.Find(a => a.Rab == rab).FirstOrDefault();

        public AirCraft Create(AirCraft aircraft)
        {
            aircraft.Id = "";
            _aircraft.InsertOne(aircraft);
            return aircraft;
        }

        public AirCraft Update(string rab, DateTime dtLastFlight) 
        {
            var aircraft = _aircraft.Find(a => a.Rab == rab).FirstOrDefault();
            if (aircraft == null)
            {
                return aircraft;
            }

            aircraft.DtLastFlight = dtLastFlight;

            _aircraft.ReplaceOne(a => a.Rab == rab, aircraft);
            return aircraft;
        }

        public void Delete(string rab) => _aircraft.DeleteOne(a => a.Rab == rab);
    }
}
