using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Kard.Core.Dtos;
using Kard.Core.Entities;
using Kard.Core.IRepositories;
using Kard.Extensions;
using Kard.Runtime.Security.Authentication.WeChat;
using Kard.Runtime.Session;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Kard.Web.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [Authorize(AuthenticationSchemes = WeChatAppDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("essay")]
    public class EssayController : BaseController
    {
        private readonly IHostingEnvironment _env;
        private readonly IDefaultRepository _defaultRepository;
        public EssayController(IHostingEnvironment env,
            ILogger<EssayController> logger,
            IMemoryCache memoryCache,
            IDefaultRepository defaultRepository,
            IKardSession kardSession) : base(logger, memoryCache, kardSession)
        {
            _env = env;
            _defaultRepository = defaultRepository;

        }

        #region essay
        /// <summary>
        /// 获取单品信息
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("{id}")]
        public ResultDto GetInfo(long id)
        {
            //单品信息
            var essayEntity = _defaultRepository.GetEssay(id,_kardSession.UserId);
            var resultDto = new ResultDto();
            resultDto.Result = essayEntity != null;
            resultDto.Data = essayEntity;
            //评论 喜欢

            //增加阅读数
            Task.Run(() =>
            {
                var result = _defaultRepository.UpdateBrowseNum(id);
                if (!result)
                {
                    _logger.LogError($"统计：单品{id}增加阅读数失败");
                }
            });

            return resultDto;
        }

        /// <summary>
        /// 上次文件
        /// </summary>
        /// <returns></returns>

        [HttpPost("uploadmedia")]
        //[Consumes("multipart/form-data")]
        //[RequestSizeLimit(100_000_000)]
        public ResultDto UploadMedia(IFormFile mediaFlie)
        {
            var result = new ResultDto();
            if (mediaFlie == null) mediaFlie = Request.Form.Files[0];

            var now = DateTime.Now;
            string webRootPath = _env.WebRootPath;

            string newFolder = Path.Combine("user", _kardSession.UserId.ToString(), "media", now.ToString("yyyyMMdd"));
            string newPath = Path.Combine(webRootPath, newFolder);
            if (!Directory.Exists(newPath))
            {
                Directory.CreateDirectory(newPath);
            }

            if (mediaFlie.Length > 0)
            {
                string fileName = now.ToString("ddHHmmssffff");
                string fileExtension = Path.GetExtension(mediaFlie.FileName.Trim('"')).ToLower(); // Path.GetExtension(ContentDispositionHeaderValue.Parse(mediaFlie.ContentDisposition).FileName.Trim('"'));
                string fullPath = Path.Combine(newPath, fileName + fileExtension);
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    mediaFlie.CopyTo(stream);
                }
                result.Result = true;
                result.Data = new { FileUrl = Path.Combine(newFolder, fileName).Replace("\\", "/"), FileExtension = fileExtension };
                return result;
            }

            result.Result = false;
            result.Message = "上传失败";
            return result;
        }


        /// <summary>
        /// 添加纪录
        /// </summary>
        /// <param name="essayEntity"></param>
        /// <param name="mediaList"></param>
        [HttpPost("add")]
        public ResultDto AddEssay(EssayEntity essayEntity, IEnumerable<MediaEntity> mediaList)
        {
            /*private static readonly Regex _regex = new Regex(@"(?'group1'#)([^#]+?)(?'-group1'#)");
             if ((!this.EssayContent.IsNullOrEmpty()) && _regex.IsMatch(this.EssayContent))
                {
                    var matchCollection = _regex.Matches(this.EssayContent);
                    return matchCollection.First()?.Value.Replace("#","");
                }*/

            var createUserId = _kardSession.UserId.Value;
            IEnumerable<TagEntity> tagList = new List<TagEntity>();
            if (essayEntity.Content.Contains('#'))
            {
                var contentList = essayEntity.Content.Split('#');
                int contentListLastIndex = contentList.Length - 1;
                tagList = contentList.Where((item, index) => ((!string.IsNullOrEmpty(item)) && (index != contentListLastIndex))).Select((item, index) => { var tagEntity = new TagEntity { Sort = (index + 1), TagName = item };  tagEntity.AuditCreation(createUserId);return tagEntity; });
                essayEntity.Content = contentList.Last();
            }
            essayEntity.Location = "广州";
            essayEntity.AuditCreation(createUserId);
            mediaList.AuditCreation(createUserId);
            var result = _defaultRepository.AddEssay(essayEntity, tagList, mediaList);

            if (result)
            {
                string cacheKey = $"homeCover[{DateTime.Now.ToString("yyyyMMdd")}]";
                _memoryCache.Remove(cacheKey);
            }
            return new ResultDto { Result = result };
        }


        #endregion


        #region other
        /// <summary>
        /// 添加喜欢
        /// </summary>
        /// <param name="essayId"></param>
        /// <param name="isLike"></param>
        /// <returns></returns>
        [HttpPost("like")]
        public ResultDto Like(long essayId)
        {
            return _defaultRepository.ChangeEssayLike(_kardSession.UserId.Value, essayId);
        }

        /// <summary>
        /// 测试
        /// </summary>
        /// <param name="connNum"></param>
        /// <returns></returns>
        [HttpGet("test")]
        public async Task<ResultDto<string>> TestAsync(long? connNum = 10)
        {
            var milliseconds = await RunTask(connNum);
            _logger.LogDebug($"耗时：{milliseconds}ms");
            return new ResultDto<string>() { Result = true, Data = $"耗时：{milliseconds}ms" };
        }

        private async Task<long> RunTask(long? taskNum)
        {
            var taskList = new List<Task<long>>();
            for (int i = 0; i < taskNum; i++)
            {
                taskList.Add(Task.Run(() =>
                {
                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    for (int j = 0; j < taskNum; j++)
                    {
                        Like(taskNum.Value);
                    }
                    sw.Stop();
                    return sw.ElapsedMilliseconds;
                }));
            }

            var result = await Task.WhenAll(taskList);
            return (result.Sum() / (taskNum.Value * taskNum.Value));
        }
        #endregion

    }
}