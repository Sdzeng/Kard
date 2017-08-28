using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using Microsoft.Extensions.Caching.Memory;
using Kard.Extensions;

namespace Kard.Web.Controllers
{
    [Produces("application/json")]
    [Route("api")]
    public class HomeController : Controller
    {
        private IMemoryCache _memoryCache;
        public HomeController(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        [HttpGet]
        public IActionResult Index()
        {
            //string page = "index";
            //string cacheKey = $"pages[{page}]";
            //string htmlContent = _memoryCache.GetCache(cacheKey, () => GetHtmlContent(page),7*24*60*60);
            //return Content(htmlContent, "text/html;charset=utf-8");
            return File("/pages/index.html", "text/html;charset=utf-8");
        }

        [HttpGet]
        public IActionResult Error()
        {
            string page = "error";
            string cacheKey = $"pages[{page}]";
            string htmlContent = _memoryCache.GetCache(cacheKey, () => GetHtmlContent(page));
            return Content(htmlContent, "text/html;charset=utf-8");
        }



        private string GetHtmlContent(string page)
        {
            string pagePath = Path.Combine(Environment.CurrentDirectory, "Pages\\", page + ".html");
            using (StreamReader sr = new StreamReader(pagePath))
            {
                string htmlContent = sr.ReadToEnd();
                return htmlContent;
            }
        }
    }
}