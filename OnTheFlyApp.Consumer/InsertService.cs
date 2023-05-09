using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnTheFly.Models.Dto;
using OnTheFly.Models;
using MongoDB.Driver;
using RabbitMQ.Client;
using OnTheFlyApp.SaleService.config;

namespace OnTheFlyApp.Consumer
{
    public class InsertService
    {
        private readonly IMongoCollection<Sale> _sale;
        private readonly IMongoCollection<Sale> _reservation;
        public InsertService()
        {
            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("dbSaleMongodb");
            _sale = database.GetCollection<Sale>("dbSaleMongodb");
            _reservation = database.GetCollection<Sale>("dbReservationMongodb");
        }
        
        public Sale Insert(Sale sale)
        {
            if (!sale.Reserved && !sale.Sold)
                return null;
            
            if (sale.Reserved)
            {
                _reservation.InsertOne(sale);
                return sale;
            }
            _sale.InsertOne(sale);
            return sale;
        }
    }
}
