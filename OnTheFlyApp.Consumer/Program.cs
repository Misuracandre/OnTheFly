using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OnTheFly.Models;
using OnTheFlyApp.Consumer;
using OnTheFlyApp.SaleService.config;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        using var connection = factory.CreateConnection();

        var taskInsertSale = Consumer(connection, "Sales");
        var taskInsertReservation = Consumer(connection, "Reservations");

        await Task.WhenAny(taskInsertSale, taskInsertReservation);        
    }

    static async Task Consumer(IConnection connection, string queueName)
    {
        using (var channel = connection.CreateModel())
        {
            channel.QueueDeclare(queue: queueName,
                        durable: false,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {

                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var deserialize = JsonConvert.DeserializeObject<Sale>(message);

                new InsertService().Insert(deserialize);
                Console.WriteLine($"Nova {queueName} foi recebida");
            };

            channel.BasicConsume(queueName, true, consumer);
            await Task.Delay(-1);
        }
    }
}