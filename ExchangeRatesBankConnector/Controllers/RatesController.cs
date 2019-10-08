using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExchangeRatesBankConnector.DTOs;
using ExchangeRatesBankConnector.Tools.Network;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace ExchangeRatesBankConnector.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatesController : ControllerBase
    {
		IConfiguration _configuration;
		public RatesController(IConfiguration configuration)
		{
			_configuration = configuration;
		}
		
		[HttpGet]
        public ServiceResult Get()
        {
			string url = _configuration.GetSection("Bank").GetSection("URL").Value;
			APIConnector connector = new APIConnector(_configuration);
			ServiceResult result = connector.GetObjectFromRemote();

			return result;
        }

    }
}
