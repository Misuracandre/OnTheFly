using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Newtonsoft.Json;
using OnTheFly.Models;
using OnTheFly.Models.Dto;
using OnTheFlyApp.FlightService.Config;
using static System.Net.WebRequestMethods;

namespace OnTheFlyApp.FlightService.Services
{
    public class FlightsService
    {
        private readonly IMongoCollection<Flight> _flight;
        private readonly IMongoCollection<Flight> _disabled;
        private readonly IMongoCollection<AirCraft> _airCraft;
        private readonly IMongoCollection<Airport> _airport;

        //AirportApiUrl = "https://localhost:44366/Airport/";
        //CompanyApiUrl = "https://localhost:7219/api/CompaniesService/";

        static readonly HttpClient flightClient = new HttpClient();

        public FlightsService(IFlightServiceSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);

            var database = client.GetDatabase(settings.Database);

            _flight = database.GetCollection<Flight>(settings.FlightCollection);
            _disabled = database.GetCollection<Flight>(settings.DisabledCollection);
            _airCraft = database.GetCollection<AirCraft>(settings.AirCraftCollection);
            _airport = database.GetCollection<Airport>(settings.AirportCollection);
        }

        public List<FlightDTO> GetAll()
        {
            List<Flight> flights = new();

            flights = _flight.Find<Flight>(f => true).ToList();

            List<FlightDTO> flightsDTO = new();
            foreach (var flight in flights)
            {
                FlightDTO flightDTO = new(flight);
                flightsDTO.Add(flightDTO);
            }


            return flightsDTO;
        }

        public List<Flight> GetDisabled() => _flight.Find(p => false).ToList();

        public async Task<FlightDTO> GetFlightByRabAndSchedule(string rab, DateTime Schedule)
        {
            var flight = await _flight.Find(f => f.Schedule == Schedule && f.Plane.Rab == rab).FirstOrDefaultAsync();

            if (flight == null)
            {
                throw new ArgumentException("Voo não encontrado para a aeronave e partida informados.");
            }
            return new FlightDTO(flight);
        }

        public async Task<FlightDTO> CreateFlight(Flight flight)
        {
            if (flight == null)
            {
                throw new ArgumentNullException(nameof(flight), "O voo não pode ser nulo.");
            }

            Airport airport = new();

            try
            {
                //Busca informaçoes do aeroporto
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
                throw new ArgumentException("O destino do voo não é um aeroporto nacional.");
            }

            AirCraft airCraft = new();

            try
            {
                //Busca informaçoes da companhia aérea              
                HttpResponseMessage airCraftResponse = await FlightsService.flightClient.GetAsync("https://localhost:5002/api/AirCraftsService/" + flight.Plane.Rab);
                airCraftResponse.EnsureSuccessStatusCode();
                string airCraftJson = await airCraftResponse.Content.ReadAsStringAsync();
                airCraft = JsonConvert.DeserializeObject<AirCraft>(airCraftJson);
            }
            catch (HttpRequestException e)
            {
                throw;
            }


            //Verifica se a companhia aérea está restrita
            if (airCraft.Company.Status != true)
            {
                throw new ArgumentException("A companhia aérea não está autorizada para voo.");
            }

            _flight.InsertOne(flight);

            return new FlightDTO(flight);
        }

        public ActionResult<FlightDTO> UpdateFlight(string rab, DateTime Schedule, bool status)
        {
            var filter = Builders<Flight>.Filter.Eq(f => f.Plane.Rab, rab) &
                Builders<Flight>.Filter.Eq("Schedule", Schedule);

            var options = new FindOneAndUpdateOptions<Flight, Flight> { ReturnDocument = ReturnDocument.After };

            var update = Builders<Flight>.Update.Set("Status", status);

            var flightUpdated = _flight.FindOneAndUpdate<Flight>(filter, update, options);

            return new FlightDTO(flightUpdated);
        }

        public async Task DeleteFlight(string rab, DateTime schedule)
        {
            var filter = Builders<Flight>.Filter.Where(f => f.Plane.Rab == rab && f.Schedule == schedule);
            var flightToDelete = await _flight.Find(filter).FirstOrDefaultAsync();

            if (flightToDelete == null)
            {
                throw new ArgumentException("Voo não encontrado.");
            }

            flightToDelete.Status = false;

            await _disabled.InsertOneAsync(flightToDelete);

            await _flight.DeleteOneAsync(filter);
        }
    }
}

