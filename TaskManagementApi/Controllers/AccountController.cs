using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TaskManagementApi.Models;
using TaskManagementApi.Services;

namespace TaskManagementApi.Controllers
{
    [ApiController]
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> log;
        private readonly IConfiguration config;
        private readonly JWTConfig jwtConfig;
        private readonly JWTBase jwtHelper;

        public AccountController(
            ILogger<AccountController> logger,
            IOptions<JWTConfig> jwtOptions,
            IConfiguration configuration,
            JWTBase jWTServices
        )
        {
            log = logger;
            jwtConfig = jwtOptions.Value;
            config = configuration;
            jwtHelper = jWTServices;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
