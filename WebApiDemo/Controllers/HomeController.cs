using Microsoft.AspNetCore.Mvc;
using MiHome.Net.Service;

namespace WebApiDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMiHomeDriver miHomeDriver;

        public HomeController(ILogger<HomeController> logger, IMiHomeDriver miHomeDriver)
        {
            _logger = logger;
            this.miHomeDriver = miHomeDriver;
        }

        [HttpGet(Name = "Index")]
        public async Task<string> Index()
        {
            await miHomeDriver.Cloud.LoginAsync();
            //�г����м�ͥ
            var homeList = await miHomeDriver.Cloud.GetHomeListAsync();
            return "1";
        }
    }
}
