using System.Net;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.IIS.Core;
using Newtonsoft.Json;
using OnTheFly.Models;

namespace OnTheFlyApp.Services
{
    public class CompaniesService
    {
        static readonly HttpClient companytClient = new HttpClient();
        static readonly string endpointCompany = "https://localhost:7219/api/CompaniesService";
        /*static readonly string endpointAirCreaft = "https://localhost:7219/api/CompaniesService";*/


        public async Task<IEnumerable<Company>> FindAll()
        {
            try
            {
                HttpResponseMessage response = await CompaniesService.companytClient.GetAsync(endpointCompany);
                response.EnsureSuccessStatusCode();
                string companyJson = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IEnumerable<Company>>(companyJson);
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<IEnumerable<Company>> FindActivated()
        {
            try
            {
                HttpResponseMessage response = await CompaniesService.companytClient.GetAsync(endpointCompany + "/activated");
                response.EnsureSuccessStatusCode();
                string companyJson = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IEnumerable<Company>>(companyJson);
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<Company> FindCnpj(string cnpj)
        {
            try
            {
                HttpResponseMessage response = await CompaniesService.companytClient.GetAsync(endpointCompany + "/cnpj" + cnpj);
                response.EnsureSuccessStatusCode();
                string companyJson = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Company>(companyJson);
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<Company> Insert(Company company)
        {
            try
            {
                HttpResponseMessage response = await CompaniesService.companytClient.GetAsync(endpointCompany + "/" + company.Cnpj);
                string getJson = await response.Content.ReadAsStringAsync();
                var compJson = JsonConvert.DeserializeObject<Company>(getJson);
                if (compJson == null)
                {
                    HttpResponseMessage responseComp = await CompaniesService.companytClient.PostAsJsonAsync(endpointCompany, company);
                    responseComp.EnsureSuccessStatusCode();
                    var companyJson = await responseComp.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<Company>(companyJson);
                }
                return compJson;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<HttpStatusCode> Delete(string cnpj)
        {
            HttpResponseMessage response = await CompaniesService.companytClient.DeleteAsync(endpointCompany + cnpj);
            response.EnsureSuccessStatusCode();
            var deleteJson = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<HttpStatusCode>(deleteJson);
        }
        public async Task<HttpStatusCode> Update(string cnpj)
        {
            bool status = false;
            HttpResponseMessage response = await CompaniesService.companytClient.PutAsJsonAsync(endpointCompany + "/" + cnpj, status);
            response.EnsureSuccessStatusCode();
            var updateJson = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<HttpStatusCode>(updateJson);
        }
    }
}
