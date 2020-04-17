using System;
using System.Threading;
using EntityGraphQL;
using EntityGraphQL.Schema;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebAPI.Data;
using WebAPI.Data.Entities;

namespace WebAPI.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class DataController : ControllerBase
	{
		private readonly ILogger<DataController> _logger;
		private readonly ApplicationDbContext _context;
		private readonly SchemaProvider<ApplicationDbContext> _schemaProvider;

		public DataController(ILogger<DataController> logger, ApplicationDbContext context, SchemaProvider<ApplicationDbContext> schemaProvider)
		{
			_logger = logger;
			_context = context;
			_schemaProvider = schemaProvider;
		}

		[HttpGet]
		public void Get()
		{
			RebbitMQManager mQManager = new RebbitMQManager();
			while (true)
			{
				Thread.Sleep(10000);
				var data = mQManager.ReadMesage();
				if (!string.IsNullOrWhiteSpace(data.Title))
				{
					if(string.IsNullOrWhiteSpace(data.Url) && string.IsNullOrWhiteSpace(data.ShortDescription))
					{
						_logger.LogInformation($"{data.Title}. And WebAPI save some of them!");
						break;
					}
					QueryRequest query = new QueryRequest()
					{
						Query =
						{

						}
					};
					_schemaProvider.ExecuteQuery(query, _context, null, null);
					//_context.OceanNetworks.Add(new OceanNetworkEntity()
					//{
					//	Title = data.Title,
					//	Url = data.Url,
					//	ShortDescription = data.ShortDescription
					//});
					_context.SaveChanges();
					_logger.LogInformation($"Data: URL - {data.Url} , Title - {data.Title}, Description - {data.ShortDescription} , was saved!");
				}
			}
		}
	}
}
