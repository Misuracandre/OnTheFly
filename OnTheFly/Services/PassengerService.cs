using Newtonsoft.Json;
using OnTheFly.Models;

namespace OnTheFlyApp.Services
{
    public class PassengerService
    {
        static readonly HttpClient passengerClient = new HttpClient();
        static readonly string endpoint = "https://localhost:7195/api/PassengersService";
        //static readonly PostOfficesService _postOfficeService = new PostOfficesService();

        public async Task<Passenger> Insert(Passenger passenger)
        {
            return new();
        }

        public async Task<List<Passenger>> FindAll()
        {
            try
            {
                HttpResponseMessage response = await PassengerService.passengerClient.GetAsync(endpoint);
                response.EnsureSuccessStatusCode();
                string passengerJson = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<Passenger>>(passengerJson);
            }
            catch (HttpRequestException e)
            {
                throw;
            }
        }

        public async Task<Address> FindById(int id)
        {
            return new();
        }

        public async Task<Address> Update(int id, Address newAddress)
        {
            return new();
        }

        public async Task Delete(int id)
        {
            
        }
    }
}
