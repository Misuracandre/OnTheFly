using MongoDB.Driver;
using Newtonsoft.Json;
using OnTheFly.Models;
using OnTheFlyApp.FlightService.Config;

namespace OnTheFlyApp.FlightService.Services
{
    public class FlightsService
    {
        private readonly IMongoCollection<Flight> _flight;
        private readonly IMongoCollection<Flight> _flightDeactivated;
        private readonly IMongoCollection<AirCraft> _airCraft;
        private readonly IMongoCollection<Airport> _airport;

        static readonly HttpClient flightClient = new HttpClient();

        public FlightsService(IFlightServiceSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);

            var database = client.GetDatabase(settings.DatabaseName);

            _flight = database.GetCollection<Flight>(settings.FlightCollectionName);
            _flightDeactivated = database.GetCollection<Flight>(settings.FlightDeactivatedCollectionName);
            _airCraft = database.GetCollection<AirCraft>(settings.FlightAirCraftCollectionName);
            _airport = database.GetCollection<Airport>(settings.FlightAirportCollectionName);
        }

        public List<Flight> GetAll()
        {
            List<Flight> flights = new();

            flights = _flight.Find<Flight>(f => true).ToList();
            flights.AddRange(_flightDeactivated.Find(fd => true).ToList());

            return flights;
        }

        public List<Flight> GetActivated() => _flight.Find(p => true).ToList();
        //public List<Flight> GetDeactivated() => _flight.Find(p => false).ToList();

        public Flight GetByAirCraftAndDeparture(string rab, DateTime departure)
        {
            var flight = _flight.Find(f => f.Departure == departure && f.Plane.Rab == rab).FirstOrDefault();

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
            string airportApiUrl = "https://localhost:44366/Airport/";

            try
            {
                HttpResponseMessage airportResponse = await FlightsService.flightClient.GetAsync(airportApiUrl + flight.Destiny.Iata);
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

            Company airline = new();
            string airlineApiUrl = "https://localhost:44366/Company/";

            try
            {
                //Busca informaçoes da companhia aérea              
                HttpResponseMessage airlineResponse = await FlightsService.flightClient.GetAsync(airlineApiUrl + flight.Plane.Company.Cnpj);
                airlineResponse.EnsureSuccessStatusCode();
                string airlineJson = await airlineResponse.Content.ReadAsStringAsync();
                airline = JsonConvert.DeserializeObject<Company>(airlineJson);
            }
            catch (HttpRequestException e)
            {
                throw;
            }
            

            if (airline.Status != true)
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

        public Flight UpdateFlight(string rab, DateTime departure, bool status)
        {
            //if (flight == null)
            //{
            //    throw new ArgumentException("Flight cannot be null.");
            //}

            var filter = Builders<Flight>.Filter.Eq(f => f.Plane.Rab, rab) &
                Builders<Flight>.Filter.Eq("Departure", departure);

            var options = new FindOneAndUpdateOptions<Flight, Flight> { ReturnDocument = ReturnDocument.After };

            var update = Builders<Flight>.Update.Set("Status", status);

            var flightUpdated = _flight.FindOneAndUpdate<Flight>(filter, update, options);

            return flightUpdated;
        }

        public void DeleteFlight(string rab, DateTime departure)
        {
            var filter = Builders<Flight>.Filter.Where(f => f.Plane.Rab == rab && f.Departure == departure);

            _flight.DeleteOne(filter);
        }
    }
}

