using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using AngleSharp.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using ScruperAPI;

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
		public IEnumerable<DataModel> GetScrapeResults()
		{
			OceanNetworksScrubber networksScrubber = new OceanNetworksScrubber();
			RebbitMQManager mQManager = new RebbitMQManager();
			var data = new List<DataModel>();
			IEnumerable<DataModel> tempData = new List<DataModel>();
				tempData = networksScrubber.GetNewsOnPage(5);
				data.AddRange(tempData);
			foreach(var item in data)
			{
				mQManager.SendMessage(item);
				Thread.Sleep(10000);
			}
			return data;
		}	
	}
}
