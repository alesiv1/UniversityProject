using RabbitMQ.Client;
using ScruperAPI.BL.Models;
using System;
using System.Text;

namespace ScruperAPI.BL
{
	public class RebbitMQManager
	{
		private readonly string _queueName = "siteData";
		private readonly string _hostName = "localhost";
		private readonly int _port = 5672;

		public void SendMessage(DataModel model)
		{
			var factory = new ConnectionFactory() { HostName = _hostName, Port = _port};
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
										 body: Encoding.UTF8.GetBytes($"{model.Url}*{model.Title}*{model.ShortDescription}"));
				}
			}
		}
	}
}
