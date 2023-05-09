using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json;
using OnTheFly.Models;

namespace OnTheFlyApp.Services
{
    public class FlightService
    {
        static readonly HttpClient flightClient = new HttpClient();
        static readonly string endpoint = "https://localhost:7195/api/FlightsService";

        public async Task<Flight> Insert(Flight flight)
        {
            return new();
        }

        public async Task<List<Flight>> FindAll()
        {
            try
            {
                HttpResponseMessage response = await FlightService.flightClient.GetAsync(endpoint);
                response.EnsureSuccessStatusCode();
                string flightJson = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<Flight>>(flightJson);
            }
            catch (HttpRequestException e)
            {
                throw;
            }
        }

        public async Task<AirCraft> FindById(int id)
        {
            return new();
        }

        public async Task<AirCraft> Update(int id, AirCraft newAirCraft)
        {
            return new();
        }

        public async Task Delete(int id)
        {

        }
    }
}
