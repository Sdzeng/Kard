using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Kard.Web.Controllers
{
    [Route("Error")]
    public class ErrorController : Controller
    {
        private readonly ILogger _logger;
        public ErrorController(ILoggerFactory loggerFactory)
        {
            _logger=loggerFactory.CreateLogger(this.GetType());
        }

        public IActionResult Index()
        {
            return File("/pages/error.htm", "text/html;charset=utf-8");
        }
    }
}