using Kard.DI;
using Kard.Extensions;
using Kard.Runtime.Session;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using NLog;
using System;
using System.IO;

namespace Kard.Web.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public abstract class BaseController : ControllerBase
    {
        protected readonly Logger _logger=LogManager.GetCurrentClassLogger(); //KardIoc.GetService<ILogger<string>>();
        protected readonly IMemoryCache _memoryCache;
        protected readonly IKardSession _kardSession;


        public BaseController(IMemoryCache memoryCache=null, IKardSession kardSession=null)
        {
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
