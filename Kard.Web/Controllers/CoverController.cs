using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Kard.Core.Entities;
using Kard.Extensions;
using Kard.Core.AppServices.Cover;

namespace Kard.Web.Controllers
{
    [Produces("application/json")]
    [Route("api/cover")]
    public class CoverController : Controller
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ICoverAppService _coverAppService;
        public CoverController(IMemoryCache memoryCache, ICoverAppService coverAppService)
        {
            _memoryCache = memoryCache;
            _coverAppService = coverAppService;
        }

        // GET: api/Cover
        //[HttpGet]
        //public CoverEntity Get()
        //{
        //    var today = DateTime.Now.Date;
        //    string cacheKey = $"cover[{today.ToString("yyyyMMdd")}]";
        //    CoverEntity coverEntity = _memoryCache.GetOrCreate(cacheKey, (cacheEntry) => {
        //        cacheEntry.SetAbsoluteExpiration(today.AddDays(1));
        //        return _coverAppService.GetDateCover(today);
        //    });
        //    return coverEntity;
        //}

        //// GET: api/Cover/5
        //[HttpGet("{id}", Name = "Get")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        // POST: api/Cover
        [HttpPost]
        public CoverEntity Post()
        {
            var today = DateTime.Now.Date;
            string cacheKey = $"cover[{today.ToString("yyyyMMdd")}]";
            CoverEntity coverEntity = _memoryCache.GetOrCreate(cacheKey, (cacheEntry) => {
                cacheEntry.SetAbsoluteExpiration(today.AddDays(1));
                return _coverAppService.GetDateCover(today);
            });
            return coverEntity;
        }

        //// PUT: api/Cover/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE: api/ApiWithActions/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}