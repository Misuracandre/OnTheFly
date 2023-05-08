using System.Net;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.IIS.Core;
using Newtonsoft.Json;
using OnTheFly.Models;

namespace OnTheFlyApp.Services
{
    public class CompanieServices
    {
        static readonly HttpClient companytClient = new HttpClient();
        static readonly string endpointCompany = "https://localhost:7219/api/CompaniesService";
        static readonly string endpointAirCraft = "https://localhost:7117/api/AirCraftsService/company/";


        public async Task<IEnumerable<Company>> FindAll()
        {
            try
            {
                HttpResponseMessage response = await CompanieServices.companytClient.GetAsync(endpointCompany);
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
                HttpResponseMessage response = await CompanieServices.companytClient.GetAsync(endpointCompany + "/activated");
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
                HttpResponseMessage response = await CompanieServices.companytClient.GetAsync(endpointCompany + "/cnpj" + cnpj);
                response.EnsureSuccessStatusCode();
                string companyJson = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Company>(companyJson);
            }
            catch (Exception)
            {
                throw new ArgumentException("");
            }
        }
        public async Task<Company> Insert(Company company)
        {
            if (company.Cnpj.Length < 14) throw new ArgumentException("Cnpj inválido");
            try
            {
                HttpResponseMessage response = await CompanieServices.companytClient.GetAsync(endpointCompany + "/" + company.Cnpj);
                string getJson = await response.Content.ReadAsStringAsync();
                var compJson = JsonConvert.DeserializeObject<Company>(getJson);
                if (compJson == null)
                {
                    HttpResponseMessage responseComp = await CompanieServices.companytClient.PostAsJsonAsync(endpointCompany, company);
                    responseComp.EnsureSuccessStatusCode();
                    var companyJson = await responseComp.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<Company>(companyJson);
                }
                return compJson;
            }
            catch (Exception)
            {
                throw new ArgumentException("Sem retorno do banco");
            }
        }
        public async Task<HttpStatusCode> Delete(string cnpj)
        {
            try
            {
                HttpResponseMessage response = await CompanieServices.companytClient.DeleteAsync(endpointCompany + cnpj);
                response.EnsureSuccessStatusCode();
                return response.StatusCode;
            }
            catch (Exception)
            {
                throw new ArgumentException("Cnpj não encontrado");
            }
        }
        public async Task<HttpStatusCode> Update(string cnpj, bool status)
        {
            if (status == true)
            {
                HttpResponseMessage responseAirCraft = await CompanieServices.companytClient.GetAsync(endpointAirCraft + cnpj);
                if (responseAirCraft.StatusCode.ToString().Equals("400")) throw new ArgumentException("Companhia sem avião.");
            }
            HttpResponseMessage response = await CompanieServices.companytClient.PutAsJsonAsync(endpointCompany + "/" + cnpj, status);
            response.EnsureSuccessStatusCode();
            var updateJson = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<HttpStatusCode>(updateJson);
        }
    }
}