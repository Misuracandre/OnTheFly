using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using OnTheFly.Models;
using OnTheFlyApp.FlightService.Config;

namespace OnTheFlyApp.FlightService.Services
{
    public class FlightsService
    {
        private readonly IMongoCollection<Flight> _flight;
        private readonly IMongoCollection<Flight> _flightDeactivated;
        private readonly IMongoCollection<AirCraft> _airCraft;
        private readonly IMongoCollection<Airport> _airport;

        public FlightsService(IFlightServiceSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);

            var database = client.GetDatabase(settings.DatabaseName);

            _flight = database.GetCollection<Flight>(settings.FlightCollectionName);
            _flightDeactivated = database.GetCollection<Flight>(settings.FlightDeactivatedCollectionName);
            _airCraft = database.GetCollection<AirCraft>(settings.FlightAirCraftCollectionName);
            _airport = database.GetCollection<Airport>(settings.FlightAirportCollectionName);
        }

        public List<Flight> GetAll()
        {
            List<Flight> flights = new();
            flights = _flight.Find<Flight>(f => true).ToList();
            flights.AddRange(_flightDeactivated.Find(fd => true).ToList());

            return flights;
        }

        public List<Flight> GetActivated() => _flight.Find(p => true).ToList();
        //public List<Flight> GetDeactivated() => _flight.Find(p => false).ToList();

        public Flight GetByAirCraftAndDeparture(string rab, DateTime departure)
        {
            var flight = _flight.Find(f => f.Departure == departure && f.Plane.Rab == rab).FirstOrDefault();

            if (flight == null)
            {
                throw new ArgumentException("Flight not found for the given aircraft and departure."); ;
            }
            return flight;
        }

        public Flight CreateFlight(Flight flight)
        {
            _flight.InsertOne(flight);

            return flight;
        }

        public Flight UpdateFlight(string rab, DateTime departure, bool status)
        {
            var filter = Builders<Flight>.Filter.Eq(r => r.Plane.Rab, rab) &
                Builders<Flight>.Filter.Eq("Departure", departure);

            var options = new FindOneAndUpdateOptions<Flight, Flight> { ReturnDocument = ReturnDocument.After };
            var update = Builders<Flight>.Update.Set("Status", status);

            var flight = _flight.FindOneAndUpdate<Flight>(filter, update, options);

            return flight;
        }

        public void DeleteFlight(string rab, DateTime departure)
        {
            var filter = Builders<Flight>.Filter.Where(f => f.Plane.Rab == rab && f.Departure == departure);

            _flight.DeleteOne(filter);
        }
    }
}

