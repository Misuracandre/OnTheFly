using System.Net;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OnTheFly.Models;

namespace OnTheFlyApp.Services
{
    public class PassengerService
    {
        static readonly HttpClient passengerClient = new HttpClient();
        static readonly string endpoint = "https://localhost:7195/api/PassengersService/";
        //static readonly PostOfficesService _postOfficeService = new PostOfficesService();

        public async Task<Passenger> Insert(Passenger passenger)
        {
            try
            {
                HttpResponseMessage response = await PassengerService.passengerClient.PostAsJsonAsync(endpoint, passenger);
                response.EnsureSuccessStatusCode();
                string passengerJson = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Passenger>(passengerJson);
            }
            catch (HttpRequestException e)
            {
                throw;
            }
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

        public async Task<Passenger> FindByCpf(string cpf)
        {
            try
            {
                HttpResponseMessage response = await PassengerService.passengerClient.GetAsync(endpoint + cpf);
                response.EnsureSuccessStatusCode();
                string passengerJson = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Passenger>(passengerJson);
            }
            catch (HttpRequestException e)
            {
                throw;
            }
        }

        public async Task<Passenger> Update(string cpf, bool status)
        {
            try
            {
                HttpResponseMessage response = await PassengerService.passengerClient.PutAsJsonAsync(endpoint + cpf, status);
                response.EnsureSuccessStatusCode();
                string passengerJson = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Passenger>(passengerJson);
            }
            catch (HttpRequestException e)
            {
                throw;
            }
        }

        public async Task<string> Delete(string cpf)
        {
            try
            {
                HttpResponseMessage response = await PassengerService.passengerClient.DeleteAsync(endpoint + cpf);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException e) when (e.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
            catch (HttpRequestException e)
            {
                throw;
            }
        }
    }
}
