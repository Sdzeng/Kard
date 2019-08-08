using Kard.Extensions;
using Kard.Runtime.Session;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace Kard.Web.Controllers
{
    public abstract class BaseController : ControllerBase
    {
        protected readonly ILogger _logger;
        protected readonly IMemoryCache _memoryCache;
        protected readonly IKardSession _kardSession;


        public BaseController(ILogger<BaseController> logger,IMemoryCache memoryCache=null, IKardSession kardSession=null)
        {
            _logger = logger;
            _memoryCache = memoryCache;
            _kardSession = kardSession;
        }

        protected string GetPageFile(string page)
        {
            //File("/pages/user.htm", "text/html;charset=utf-8");
            string cacheKey = $"pages[{page}]";
            string htmlContent = _memoryCache.GetCache(cacheKey, () => GetHtmlContent(page));
            return htmlContent;
        }

        private string GetHtmlContent(string page)
        {
            string pagePath = Path.Combine(Environment.CurrentDirectory, "wwwroot", page);
            using (StreamReader sr = new StreamReader(pagePath))
            {
                string htmlContent = sr.ReadToEnd();
                return htmlContent;
            }
        }

   



    }
}
