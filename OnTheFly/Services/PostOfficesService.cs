using Newtonsoft.Json;
using OnTheFly.Models;

namespace OnTheFlyApp.Services
{
    public class PostOfficesService
    {
        static readonly HttpClient address = new HttpClient();
        public async Task<Address> GetAddress(string cep)
        {
            try
            {
                HttpResponseMessage response = await PostOfficesService.address.GetAsync("https://viacep.com.br/ws/" + cep + "/json/");
                response.EnsureSuccessStatusCode();
                string ender = await response.Content.ReadAsStringAsync();
                var end = JsonConvert.DeserializeObject<Address>(ender);
                return end;
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }
    }
}
