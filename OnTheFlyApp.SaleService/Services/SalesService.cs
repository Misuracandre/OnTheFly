using OnTheFly.Models.Dto;
using OnTheFly.Models;
using Utility;
using MongoDB.Driver;
using OnTheFlyApp.SaleService.config;
using Microsoft.AspNetCore.Identity;
using RabbitMQ.Client;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using Newtonsoft.Json;
using System.Net;

namespace OnTheFlyApp.SaleService.Services
{
    public class SalesService
    {
        private readonly IMongoCollection<Sale> _sale;
        private readonly IMongoCollection<Sale> _reservation;
        private readonly IMongoCollection<Sale> _saleDeactivated;
        private readonly Util _util;
        private readonly ConnectionFactory _factory;

        public SalesService(ISaleServiceSettings settings, Util util, ConnectionFactory factory)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _sale = database.GetCollection<Sale>(settings.SaleCollectionName);
            _reservation = database.GetCollection<Sale>(settings.ReservationCollectionName);
            _saleDeactivated = database.GetCollection<Sale>(settings.SaleDeactivedCollectionName);
            _util = util;
            _factory = factory;
        }

        public List<Sale> GetAll()
        {
            return _sale.Find<Sale>(s => true).ToList();
        }

        public Sale GetByIdentifier(string rab, DateTime schedule, string cpf)
        {
            Sale s = _sale.Find(s => s.Flight.Departure == schedule && 
                                        s.Flight.Plane.Rab == rab &&
                                        s.Passengers[0].Cpf == cpf).FirstOrDefault();
            if (s == null) return null;
            return s;
        }

        public bool PostMQMessage([FromBody] Sale sale, bool reservation)
        {
            string queueName;
            if (reservation) queueName = "Reservation";
            else queueName = "sales";

            using (var connection = _factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {

                    channel.QueueDeclare(
                        queue: queueName,
                        durable: false,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null
                        );

                    var stringfieldMessage = JsonConvert.SerializeObject(sale);
                    var bytesMessage = Encoding.UTF8.GetBytes(stringfieldMessage);

                    channel.BasicPublish(
                        exchange: "",
                        routingKey: queueName,
                        basicProperties: null,
                        body: bytesMessage
                        );
                }
            }
            return true;
        }

        public Sale Create(SaleDTO sale)
        {
            //numero de vendas nao exceda a capacidade do aviao
            //garantir que o mesmo cpf nao apareca na lista de passageiros
            //verificar se o passageiro nao esta restrito, cancelar venda de verdadeiro
            //primeiro passageiro deve ser maior de 18 anos, se menor de 18, cancela a venda
            Sale s = new Sale(sale);
            if (s.Reserved)
            {
                if (!PostMQMessage(s, true)) return null;

                while (s == null)
                {
                    s = _reservation.Find(s => s.Flight.Departure == s.Flight.Departure &&
                                           s.Flight.Plane.Rab == s.Flight.Plane.Rab &&
                                           s.Passengers[0].Cpf == s.Passengers[0].Cpf).FirstOrDefault();
                    if (s == null) return null;
                }
                return s;
            }
            else
            {
                if (!PostMQMessage(s, false)) return null;
                while (s == null)
                {
                    s = _sale.Find(s => s.Flight.Departure == s.Flight.Departure &&
                                       s.Flight.Plane.Rab == s.Flight.Plane.Rab &&
                                       s.Passengers[0].Cpf == s.Passengers[0].Cpf).FirstOrDefault();
                    if (s == null) return null;
                }
                return s;
            }

            //_sale.InsertOne(s);
            return s;
        }
    }
}
