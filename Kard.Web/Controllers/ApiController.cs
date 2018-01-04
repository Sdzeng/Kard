using Kard.Core.AppServices.Default;
using Kard.Core.Dtos;
using Kard.Core.Entities;
using Kard.Extensions;
using Kard.Runtime.Session;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;

namespace Kard.Web.Controllers
{
    //[Route("api/[controller]")]
    [Produces("application/json")]
    [Route("api")]
    public class ApiController : BaseController
    {
         
        private readonly IDefaultAppService _defaultAppService;
        public ApiController(
            ILogger<ApiController> logger,
            IMemoryCache memoryCache,
            IDefaultAppService defaultAppService,
            IKardSession kardSession) : base(logger, memoryCache, kardSession)
        {
           
            _defaultAppService = defaultAppService;
        }


        [HttpGet("menu")]
        public string GetMenu()
        {
            return "";
        }

        [HttpPost("cover")]
        public CoverEntity GetCover()
        {
            var today = new DateTime(2017, 8, 1);// DateTime.Now.Date;
            string cacheKey = $"cover[{today.ToString("yyyyMMdd")}]";
            CoverEntity coverEntity = _memoryCache.GetOrCreate(cacheKey, (cacheEntry) =>
            {
                cacheEntry.SetAbsoluteExpiration(today.AddDays(1));
                return _defaultAppService.GetDateCover(today);
            });
            return coverEntity;
        }

        [HttpPost("topMediaPicture")]
        public IEnumerable<TopMediaDto> GetTopMediaPicture()
        {
            var aWeekAgo = DateTime.Now.Date.AddMonths(-7);
            return _defaultAppService.GetTopMediaPicture(aWeekAgo);
        }

 


        #region test
        [HttpGet]
        public IEnumerable<string> Get()
        {
            var y = _defaultAppService.GetDateCover(DateTime.Now).Id;
            return new string[] { "value1", "value2" };
        }



        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return id.ToString();
            // return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        #endregion
    }
}