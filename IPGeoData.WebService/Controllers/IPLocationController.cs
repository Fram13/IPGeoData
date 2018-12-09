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
        private readonly IPLocationsManager _locationManager;
        private readonly ILogger<IPLocationController> _logger;

        public IPLocationController(IPLocationsManager locationManager, ILogger<IPLocationController> logger)
        {
            _locationManager = locationManager;
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
                return Ok(_locationManager.GetLocation(ipAddress));
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