using IPGeoData.WebService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Net;

namespace IPGeoData.WebService.Controllers
{
    [Produces("application/json")]
    [Route("IPLocation")]
    public class IPLocationController : Controller
    {
        private readonly IPLocationsManager _manager;
        private readonly ILogger<IPLocationController> _logger;

        public IPLocationController(IPLocationsManager manager, ILogger<IPLocationController> logger)
        {
            _manager = manager;
            _logger = logger;
        }

        [HttpGet("{ip}")]
        public IActionResult Get(string ip)
        {
            if (!IPAddress.TryParse(ip, out var ipAddress))
            {
                return BadRequest(new
                {
                    Error = "IP address has invalid format."
                });
            }

            try
            {
                return Ok(_manager.Get(ipAddress));
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "IP: {0}", ip);

                return BadRequest(new
                {
                    Error = "An internal server error occurred."
                });
            }
        }
    }
}