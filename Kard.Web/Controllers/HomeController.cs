using Kard.Core.Dtos;
using Kard.Core.Entities;
using Kard.Core.IRepositories;
using Kard.Runtime.Session;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Kard.Web.Controllers
{
    [AllowAnonymous]
    [Produces("application/json")]
    [Route("api/home")]
    public class HomeController : BaseController
    {
        private readonly IDefaultRepository _defaultRepository;
        public HomeController (ILogger<HomeController> logger,
            IMemoryCache memoryCache,
            IDefaultRepository defaultRepository,
            IKardSession kardSession) : base(logger, memoryCache, kardSession)
        {
            _defaultRepository = defaultRepository;
        }


        /// <summary>
        /// 获取封面
        /// </summary>
        /// <returns></returns>
        [HttpGet("cover")]
        public CoverEntity GetCover()
        {
            var today = DateTime.Now.Date;
            string cacheKey = $"homeCover[{today.ToString("yyyyMMdd")}]";
            CoverEntity coverEntity = _memoryCache.GetOrCreate(cacheKey, (cacheEntry) =>
            {
                cacheEntry.SetAbsoluteExpiration(today.AddDays(1));
                return _defaultRepository.GetDateCover(today);
            });
            return coverEntity;
        }


        /// <summary>
        /// 获取单品图片
        /// </summary>
        /// <returns></returns>
        [HttpGet("pictures")]
        public IEnumerable<TopMediaDto> GetPicture()
        {
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            try
            {
                //var aWeekAgo = DateTime.Now.Date.AddYears(-7);
                return _defaultRepository.GetHomeMediaPicture(12);
            }
            finally {
                sw.Stop();
                _logger.LogDebug($"GetPicture耗时：{sw.ElapsedMilliseconds}");
            }
        }

    }
}