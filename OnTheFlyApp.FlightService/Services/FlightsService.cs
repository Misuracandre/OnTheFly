using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Newtonsoft.Json;
using OnTheFly.Models;
using OnTheFly.Models.Dto;
using OnTheFlyApp.FlightService.Config;
using static System.Net.WebRequestMethods;
using Utility;

namespace OnTheFlyApp.FlightService.Services
{
    public class FlightsService
    {
        private readonly IMongoCollection<Flight> _flight;
        private readonly IMongoCollection<Flight> _disabled;
        private readonly IMongoCollection<Flight> _deleted;
        private readonly IMongoCollection<AirCraft> _airCraft;
        private readonly IMongoCollection<Airport> _airport;
        private readonly Util _util;

        //AirportApiUrl = "https://localhost:44366/Airport/";
        //CompanyApiUrl = "https://localhost:7219/api/CompaniesService/";

        static readonly HttpClient flightClient = new HttpClient();

        public FlightsService(IFlightServiceSettings settings, Util util)
        {
            var client = new MongoClient(settings.ConnectionString);

            var database = client.GetDatabase(settings.Database);

            _flight = database.GetCollection<Flight>(settings.FlightCollection);
            _airport = database.GetCollection<Airport>(settings.AirportCollection);
            _disabled = database.GetCollection<Flight>(settings.DisabledCollection);
            _deleted = database.GetCollection<Flight>(settings.DeletedCollection);
            _airCraft = database.GetCollection<AirCraft>(settings.AirCraftCollection);
            _util = util;
        }

        public ActionResult<List<FlightDTO>> GetAll()
        {
            List<Flight> flights = _flight.Find<Flight>(f => true).ToList();
            if (flights.Count == 0)
                return new ContentResult() { Content = "Nenhum voo encontrado", StatusCode = StatusCodes.Status400BadRequest };

            List<FlightDTO> flightsDTO = new();
            foreach (var flight in flights)
            {
                FlightDTO flightDTO = new(flight);
                flightsDTO.Add(flightDTO);
            }


            return flightsDTO;
        }

        //public ActionResult<List<FlightDTO>> GetDisabled()
        //{
        //    Flight flights = _flight.Find(f => false).ToList();
        //    if (flights == null)
        //        return new ContentResult() { Content = "Nenhum voo encontrado", StatusCode = StatusCodes.Status400BadRequest };

        //    return new List<FlightDTO>(flights);
        //}

        public ActionResult<List<Flight>> GetDeleted() => _deleted.Find(Builders<Flight>.Filter.Empty).ToList();

        public async Task<ActionResult<FlightDTO>> GetFlightByRabAndSchedule(string rab, DateTime Schedule)
        {
            var filter = Builders<Flight>.Filter.And(
                Builders<Flight>.Filter.Eq(f => f.Plane.Rab, rab),
                Builders<Flight>.Filter.Eq(f => f.Schedule, Schedule)
            );
            var flight = await _flight.Find(filter).FirstOrDefaultAsync();

            if (flight == null)
                return new ContentResult() { Content = "Voo não encontrado para a aeronave e partida informados.", StatusCode = StatusCodes.Status400BadRequest };

            return new FlightDTO(flight);
        }

        public async Task<ActionResult<FlightDTO>> CreateFlight(Flight flight)
        {
            if (flight == null)
                return new ContentResult() { Content = "O voo não pode ser nulo.", StatusCode = StatusCodes.Status400BadRequest };

            Airport airport = new();

            try
            {
                // Busca informações do aeroporto
                HttpResponseMessage airportResponse = await FlightsService.flightClient.GetAsync("https://localhost:44366/Airport/" + flight.Arrival.Iata);
                airportResponse.EnsureSuccessStatusCode();
                string airportJson = await airportResponse.Content.ReadAsStringAsync();
                airport = JsonConvert.DeserializeObject<Airport>(airportJson);

                if (airport == null)
                    return new ContentResult() { Content = "O IATA do aeroporto não é válido.", StatusCode = StatusCodes.Status400BadRequest };
            }
            catch (HttpRequestException e)
            {
                return new ContentResult() { Content = "Erro ao buscar informações do aeroporto.", StatusCode = StatusCodes.Status500InternalServerError };
            }


            // Verifica se o voo é nacional
            if (airport.Country_id != "BR")
                return new ContentResult() { Content = "O destino do voo não é um aeroporto nacional.", StatusCode = StatusCodes.Status400BadRequest };


            AirCraft airCraft = new();

            try
            {
                // Busca informações da companhia aérea
                HttpResponseMessage airCraftResponse = await FlightsService.flightClient.GetAsync("https://localhost:7117/api/AirCraftsService/" + flight.Plane.Rab);
                airCraftResponse.EnsureSuccessStatusCode();
                string airCraftJson = await airCraftResponse.Content.ReadAsStringAsync();
                airCraft = JsonConvert.DeserializeObject<AirCraft>(airCraftJson);

                if (airCraft == null)
                    return new ContentResult() { Content = "O RAB do avião não é válido.", StatusCode = StatusCodes.Status400BadRequest };
            }
            catch (HttpRequestException e)
            {
                return new ContentResult() { Content = "Erro ao buscar informações da companhia aérea.", StatusCode = StatusCodes.Status500InternalServerError };
            }


            // Verifica se a companhia aérea está restrita
            if (airCraft.Company.Status != true)
                return new ContentResult() { Content = "A companhia aérea não está autorizada para voo.", StatusCode = StatusCodes.Status400BadRequest };

            if (flight.Sales >= flight.Plane.Capacity)
                return new ContentResult() { Content = "O número de vendas não pode ser maior que a capacidade do avião.", StatusCode = StatusCodes.Status400BadRequest };

            if (flight.Sales < 1)
                return new ContentResult() { Content = "O número de vendas não pode ser menor do que 1.", StatusCode = StatusCodes.Status400BadRequest };

            if (flight.Status == true || flight.Status == false)
                return new ContentResult() { Content = "O status do voo deve ser informado como 'true' ou 'false'.", StatusCode = StatusCodes.Status400BadRequest };

            if (flight.Arrival.Iata.Length != 3)
                return new ContentResult() { Content = "O código IATA do aeroporto deve ter exatamente 3 caracteres.", StatusCode = StatusCodes.Status400BadRequest };


            _flight.InsertOne(flight);


            return new FlightDTO(flight);
        }

        public async Task<ActionResult<FlightDTO>> UpdateFlight(string rab, DateTime schedule, bool status)
        {

            if (rab == null)
                return new ContentResult() { Content = "O RAB do avião não pode ser nulo.", StatusCode = StatusCodes.Status400BadRequest };

            if (schedule == null)
                return new ContentResult() { Content = "O horário do avião não pode ser nulo.", StatusCode = StatusCodes.Status400BadRequest };

            //if (!_util.VerifyRab(rab))
            //    return new ContentResult() { Content = "RAB inválido.", StatusCode = StatusCodes.Status400BadRequest };

            var filter = Builders<Flight>.Filter.Eq(f => f.Plane.Rab, rab) &
                Builders<Flight>.Filter.Eq("Schedule", schedule);

            //var options = new FindOneAndUpdateOptions<Flight, Flight> { ReturnDocument = ReturnDocument.After };
            //var update = Builders<Flight>.Update.Set("Status", !status);

            var flightUpdated = await _flight.Find(filter).FirstOrDefaultAsync();

            if (flightUpdated == null)
                return new ContentResult() { Content = "O voo não foi encontrado.", StatusCode = StatusCodes.Status404NotFound };

            flightUpdated.Status = !status;

            if (flightUpdated.Status == true)
            {
                await _flight.InsertOneAsync(flightUpdated);
                await _deleted.DeleteOneAsync(filter);
            }
            else
            {
                await _deleted.InsertOneAsync(flightUpdated);
                await _flight.DeleteOneAsync(filter);
            }


            var result = await _flight.ReplaceOneAsync(filter, flightUpdated);

            if (result.ModifiedCount == 0)
                return new ContentResult() { Content = "O voo não foi atualizado.", StatusCode = StatusCodes.Status400BadRequest };

            return new FlightDTO(flightUpdated);
        }

        public async Task<ActionResult> DeleteFlight(string rab, DateTime schedule)
        {
            if (rab == null)
                return new ContentResult() { Content = "O RAB do avião não pode ser nulo.", StatusCode = StatusCodes.Status400BadRequest };

            if (schedule == null)
                return new ContentResult() { Content = "O horário do avião não pode ser nulo.", StatusCode = StatusCodes.Status400BadRequest };

            if (!_util.VerifyRab(rab))
                return new ContentResult() { Content = "RAB inválido.", StatusCode = StatusCodes.Status400BadRequest };

            var filter = Builders<Flight>.Filter.Where(f => f.Plane.Rab == rab && f.Schedule == schedule);
            var flightToDelete = await _flight.Find(filter).FirstOrDefaultAsync();

            if (flightToDelete == null)
                return new ContentResult() { Content = "Voo não encontrado.", StatusCode = StatusCodes.Status404NotFound };

            await _deleted.InsertOneAsync(flightToDelete);

            await _flight.DeleteOneAsync(filter);

            return new ContentResult() { Content = "O voo foi deletado com sucesso.", StatusCode = StatusCodes.Status204NoContent };
        }
    }
}