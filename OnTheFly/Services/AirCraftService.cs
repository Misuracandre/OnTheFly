using Newtonsoft.Json;
using OnTheFly.Models;

namespace OnTheFlyApp.Services
{
    public class AirCraftService
    {
        static readonly HttpClient aircraftClient = new HttpClient();
        static readonly string endpoint = "https://localhost:7195/api/PassengersService";

        public async Task<AirCraft> Insert(AirCraft aircraft)
        {
            return new();
        }

        public async Task<List<AirCraft>> FindAll()
        {
            try
            {
                HttpResponseMessage response = await AirCraftService.aircraftClient.GetAsync(endpoint);
                response.EnsureSuccessStatusCode();
                string aircraftJson = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<AirCraft>>(aircraftJson);
            }
            catch (HttpRequestException e)
            {
                throw;
            }
        }

        public async Task<AirCraft> FindByRab(string rab)
        {
            return new();
        }

        public async Task<AirCraft> Update(string rab, DateTime dtLastFlight)
        {
            return new();
        }

        public async Task Delete(string rab)
        {

        }
    }
}
