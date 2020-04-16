using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using AngleSharp.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Scruper
{
	public class OceanNetworksScrubber
	{
		private string _baseSiteUrl = "https://www.oceannetworks.ca/news/stories";
		public string[] QueryTerms { get; } = { "Ocean", "Nature", "Pollution" };

		public IEnumerable<DataModel> GetNewsOnPage(int pageNumber = 0)
		{
			IHtmlDocument document = ScrapeWebsite(pageNumber).Result;
			List<DataModel> news = new List<DataModel>();
			IEnumerable<IElement> articleData = document.All.Where(x => x.ClassName == "onc_clear_left");
			foreach (var item in articleData)
			{
				news.Add(CleanUpResults(item));
			}
			return news;
		}
		#region Private Methida
		private async Task<IHtmlDocument> ScrapeWebsite(int pageNumber = 0)
		{
			var parameters = "";
			if(pageNumber > 0)
			{
				parameters = $"?page={pageNumber}";
			}
			CancellationTokenSource cancellationToken = new CancellationTokenSource();
			HttpClient httpClient = new HttpClient();
			HttpResponseMessage request = await httpClient.GetAsync(_baseSiteUrl+parameters);
			cancellationToken.Token.ThrowIfCancellationRequested();

			Stream response = await request.Content.ReadAsStreamAsync();
			cancellationToken.Token.ThrowIfCancellationRequested();

			HtmlParser parser = new HtmlParser();
			IHtmlDocument document = parser.ParseDocument(response);
			return document;
		}
		private DataModel CleanUpResults(IElement result)
		{
			string htmlResult = result.InnerHtml.ReplaceFirst("        <span class=\"field-content\"><div><a href=\"", "https://www.oceannetworks.ca");
			htmlResult = htmlResult.ReplaceFirst("\">", "*");
			htmlResult = htmlResult.ReplaceFirst("</a>", "");
			//htmlResult = htmlResult.ReplaceFirst("</a></div>\n<div class=\"article-title-top\">", "-");
			//htmlResult = htmlResult.ReplaceFirst("</div>\n<hr></span>  ", "");
			htmlResult = htmlResult.ReplaceFirst("<a href=\"", "");

			return SplitResults(htmlResult);
		}
		private DataModel SplitResults(string htmlResult)
		{
			string[] splitResults = htmlResult.Split('*');
			return new DataModel()
			{
				Url = splitResults[0],
				Title = splitResults[1]
			};
		}
		#endregion
	}

	public class DataModel
	{
		public string Url { get; set; }
		public string Title { get; set; }
	}
}
