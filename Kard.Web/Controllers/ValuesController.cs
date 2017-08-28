using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Kard.Core.AppServices.Test;
using Kard.Core.IRepositories;
using Microsoft.Extensions.Caching.Memory;
using Kard.Core.AppServices.Cover;
using Kard.Core.Entities;

namespace Kard.Web.Controllers
{
    //[Route("api/[controller]")]
    [Produces("application/json")]
    [Route("api")]
    public class ValuesController : Controller
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ICoverAppService _coverAppService;
        public ValuesController(IMemoryCache memoryCache, ICoverAppService coverAppService)
        {
            _memoryCache = memoryCache;
            _coverAppService = coverAppService;
        }

        // GET api/menu
        [HttpGet("menu")]
        public string GetMenu()
        {
            return "";
        }

        [HttpPost("cover")]
        public CoverEntity GetCover()
        {
            var today = DateTime.Now.Date;
            string cacheKey = $"cover[{today.ToString("yyyyMMdd")}]";
            CoverEntity coverEntity = _memoryCache.GetOrCreate(cacheKey, (cacheEntry) => {
                cacheEntry.SetAbsoluteExpiration(today.AddDays(1));
                return _coverAppService.GetDateCover(today);
            });
            return coverEntity;
        }


      

        [HttpGet]
        public IEnumerable<string> Get()
        {
            var y= _coverAppService.GetDateCover(DateTime.Now).Id;
            return new string[] { "value1", "value2" };
        }



        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return  id.ToString();
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
