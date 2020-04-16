using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using AngleSharp.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Scrubber.Controllers
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
			var rez = networksScrubber.GetNewsOnPage(1);
			return rez;
		}
	}
}
