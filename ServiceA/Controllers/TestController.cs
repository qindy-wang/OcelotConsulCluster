using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace ServiceA.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        public readonly IConfiguration _configuration;

        public TestController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public string Get()
        {
            return $"{_configuration["Consul:ServiceName"]}";
        }

        [HttpGet]
        public async Task<string> LongTime()
        {
            await Task.Delay(5000);
            return $"{_configuration["Consul: ServiceName"]} Finished";
        }
    }
}
