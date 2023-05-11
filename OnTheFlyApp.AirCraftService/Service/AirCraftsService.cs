using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using OnTheFly.Models;
using OnTheFly.Models.Dto;
using OnTheFlyApp.AirCraftService.config;
using System.Net;
using Newtonsoft.Json;

namespace OnTheFlyApp.AirCraftService.Service
{
    public class AirCraftsService
    {
        private readonly IMongoCollection<AirCraft> _aircraft;
        private readonly IMongoCollection<AirCraft> _aircraftDisabled;
        private readonly IMongoCollection<Company> _company;
        static readonly HttpClient aircraftClient = new HttpClient();
        static readonly string endCompany = "https://localhost:5001/api/CompaniesService/cnpj/";
        

        public AirCraftsService() { }

        public AirCraftsService(IAirCraftServiceSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.Database);
            _aircraft = database.GetCollection<AirCraft>(settings.AircraftCollection);
            _aircraftDisabled = database.GetCollection<AirCraft>(settings.AircraftDisabledCollection);
            _company = database.GetCollection<Company>(settings.AircraftCompanyCollection);
            
        }

        public async Task<ActionResult<AirCraftDTO>> Create(AirCraftInsertDTO aircraft)
        {
            AirCraftDTO aircraftReturn = new(aircraft);
            var companyObj = new AirCraftCompany();
            companyObj.Address = new();
            try
            {                
                HttpResponseMessage response = await AirCraftsService.aircraftClient.GetAsync("https://localhost:5001/api/CompanyService/getCompanyCnpj?cnpj=" + aircraft.Company);
                response.EnsureSuccessStatusCode();
                var companyReturn = await response.Content.ReadAsStringAsync();
                companyObj = JsonConvert.DeserializeObject<AirCraftCompany>(companyReturn);

            }
            catch (Exception)
            {
                return null;
            }
            aircraftReturn.Company = companyObj;
            AirCraft aircraftdto = new(aircraftReturn);

            _aircraft.InsertOne(aircraftdto);
            return aircraftReturn;

        }

        public ActionResult<List<AirCraftDTO>> GetAll()
        {
            List<AirCraft> aircrafts = _aircraft.Find<AirCraft>(a => true).ToList();
            if (aircrafts.Count == 0)
                return new ContentResult() { Content = "Nenhuma aeronave encontrada", StatusCode = StatusCodes.Status400BadRequest };

            List<AirCraftDTO> aircraftDTOs = new();
            foreach (var aircraft in aircrafts)
            {
                AirCraftDTO a = new(aircraft);
                aircraftDTOs.Add(a);
            }

            return aircraftDTOs;
        }

        public ActionResult<AirCraftDTO> GetByRab(string rab)
        {
            AirCraft a = _aircraft.Find(a => a.Rab == rab).FirstOrDefault();
            if (a == null)
                return new ContentResult() { Content = "Rab não encontrado", StatusCode = StatusCodes.Status400BadRequest };
            return new AirCraftDTO(a);
        }

        
        
        


        public List<AirCraftDTO> GetDisable()
        {
            List<AirCraft> lstDisable = _aircraftDisabled.Find(a => true).ToList();
            if (lstDisable == null) return null;

            List<AirCraftDTO> lstReturn = new();
            foreach (var aircraft in lstDisable) { AirCraftDTO aircraftDTO = new(); lstReturn.Add(aircraftDTO = new(aircraft)); }

            return lstReturn;
        }


        public List<AirCraft> GetByCompany(string cnpj)
        {
            List<AirCraft> aircrafts = new();
            aircrafts = _aircraft.Find<AirCraft>(a => a.Company.Cnpj == cnpj).ToList();

            return aircrafts;
        }

        

        public AirCraft Update(string rab, DateTime dtLastFlight) 
        {
            var aircraft = _aircraft.Find(a => a.Rab == rab).FirstOrDefault();
            if (aircraft == null)
            {
                return aircraft;
            }

            aircraft.DtLastFlight = dtLastFlight;

            _aircraft.ReplaceOne(a => a.Rab == rab, aircraft);
            return aircraft;
        }

        public void Delete(string rab) => _aircraft.DeleteOne(a => a.Rab == rab);
    }
}
