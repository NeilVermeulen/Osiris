using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExchangeRatesBankConnector.DTOs
{
	public class ServiceResult
	{
		public List<ExchangeRate> rates { get; set; }
		public string Base { get; set; }
		public string date { get; set; }
	}
}
