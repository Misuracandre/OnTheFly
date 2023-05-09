using System.Net;
using Amazon.Runtime.Internal.Transform;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using OnTheFly.Models;
using OnTheFly.Models.Dto;
using OnTheFlyApp.PassengerService.config;
using Utility;

namespace OnTheFlyApp.PassengerService.Service
{
    public class PassengersService
    {
        private readonly IMongoCollection<Passenger> _passenger;
        private readonly IMongoCollection<Passenger> _passengerDisabled;
        private readonly IMongoCollection<Address> _address;
        private readonly Util _util;

        public PassengersService(IPassengerServiceSettings settings, Util util)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.Database);
            _passenger = database.GetCollection<Passenger>(settings.PassengerCollection);
            _passengerDisabled = database.GetCollection<Passenger>(settings.PassengerDisabledCollection);
            _address = database.GetCollection<Address>(settings.PassengerAddressCollection);
            _util = util;
        }

        public ActionResult<List<PassengerDTO>> GetAll()
        {
            List<Passenger> passengers = _passenger.Find<Passenger>(p => true).ToList();
            if (passengers.Count == 0)
                return new ContentResult() { Content = "Nenhum passageiro encontrado", StatusCode = StatusCodes.Status400BadRequest };

            List<PassengerDTO> passengerDTOs = new();
            foreach (var passenger in passengers)
            {
                PassengerDTO p = new(passenger);
                passengerDTOs.Add(p);
            }

            return passengerDTOs;
        }

        public ActionResult<PassengerDTO> GetByCpf(string cpf)
        {
            Passenger p = _passenger.Find(p => p.Cpf == cpf).FirstOrDefault();
            if (p == null) 
                return new ContentResult() { Content = "Passageiro não encontrado", StatusCode = StatusCodes.Status400BadRequest };
            return new PassengerDTO(p);
        }

        public ActionResult<PassengerDTO> Create(PassengerInsert passenger)
        {
            Passenger passengerComplete = new(passenger);

            passengerComplete.Cpf = _util.JustDigits(passengerComplete.Cpf);
            var passengerAlreadyExists = GetByCpf(passengerComplete.Cpf).Value;
            if (passengerAlreadyExists != null)
                return passengerAlreadyExists;

            if (!_util.VerifyCpf(passengerComplete.Cpf))
                return new ContentResult() { Content = "CPF inválido", StatusCode = StatusCodes.Status400BadRequest };

            if (passengerComplete.Gender.ToUpper() != "M" && passenger.Gender.ToUpper() != "F")
                return new ContentResult() { Content = "Gênero não definido", StatusCode = StatusCodes.Status400BadRequest };

            passengerComplete.Phone = _util.JustDigits(passengerComplete.Phone);
            if (passengerComplete.Phone.Length < 8)
                return new ContentResult() { Content = "Telefone requer no minimo 8 digitos", StatusCode = StatusCodes.Status400BadRequest };

            passengerComplete.Address = CreateAddres(passengerComplete.Address).Value;
            if (passengerComplete.Address == null)
                return new ContentResult() { Content = "Localidade não encontrada", StatusCode = StatusCodes.Status400BadRequest };

            _passenger.InsertOne(passengerComplete);
            PassengerDTO passengerDTO = new(passengerComplete);
            return passengerDTO;
        }

        public ActionResult<AddressDTO> CreateAddres(AddressDTO address)
        {
            address.ZipCode = _util.JustDigits(address.ZipCode);
            Address addressAlreadyExists = _address.Find(a => a.ZipCode == address.ZipCode && a.Number == address.Number).FirstOrDefault();

            if (addressAlreadyExists != null)
                return new AddressDTO(addressAlreadyExists);

            addressAlreadyExists = _util.GetAddress(address.ZipCode).Result;
            if (addressAlreadyExists == null)
                return new ContentResult() { Content = "Localidade não encontrada", StatusCode = StatusCodes.Status400BadRequest };
            
            addressAlreadyExists.ZipCode = address.ZipCode;
            addressAlreadyExists.Number = address.Number;

            _address.InsertOne(addressAlreadyExists);

            //AddressDTO returnAddress = new(addressAlreadyExists);
            //return returnAddress;
            return new AddressDTO(addressAlreadyExists);
        }

        public ActionResult<PassengerDTO> Update(string cpf, bool status)
        {
            var options = new FindOneAndUpdateOptions<Passenger, Passenger> { ReturnDocument = ReturnDocument.After };
            var update = Builders<Passenger>.Update.Set("Status", status);
            var passenger = _passenger.FindOneAndUpdate<Passenger>(p => p.Cpf == cpf, update, options);
            if(passenger == null)
                return new ContentResult() { Content = "Passageiro não encontrado", StatusCode = StatusCodes.Status404NotFound };
            return new PassengerDTO(passenger);
        }

        public async Task<ActionResult> Delete(string cpf)
        {
            Passenger passenger = _passenger.Find(p => p.Cpf == cpf).FirstOrDefault();
            if (passenger == null) return new ContentResult() { Content = "Passageiro não encontrado", 
                StatusCode = StatusCodes.Status400BadRequest };

            passenger.Status = false;
            _passengerDisabled.InsertOne(passenger);
            var pdisabled = _passengerDisabled.Find(p => p.Cpf == passenger.Cpf);
            if (pdisabled == null) 
                return new ContentResult() { Content = "CPF inválido", StatusCode = StatusCodes.Status400BadRequest }; ;

            if(_passenger.DeleteOne(p => p.Cpf == passenger.Cpf).DeletedCount != 1)
                return new ContentResult() { Content = "Falha ao deletar", StatusCode = StatusCodes.Status400BadRequest };

            return new ContentResult() { Content = "Passageiro deletado", StatusCode = StatusCodes.Status200OK };
        }
    }
}
