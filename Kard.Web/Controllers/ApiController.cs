using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Kard.Core.AppServices.Cover;
using Kard.Core.Entities;
using Kard.Core.IRepositories;
using Kard.Core.AppServices.Media;

namespace Kard.Web.Controllers
{
    //[Route("api/[controller]")]
    [Produces("application/json")]
    [Route("api")]
    public class ApiController : Controller
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ICoverAppService _coverAppService;
        private readonly IMediaAppService _mediaAppService;
        public ApiController(
            IMemoryCache memoryCache,
            ICoverAppService coverAppService,
            IMediaAppService mediaAppService)
        {
            _memoryCache = memoryCache;
            _coverAppService = coverAppService;
            _mediaAppService = mediaAppService;
        }


        [HttpGet("menu")]
        public string GetMenu()
        {
            return "";
        }

        [HttpPost("cover")]
        public CoverEntity GetCover()
        {
            var today = new DateTime(2017,8,1);// DateTime.Now.Date;
            string cacheKey = $"cover[{today.ToString("yyyyMMdd")}]";
            CoverEntity coverEntity = _memoryCache.GetOrCreate(cacheKey, (cacheEntry) => {
                cacheEntry.SetAbsoluteExpiration(today.AddDays(1));
                return _coverAppService.GetDateCover(today);
            });
            return coverEntity;
        }

        [HttpPost("topMediaPicture")]
        public IEnumerable<MediaEntity> GetTopMediaPicture()
        {
            var aWeekAgo =  DateTime.Now.Date.AddMonths(-7);
            return _mediaAppService.GetTopMediaPicture(aWeekAgo);
        }




        [HttpGet]
        public IEnumerable<string> Get()
        {
            var y = _coverAppService.GetDateCover(DateTime.Now).Id;
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
    }
}