using MongoDB.Driver;
using Newtonsoft.Json;
using OnTheFly.Models;
using OnTheFlyApp.FlightService.Config;
using static System.Net.WebRequestMethods;

namespace OnTheFlyApp.FlightService.Services
{
    public class FlightsService
    {
        private readonly IMongoCollection<Flight> _flight;
        private readonly IMongoCollection<Flight> _deactivated;
        private readonly IMongoCollection<AirCraft> _airCraft;
        private readonly IMongoCollection<Airport> _airport;

        //private const string AirportApiUrl = "https://localhost:44366/Airport/";
        //private const string CompanyApiUrl = "https://localhost:7219/api/CompaniesService/";

        static readonly HttpClient flightClient = new HttpClient();

        public FlightsService(IFlightServiceSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);

            var database = client.GetDatabase(settings.Database);

            _flight = database.GetCollection<Flight>(settings.FlightCollection);
            _deactivated = database.GetCollection<Flight>(settings.FlightDeactivatedCollection);
            _airCraft = database.GetCollection<AirCraft>(settings.FlightAirCraftCollection);
            _airport = database.GetCollection<Airport>(settings.FlightAirportCollection);
        }

        public List<Flight> GetAll()
        {
            List<Flight> flights = new();

            flights = _flight.Find<Flight>(f => true).ToList();
            //flights.AddRange(_flightDeactivated.Find(fd => true).ToList());

            return flights;
        }

        //public List<Flight> GetActivated() => _flight.Find(p => true).ToList();
        public List<Flight> GetDeactivated() => _flight.Find(p => false).ToList();

        public Flight GetFlightByRabAndSchedule(string rab, DateTime Schedule)
        {
            var flight = _flight.Find(f => f.Schedule == Schedule && f.Plane.Rab == rab).FirstOrDefault();

            if (flight == null)
            {
                throw new ArgumentException("Flight not found for the given aircraft and departure.");
            }
            return flight;
        }

        public async Task<Flight> CreateFlight(Flight flight)
        {
            if (flight == null)
            {
                throw new ArgumentNullException(nameof(flight), "The flight object cannot be null.");
            }

            Airport airport = new();

            try
            {

                HttpResponseMessage airportResponse = await FlightsService.flightClient.GetAsync("https://localhost:44366/Airport/" + flight.Arrival.Iata);
                airportResponse.EnsureSuccessStatusCode();
                string airportJson = await airportResponse.Content.ReadAsStringAsync();
                airport = JsonConvert.DeserializeObject<Airport>(airportJson);
            }
            catch (HttpRequestException e)
            {
                throw;
            }

            //Verifica se o voo é nacional
            if (airport.Country_id != "BR")
            {
                throw new ArgumentException("The flight destination must be a national airport.");
            }

            AirCraft airCraft = new();

            try
            {
                //Verifica se a companhia aérea está restrita
                //var airCraft = _airCraft.Find(ac => ac.Rab == flight.Plane.Rab && ac.Company.Status == true).FirstOrDefault();
                //if (airCraft == null)
                //{
                //    throw new ArgumentException("The aircraft company is not authorized to operate flights.");
                //}
                //Busca informaçoes da companhia aérea              
                HttpResponseMessage airCraftResponse = await FlightsService.flightClient.GetAsync("https://localhost:7117/api/AirCraftsService/" + flight.Plane.Rab);
                airCraftResponse.EnsureSuccessStatusCode();
                string airCraftJson = await airCraftResponse.Content.ReadAsStringAsync();
                airCraft = JsonConvert.DeserializeObject<AirCraft>(airCraftJson);
            }
            catch (HttpRequestException e)
            {
                throw;
            }


            if (airCraft.Company.Status != true)
            {
                throw new ArgumentException("the aircraft company is not authorized to operate flights.");
            }

            //Verifica se a companhia aérea está restrita
            //var airCraft = _airCraft.Find(ac => ac.Rab == flight.Plane.Rab && ac.Company.Status == true).FirstOrDefault();
            //if (airCraft == null)
            //{
            //    throw new ArgumentException("The aircraft company is not authorized to operate flights.");
            //}

            _flight.InsertOne(flight);

            return flight;
        }

        public Flight UpdateFlight(string rab, DateTime Schedule, bool status)
        {
            //if (flight == null)
            //{
            //    throw new ArgumentException("Flight cannot be null.");
            //}

            var filter = Builders<Flight>.Filter.Eq(f => f.Plane.Rab, rab) &
                Builders<Flight>.Filter.Eq("Schedule", Schedule);

            var options = new FindOneAndUpdateOptions<Flight, Flight> { ReturnDocument = ReturnDocument.After };

            var update = Builders<Flight>.Update.Set("Status", status);

            var flightUpdated = _flight.FindOneAndUpdate<Flight>(filter, update, options);

            return flightUpdated;
        }

        public async Task DeleteFlight(string rab, DateTime schedule)
        {
            var filter = Builders<Flight>.Filter.Where(f => f.Plane.Rab == rab && f.Schedule == schedule);
            var flightToDelete = await _flight.Find(filter).FirstOrDefaultAsync();

            if (flightToDelete == null)
            {
                throw new ArgumentException("Flight not found.");
            }

            flightToDelete.Status = false;

            await _deactivated.InsertOneAsync(flightToDelete);

            await _flight.DeleteOneAsync(filter);
        }
    }
}

