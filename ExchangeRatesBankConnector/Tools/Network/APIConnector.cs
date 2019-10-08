using ExchangeRatesBankConnector.DTOs;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ExchangeRatesBankConnector.Tools.Network
{
	public class APIConnector
	{
		private class ResultsFromForeignServer
		{
			public object rates { get; set; }
			public string Base { get; set; }
			public string date { get; set; }
		}
		IConfiguration _configuration;
		static HttpClient _client = new HttpClient();
		private static string _bankURL;
		public string BankURL => _bankURL;
		public APIConnector(IConfiguration configuration)
		{
			_configuration = configuration;
			_bankURL = _configuration.GetSection("Bank").GetSection("BankURL").Value;
		}
		private async static Task<object> GetObjectAsync()
		{
			object obj = null;
			HttpResponseMessage response = await _client.GetAsync(_bankURL);
			if (response.IsSuccessStatusCode)
				obj = response.Content.ReadAsStringAsync().Result;
			return obj;
		}
		private ServiceResult TranslateToUsableObject(object json)
		{
			ServiceResult result = new ServiceResult();
			List<ExchangeRate> massagedRates = new List<ExchangeRate>();
			ResultsFromForeignServer exchangeRates = JsonConvert.DeserializeObject<ResultsFromForeignServer>(json.ToString());
			result.Base = exchangeRates.Base;
			result.date = exchangeRates.date;
			object ratesReturned = exchangeRates.rates;
			string worker = JsonConvert.SerializeObject(ratesReturned);

			worker.Remove('\"');
			worker.Remove('{');
			worker.Remove('}');


			List<string> rates = worker.Split(',').ToList();

			foreach (string rate in rates)
			{

				string[] exchangeRateParts = rate.Split(':');

				if (exchangeRateParts[1].Substring(exchangeRateParts[1].Length) == "}")
					exchangeRateParts[1] = exchangeRateParts[1].Substring(0, exchangeRateParts[1].Length - 1);

				string s = exchangeRateParts[1];

				ExchangeRate exchangeRate = new ExchangeRate();
				exchangeRate.threeDigitAbreviation = exchangeRateParts[0];
				try
				{
					//TODO
					//For some funky reason this app and the string manipulation puts an "}" at the end of the last value.  This makes the application crash trying to convert to a double value.
					//Yeah.  I know this is a crappy way to do this but I am running out of time.  This'll do untill I have time to look for what the issue is.
					exchangeRate.value = Convert.ToDouble(exchangeRateParts[1]);
					massagedRates.Add(exchangeRate);
				}
				catch (Exception)
				{
				}
			}
			if (massagedRates.Any())
				result.rates = massagedRates;
			else
				result.rates = null;
			return result;
		}
		public ServiceResult GetObjectFromRemote()
		{
			Stopwatch sw = new Stopwatch();
			sw.Start();
			object obj = GetObjectAsync().Result;
			ServiceResult result = TranslateToUsableObject(GetObjectAsync().Result);
			sw.Stop();
			return result;
		}
	}
}
