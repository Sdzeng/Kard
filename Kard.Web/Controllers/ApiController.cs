using Kard.Core.AppServices.Default;
using Kard.Core.Dtos;
using Kard.Core.Entities;
using Kard.Core.IRepositories;
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

    [Produces("application/json")]
    [Route("[controller]")]
    public class ApiController : BaseController
    {

        private readonly IDefaultRepository _defaultRepository;
        public ApiController(
            ILogger<ApiController> logger,
            IMemoryCache memoryCache,
            IDefaultRepository defaultRepository,
            IKardSession kardSession) : base(logger, memoryCache, kardSession)
        {

            _defaultRepository = defaultRepository;
        }

        /// <summary>
        /// 获取菜单
        /// </summary>
        /// <returns></returns>
        [HttpGet("menu")]
        public string GetMenu()
        {
            return "";
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
        [HttpGet("getpicture")]
        public IEnumerable<TopMediaDto> GetPicture()
        {
            var aWeekAgo = DateTime.Now.Date.AddYears(-7);
            return _defaultRepository.GetTopMediaPicture(aWeekAgo);
        }

        /// <summary>
        /// 获取记录
        /// </summary>
        /// <returns></returns>
        [HttpGet("getessay")]
        public IEnumerable<TopMediaDto> GetEssay()
        {
            var aWeekAgo = DateTime.Now.Date.AddMonths(-7);
            return _defaultRepository.GetTopMediaPicture(aWeekAgo);
        }

        /// <summary>
        /// 添加纪录
        /// </summary>
        /// <param name="essayEntity"></param>
        [HttpPost("addessay")]
        public void AddEssay(EssayEntity essayEntity) {
            _defaultRepository.TransExecute((conn, trans) =>
            {
                var resultDto = _defaultRepository.CreateAndGetId<EssayEntity, long>(essayEntity, conn, trans);
 
                return resultDto.Result;
            });
          

        }



        #region test
        /// <summary>
        /// 测试1
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            var y = _defaultRepository.GetDateCover(DateTime.Now).Id;
            return new string[] { "value1", "value2" };
        }


        /// <summary>
        /// GET api/values/5
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return id.ToString();
            // return "value";
        }

 
        /// <summary>
        /// POST api/values
        /// </summary>
        /// <param name="value"></param>
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

  
        /// <summary>
        /// PUT api/values/5
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

 
        /// <summary>
        /// DELETE api/values/5
        /// </summary>
        /// <param name="id"></param>
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        #endregion
    }
}