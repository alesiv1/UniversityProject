﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

		public DataController(ILogger<DataController> logger, ApplicationDbContext context)
		{
			_logger = logger;
			_context = context;
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
					_context.OceanNetworks.Add(new OceanNetworkEntity()
					{
						Title = data.Title,
						Url = data.Url,
						ShortDescription = data.ShortDescription
					});
					_context.SaveChanges();
					_logger.LogInformation($"Data: URL - {data.Url} , Title - {data.Title}, Description - {data.ShortDescription} , was saved!");
				}
			}
		}
	}
}
