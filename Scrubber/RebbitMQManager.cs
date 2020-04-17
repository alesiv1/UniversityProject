using Newtonsoft.Json;
using RabbitMQ.Client;
using Scruper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScruperAPI
{
	public class RebbitMQManager
	{
		private readonly string _queueName = "data";
		private readonly string _hostName = "localhost";

		public void SendMessage(DataModel model)
		{
			var factory = new ConnectionFactory() { HostName = _hostName, Port = 5672};
			using (var connection = factory.CreateConnection())
			{
				using (var channel = connection.CreateModel())
				{
					channel.QueueDeclare(queue: _queueName, 
										 durable: false, 
										 exclusive: false, 
										 autoDelete: false, 
										 arguments: null);

					channel.BasicPublish(exchange: string.Empty,
										 routingKey: _queueName,
										 basicProperties: null,
										 body: Encoding.UTF8.GetBytes($"URL - {model.Url} , Title - {model.Title}"));
				}
			}
		}
	}
}
