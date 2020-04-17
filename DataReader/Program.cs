using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;

namespace DataReader
{
	class Program
	{
		static void Main(string[] args)
		{
			while (true)
			{
                Thread.Sleep(10000);
                var mess = RabbitMQManager.ReadMesage();
                if (!string.IsNullOrEmpty(mess))
                {
                    Console.WriteLine(mess);
                }
            }
		}
	}

	public static class RabbitMQManager
	{
        public static string ReadMesage()
        {
            var message = "";
            var factory = new ConnectionFactory() { HostName = "localhost", Port = 5672};
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "data",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    message = Encoding.UTF8.GetString(body.ToArray());
                };
                channel.BasicConsume(queue: "data",
                                     autoAck: true,
                                     consumer: consumer);
            }
            return message;
        }
    }
}
