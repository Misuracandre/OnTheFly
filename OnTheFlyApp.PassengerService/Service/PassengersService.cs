﻿using Amazon.Runtime.Internal.Transform;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using OnTheFly.Models;
using OnTheFlyApp.PassengerService.config;

namespace OnTheFlyApp.PassengerService.Service
{
    public class PassengersService
    {
        private readonly IMongoCollection<Passenger> _passenger;
        private readonly IMongoCollection<Passenger> _passengerDeactivated;
        private readonly IMongoCollection<Address> _address;

        public PassengersService(IPassengerServiceSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _passenger = database.GetCollection<Passenger>(settings.PassengerCollectionName);
            _passengerDeactivated = database.GetCollection<Passenger>(settings.PassengerDeactivatedCollectionName);
            _address = database.GetCollection<Address>(settings.PassengerAddressCollectionName);
        }

        public List<Passenger> GetAll()
        {
            List<Passenger> passengers = new();
            passengers = _passenger.Find<Passenger>(p => true).ToList();
            passengers.AddRange(_passengerDeactivated.Find(pd => true).ToList());
            
            return passengers;
        }

        public List<Passenger> GetActiveted() => _passenger.Find(p => true).ToList();

        public Passenger GetByCpf(string cpf) => _passenger.Find(p => p.Cpf == cpf).FirstOrDefault();

        public Passenger Create(Passenger passenger)
        {
            _passenger.InsertOne(passenger);
            return passenger;
        }

        public ActionResult<Passenger> Update(string cpf, bool status)
        {
            var options = new FindOneAndUpdateOptions<Passenger, Passenger> { ReturnDocument = ReturnDocument.After };
            var update = Builders<Passenger>.Update.Set("Status", status);
            var passenger = _passenger.FindOneAndUpdate<Passenger>(p => p.Cpf == cpf, update, options);
            return passenger;
        }

        public long Delete(string cpf) => _passenger.DeleteOne(p => p.Cpf == cpf).DeletedCount;
    }
}