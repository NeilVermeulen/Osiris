using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExchangeRatesBankConnector.DTOs
{
	public class ExchangeRate
	{
		public string threeDigitAbreviation { get; set; }
		public double value { get; set; }
	}
}
