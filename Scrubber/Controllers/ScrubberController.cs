using System;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Scruper.BL.Scrapers;
using ScruperAPI.BL;

namespace Scruper.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class ScrubberController : ControllerBase
	{
		private readonly ILogger<ScrubberController> _logger;

		public ScrubberController(ILogger<ScrubberController> logger)
		{
			_logger = logger;
		}

		[HttpGet]
		public void GetScrapeResults()
		{
			OceanNetworksScrubber networksScrubber = new OceanNetworksScrubber();
			RebbitMQManager mQManager = new RebbitMQManager();
			var data = networksScrubber.GetNewsOnPage();
			int dataCount = 0;
			foreach(var item in data)
			{
				try
				{
					mQManager.SendMessage(item);
					_logger.LogInformation($"URL - {item.Url} , Title - {item.Title}, Description - {item.ShortDescription}");
					dataCount++;
				}
				catch(Exception ex)
				{
					_logger.LogError(ex.Message);
				}
				Thread.Sleep(10000);
			}
			mQManager.SendMessage($"I send {dataCount} items!");
			_logger.LogInformation($"I send {dataCount} items!");
		}	
	}
}
