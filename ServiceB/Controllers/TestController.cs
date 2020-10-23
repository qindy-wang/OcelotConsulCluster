using Consul;
using ConsulUtility;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ServiceB.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        public readonly ConsulOption _consulOption;
        private readonly IHttpClientFactory _httpClientFactory;
        public TestController(ConsulOption consulOption, IHttpClientFactory httpClientFactory)
        {
            _consulOption = consulOption;
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<string> Get()
        {
            using (var consulClient = new ConsulClient(a => a.Address = new Uri(_consulOption.Address)))
            {
                var services = consulClient.Catalog.Service("servicea").Result.Response;
                if (services != null && services.Any())
                {
                    var service = services.First();//services.ElementAt(new Random().Next(services.Count()));

                    var client = _httpClientFactory.CreateClient();
                    var response = await client.GetAsync($"http://{service.ServiceAddress}:{service.ServicePort}/test/get");
                    var result = await response.Content.ReadAsStringAsync();
                    return result;
                }
            }
            return $"Service: {_consulOption.ServiceName} not found";
        }
    }
}
