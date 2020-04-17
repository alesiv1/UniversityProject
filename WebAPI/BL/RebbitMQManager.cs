using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using WebAPI.BL.Models;

namespace WebAPI
{
	public class RebbitMQManager
	{
        private readonly string _queueName = "siteData";
        private readonly string _hostName = "localhost";
        private readonly int _port = 5672;
        public DataModel ReadMesage()
        {
            var message = "";
            var factory = new ConnectionFactory() { HostName = _hostName, Port = _port };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: _queueName,
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
                channel.BasicConsume(queue: _queueName,
                                     autoAck: true,
                                     consumer: consumer);
            }
            return SplitResults(message);
        }
		#region Private Methods
		private DataModel SplitResults(string message)
        {
            if(message.Contains("I send"))
            {
                return new DataModel()
                {
                    Title = message,
                };
            }
            if (string.IsNullOrEmpty(message))
            {
                return new DataModel();
            }
            string[] splitResults = message.Split('*');
            return new DataModel()
            {
                Url = splitResults[0],
                Title = splitResults[1],
                ShortDescription = splitResults[2]
            };
        }
		#endregion
	}
}
