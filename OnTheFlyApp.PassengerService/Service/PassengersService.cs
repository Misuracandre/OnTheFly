using Amazon.Runtime.Internal.Transform;
using Microsoft.AspNetCore.Mvc;
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
        private readonly IMongoCollection<Passenger> _passengerDeactivated;
        private readonly IMongoCollection<Address> _address;
        private readonly Util _util;

        public PassengersService(IPassengerServiceSettings settings, Util util)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _passenger = database.GetCollection<Passenger>(settings.PassengerCollectionName);
            _passengerDeactivated = database.GetCollection<Passenger>(settings.PassengerDeactivatedCollectionName);
            _address = database.GetCollection<Address>(settings.PassengerAddressCollectionName);
            _util = util;
        }

        public List<PassengerDTO> GetAll()
        {
            List<Passenger> passengers = new();
            passengers = _passenger.Find<Passenger>(p => true).ToList();
            passengers.AddRange(_passengerDeactivated.Find(pd => true).ToList());

            List<PassengerDTO> passengerDTOs = new();
            foreach (var passenger in passengers)
            {
                PassengerDTO p = new(passenger);
                passengerDTOs.Add(p);
            }
            
            return passengerDTOs;
        }

        public PassengerDTO GetByCpf(string cpf)
        {
            Passenger p = _passenger.Find(p => p.Cpf == cpf).FirstOrDefault();
            if (p == null) return null;
            return new PassengerDTO(p);
        }

        public PassengerDTO Create(PassengerInsert passenger)
        {
            passenger.Cpf = _util.JustDigits(passenger.Cpf);
            if (!_util.VerifyCpf(passenger.Cpf))
                return null;

            passenger.Address.ZipCode = _util.JustDigits(passenger.Address.ZipCode);
            Address ad = _util.GetAddress(passenger.Address.ZipCode).Result;
            if (ad == null)
                return null;

            passenger.Phone = _util.JustDigits(passenger.Phone);

            Passenger p = new(passenger);
            p.Address = ad;
            _passenger.InsertOne(p);
            PassengerDTO passengerDTO = new(p);
            return passengerDTO;
        }

        public ActionResult<PassengerDTO> Update(string cpf, bool status)
        {
            var options = new FindOneAndUpdateOptions<Passenger, Passenger> { ReturnDocument = ReturnDocument.After };
            var update = Builders<Passenger>.Update.Set("Status", status);
            var passenger = _passenger.FindOneAndUpdate<Passenger>(p => p.Cpf == cpf, update, options);
            if(passenger == null) return null;
            return new PassengerDTO(passenger);
        }

        public async Task<long> Delete(string cpf)
        {
            Passenger pa = _passenger.Find(p => p.Cpf == cpf).FirstOrDefault();
            if (pa == null) return 0;

            pa.Status = false;
            _passengerDeactivated.InsertOne(pa);
            var t = _passengerDeactivated.Find(p => p.Cpf == pa.Cpf);
            if (t == null) return 0;

            if (_passenger.DeleteOne(p => p.Cpf == pa.Cpf).DeletedCount != 1)
                return 0;

            return 1;
        }
    }
}
