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
using System.Runtime.ConstrainedExecution;
using System.Linq;

namespace OnTheFlyApp.SaleService.Services
{
    public class SalesService
    {
        private readonly IMongoCollection<Sale> _sale;
        private readonly IMongoCollection<Sale> _reservation;
        private readonly IMongoCollection<Sale> _flight;
        private readonly IMongoCollection<Sale> _passenger;
        private readonly IMongoCollection<Sale> _saleDeactivated;
        private readonly Util _util;
        private readonly ConnectionFactory _factory;

        public SalesService(ISaleServiceSettings settings, Util util, ConnectionFactory factory)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.Database);
            _sale = database.GetCollection<Sale>(settings.SaleCollection);
            _flight = database.GetCollection<Sale>(settings.FlightCollection);
            _passenger = database.GetCollection<Sale>(settings.PassengerCollection);
            _reservation = database.GetCollection<Sale>(settings.ReservationCollection);
            _saleDeactivated = database.GetCollection<Sale>(settings.Database);
            _util = util;
            _factory = factory;
        }

        public List<Sale> GetAll()
        {
            return _sale.Find<Sale>(s => true).ToList();
        }

        public Sale GetByIdentifier(string rab, DateTime schedule, string cpf)
        {
            Sale s = _sale.Find(s => s.Flight.Schedule == schedule && 
                                        s.Flight.Plane.Rab == rab &&
                                        s.Passengers[0].Cpf == cpf).FirstOrDefault();
            if (s == null) return null;
            return s;
        }

        public bool PostMQMessage([FromBody] Sale sale, bool reservation)
        {
            string queueName;
            if (reservation) queueName = "Reservations";
            else queueName = "Sales";

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

        public async Task<ActionResult> Create(SaleDTO sale)
        {
            //numero de vendas nao exceda a capacidade do aviao
            //garantir que o mesmo cpf nao apareca na lista de passageiros
            //verificar se o passageiro nao esta restrito, cancelar venda de verdadeiro
            //primeiro passageiro deve ser maior de 18 anos, se menor de 18, cancela a venda

            Sale s = new Sale(sale);
            HttpClient saleClient = new HttpClient();
            try
            {
                var dt = sale.Flight.Schedule.ToString("yyyy-MM-THH:mm:ss.FFF+00:00");
                //HttpResponseMessage response = await saleClient.GetAsync("https://localhost:5003/api/Flights/" + sale.Flight.Plane.Rab + "/" + sale.Flight.Schedule);
                HttpResponseMessage response = await saleClient.GetAsync("https://localhost:5003/api/Flights/AAG/2023-05-09T03%3A04%3A31.966%2B00%3A00");
                response.EnsureSuccessStatusCode();
                string flightJson = await response.Content.ReadAsStringAsync();
                s.Flight = JsonConvert.DeserializeObject<Flight>(flightJson);
            }
            catch (Exception)
            {

                throw;
            }

            try
            {
                HttpResponseMessage response = await saleClient.GetAsync("https://localhost:5000/api/PassengersService/" + sale.Passengers[0].Cpf);
                response.EnsureSuccessStatusCode();
                string flightJson = await response.Content.ReadAsStringAsync();
                s.Passengers[0] = new(JsonConvert.DeserializeObject<Passenger>(flightJson));
            }
            catch (Exception)
            {

                throw;
            }

            //Verifica se o numero de vendas nao excede a capacidade do aviao
            if (s.Flight.Plane.Capacity > s.Flight.Sales)
            {
                throw new InvalidOperationException("Capacidade do avião excedida");
            }

            //Verifica se o mesmo CPF nao aparece na lista de passageiros
            var passengerCpfs = new List<string>();
            foreach (var passenger in s.Passengers)
            {
                if (passengerCpfs.Contains(passenger.Cpf))
                {
                    throw new InvalidOperationException("O mesmo CPF não pode aparecer na lista de passageiros");
                }
                passengerCpfs.Add(passenger.Cpf);
            }

            //Verificar se o passageiro nao esta restrito
            if (s.Passengers.Any(p => p.Status == false))
            {
                throw new InvalidOperationException("A venda foi cancelada devido a um passageiro restrito");
            }

            //Verificar se primeiro passageiro é maior de 18 anos
            DateTime i = DateTime.Parse(s.Passengers[0].DtBirth).AddYears(18);
            if (i > DateTime.Now)
            {
                throw new InvalidOperationException("O primeiro passageiro deve ser maior de 18 anos");
            }

            if (s.Reserved)
            {
                if (!PostMQMessage(s, true)) return null;

                return new ContentResult() { Content = "Reserva inserida na fila", StatusCode = StatusCodes.Status200OK };
            }
            else
            {
                if (!PostMQMessage(s, false)) return null;
                return new ContentResult() { Content = "Venda inserida na fila", StatusCode = StatusCodes.Status200OK };
            }
        }
    }
}
