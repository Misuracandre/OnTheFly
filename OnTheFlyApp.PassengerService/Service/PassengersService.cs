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
        private readonly IMongoCollection<Passenger> _passengerRestricted;
        private readonly IMongoCollection<Passenger> _passengerDeleted;
        private readonly IMongoCollection<Address> _address;
        private readonly Util _util;

        public PassengersService(IPassengerServiceSettings settings, Util util)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.Database);
            _passenger = database.GetCollection<Passenger>(settings.PassengerCollection);
            _passengerDisabled = database.GetCollection<Passenger>(settings.PassengerDisabledCollection);
            _passengerRestricted = database.GetCollection<Passenger>(settings.PassengerRestrictedCollection);
            _passengerDeleted = database.GetCollection<Passenger>(settings.PassengerDeletedCollection);
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
            cpf = _util.JustDigits(cpf);
            if (!_util.VerifyCpf(cpf))
                return new ContentResult() { Content = "CPF inválido", StatusCode = StatusCodes.Status400BadRequest };

            Passenger p = _passenger.Find(p => p.Cpf == cpf).FirstOrDefault();
            if (p == null) 
                return new ContentResult() { Content = "Passageiro não encontrado", StatusCode = StatusCodes.Status404NotFound };
            return new PassengerDTO(p);
        }

        public ActionResult<PassengerDTO> Create(PassengerInsert passenger)
        {
            Passenger passengerComplete = new(passenger);

            passengerComplete.Cpf = _util.JustDigits(passengerComplete.Cpf);
            if (passengerComplete.Cpf == "")
                return new ContentResult() { Content = "CPF inválido. Digite apenas os números do CPF (Ex.: 00055599977)", StatusCode = StatusCodes.Status400BadRequest };

            var passengerAlreadyExists = GetByCpf(passengerComplete.Cpf).Value;
            if (passengerAlreadyExists != null)
                return passengerAlreadyExists;

            if (!_util.VerifyCpf(passengerComplete.Cpf))
                return new ContentResult() { Content = "CPF inválido. Verifique se os digitoos estão corretos", StatusCode = StatusCodes.Status400BadRequest };

            if (passengerComplete.Gender.ToUpper() != "M" && passenger.Gender.ToUpper() != "F" && passenger.Gender.ToUpper() != "O")
                return new ContentResult() { Content = "Gênero inválido, coloque 'F' para Feminino,'M' Masculino, ou 'O' para outro", StatusCode = StatusCodes.Status400BadRequest };

            passengerComplete.Phone = _util.JustDigits(passengerComplete.Phone);
            if (passengerComplete.Phone.Length < 8)
                return new ContentResult() { Content = "Telefone requer no mínimo 8 digitos (Ex.: (16)99788-6655)", StatusCode = StatusCodes.Status400BadRequest };

            if (!DateTime.TryParse(passenger.DtBirth, out DateTime dBirth))
                return new ContentResult() { Content = "Data de nascimento em formato inválido, digite uma data no formato dia, mês e ano (Ex.: 31/12/1999)", StatusCode = StatusCodes.Status400BadRequest };

            if (dBirth > DateTime.Now)
                return new ContentResult() { Content = "Data de nascimento deve ser menor que a data atual", StatusCode = StatusCodes.Status400BadRequest };

            passengerComplete.DtBirth = dBirth.AddHours(-3);

            if (passengerComplete.Status != true && passengerComplete.Status != false)
                return new ContentResult() { Content = "Status deve ser apenas true ou false.", StatusCode = StatusCodes.Status400BadRequest };

            passengerComplete.Address = CreateAddres(passengerComplete.Address).Value;
            if (passengerComplete.Address == null)
                return new ContentResult() { Content = "Localidade não encontrada", StatusCode = StatusCodes.Status400BadRequest };

            _passenger.InsertOne(passengerComplete);
            passengerComplete.DtBirth = dBirth.AddHours(3);
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

        public ActionResult<PassengerDTO> Update(string cpf, PassengerDTO passenger)
        {
            cpf = _util.JustDigits(cpf);
            if (!_util.VerifyCpf(cpf))
                return new ContentResult() { Content = "CPF inválido", StatusCode = StatusCodes.Status400BadRequest };

            var options = new FindOneAndUpdateOptions<Passenger, Passenger> { ReturnDocument = ReturnDocument.After };
            var update = Builders<Passenger>.Update.Set("Name", passenger.Name).
                                                    Set("Gender", passenger.Gender).
                                                    Set("Gender", passenger.Gender).
                                                    Set("Phone", passenger.Phone).
                                                    Set("DtBirth", passenger.DtBirth).
                                                    Set("Status", passenger.Status).
                                                    Set("Address", passenger.Address);
            var passengerUpdated = _passenger.FindOneAndUpdate<Passenger>(p => p.Cpf == cpf, update, options);
            if (passengerUpdated == null)
                return new ContentResult() { Content = "Passageiro não encontrado", StatusCode = StatusCodes.Status404NotFound };
            return new PassengerDTO(passengerUpdated);
        }

        public async Task<ActionResult> Delete(string cpf)
        {
            cpf = _util.JustDigits(cpf);
            if (!_util.VerifyCpf(cpf))
                return new ContentResult() { Content = "CPF inválido", StatusCode = StatusCodes.Status400BadRequest };

            Passenger passenger = _passenger.Find(p => p.Cpf == cpf).FirstOrDefault();
            if (passenger == null) return new ContentResult() { Content = "Passageiro não encontrado", 
                StatusCode = StatusCodes.Status400BadRequest };

            passenger.Status = false;
            _passengerDeleted.InsertOne(passenger);
            var pdisabled = _passengerDeleted.Find(p => p.Cpf == passenger.Cpf);
            if (pdisabled == null) 
                return new ContentResult() { Content = "CPF inválido", StatusCode = StatusCodes.Status400BadRequest }; ;

            if(_passenger.DeleteOne(p => p.Cpf == passenger.Cpf).DeletedCount != 1)
                return new ContentResult() { Content = "Falha ao deletar", StatusCode = StatusCodes.Status400BadRequest };

            return new ContentResult() { Content = "Passageiro deletado", StatusCode = StatusCodes.Status200OK };
        }

        public async Task<ActionResult> Disable(string cpf)
        {
            cpf = _util.JustDigits(cpf);
            if (!_util.VerifyCpf(cpf))
                return new ContentResult() { Content = "CPF inválido", StatusCode = StatusCodes.Status400BadRequest };

            Passenger passenger = _passenger.Find(p => p.Cpf == cpf).FirstOrDefault();
            if (passenger == null) return new ContentResult()
            {
                Content = "Passageiro não encontrado",
                StatusCode = StatusCodes.Status400BadRequest
            };

            passenger.Status = false;
            _passengerDisabled.InsertOne(passenger);
            var pdisabled = _passengerDisabled.Find(p => p.Cpf == passenger.Cpf);
            if (pdisabled == null)
                return new ContentResult() { Content = "CPF inválido", StatusCode = StatusCodes.Status400BadRequest }; ;

            if (_passenger.DeleteOne(p => p.Cpf == passenger.Cpf).DeletedCount != 1)
                return new ContentResult() { Content = "Falha ao desativar", StatusCode = StatusCodes.Status400BadRequest };

            return new ContentResult() { Content = "Passageiro desativado", StatusCode = StatusCodes.Status200OK };
        }

        public async Task<ActionResult> Restrict(string cpf)
        {
            cpf = _util.JustDigits(cpf);
            if (!_util.VerifyCpf(cpf))
                return new ContentResult() { Content = "CPF inválido", StatusCode = StatusCodes.Status400BadRequest };

            Passenger passenger = _passenger.Find(p => p.Cpf == cpf).FirstOrDefault();
            if (passenger == null) return new ContentResult()
            {
                Content = "Passageiro não encontrado",
                StatusCode = StatusCodes.Status400BadRequest
            };

            passenger.Status = false;
            _passengerRestricted.InsertOne(passenger);
            var pdisabled = _passengerRestricted.Find(p => p.Cpf == passenger.Cpf);
            if (pdisabled == null)
                return new ContentResult() { Content = "CPF inválido", StatusCode = StatusCodes.Status400BadRequest }; ;

            if (_passenger.DeleteOne(p => p.Cpf == passenger.Cpf).DeletedCount != 1)
                return new ContentResult() { Content = "Falha ao restringir", StatusCode = StatusCodes.Status400BadRequest };

            return new ContentResult() { Content = "Passageiro restrito", StatusCode = StatusCodes.Status200OK };
        }
    }
}
