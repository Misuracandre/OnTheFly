using Newtonsoft.Json;
using OnTheFly.Models;
using System.Net;

namespace OnTheFlyApp.Services
{
    public class AirCraftService
    {
        static readonly HttpClient aircraftClient = new HttpClient();
        static readonly string endpoint = "https://localhost:7195/api/PassengersService";

        public async Task<AirCraft> Insert(AirCraft aircraft)
        {

            try
            {
                HttpResponseMessage response = await AirCraftService.aircraftClient.GetAsync(endpoint + "/" + aircraft.Rab);
                string getJson = await response.Content.ReadAsStringAsync();
                var acJson = JsonConvert.DeserializeObject<AirCraft>(getJson);
                if (acJson == null)
                {
                    HttpResponseMessage responseAircraft = await AirCraftService.aircraftClient.PostAsJsonAsync(endpoint, aircraft);
                    responseAircraft.EnsureSuccessStatusCode();
                    var aircraftJson = await responseAircraft.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<AirCraft>(aircraftJson);
                }
                return acJson;
            }
            catch (Exception)
            {
                return null;
            }

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
            try
            {
                HttpResponseMessage response = await AirCraftService.aircraftClient.GetAsync(endpoint + "/rab" + rab);
                response.EnsureSuccessStatusCode();
                string aircraftJson = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<AirCraft>(aircraftJson);
            }
            catch (Exception)
            {
                return null;
            }
        }


        public async Task<List<AirCraft>> FindByCompany(string cnpj)
        {
            try
            {
                HttpResponseMessage response = await AirCraftService.aircraftClient.GetAsync(endpoint + "/cnpj" + cnpj);
                response.EnsureSuccessStatusCode();
                string aircraftJson = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<AirCraft>>(aircraftJson);
            }
            catch (Exception)
            {
                return null;
            }
        }



        public async Task<HttpStatusCode> Update(string rab)
        {
            bool status = false;
            HttpResponseMessage response = await AirCraftService.aircraftClient.PutAsJsonAsync(endpoint + "/" + rab, status);
            response.EnsureSuccessStatusCode();
            var updateJson = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<HttpStatusCode>(updateJson);
        }





        //public async Task Delete(string rab) { }
        public async Task<HttpStatusCode> Delete(string rab)
        {
            HttpResponseMessage response = await AirCraftService.aircraftClient.DeleteAsync(endpoint + rab);
            response.EnsureSuccessStatusCode();
            var deleteJson = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<HttpStatusCode>(deleteJson);
        }

    }
}
