using Kard.Core.Dtos;
using Kard.Core.Entities;
using Kard.Core.IRepositories;
using Kard.Runtime.Security.Authentication.WeChat;
using Kard.Runtime.Session;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;

namespace Kard.Web.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [Authorize(AuthenticationSchemes = WeChatAppDefaults.AuthenticationScheme)]
    [Route("api/user")]
    public class WebController : BaseController
    {
        private readonly IHostingEnvironment _env;
       
        private readonly IDefaultRepository _defaultRepository;
        public WebController(IHostingEnvironment env,
            ILogger<WebController> logger,
            IMemoryCache memoryCache,
            IDefaultRepository defaultRepository,
            IKardSession kardSession)
            : base(logger, memoryCache, kardSession)
        {
            //HttpContext.Session.SetString("UserId", user.Id.ToString());
            _env = env;
            _defaultRepository = defaultRepository;
        }

        //[Authorize(Roles = "member", AuthenticationSchemes= "members")]
        [HttpGet("test")]
        public IActionResult Test(long? userId)
        {
            return Content("已登陆");
        }


        #region user

 
 

 
        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("{userId}")]
        public IActionResult Index(long? userId)
        {
            return Json(GetUser(userId));
        }

        /// <summary>
        /// 获取用户封面
        /// </summary>
        /// <returns></returns>
        [HttpGet("cover")]
        public IActionResult GetCover()
        {
            return Json(GetUser(_kardSession.UserId));
        }


        private KuserEntity GetUser(long? userId)
        {
            string cacheKey = $"user[{userId}]";
            KuserEntity kuserEntity = _memoryCache.GetOrCreate(cacheKey, (cacheEntry) =>
            {
                cacheEntry.SetAbsoluteExpiration(DateTime.Now.Date.AddDays(60));
                return _defaultRepository.GetUser(_kardSession.UserId.Value);
            });
            return kuserEntity;
        }




        /// <summary>
        /// 获取单品图片
        /// </summary>
        /// <returns></returns>
        [HttpGet("pictures")]
        public IEnumerable<TopMediaDto> GetPicture()
        {
            return _defaultRepository.GetUserMediaPicture(4);
        }



        /// <summary>
        /// 上次文件
        /// </summary>
        /// <returns></returns>

        [HttpPost("uploadMedia")]
        //[Consumes("multipart/form-data")]
        //[RequestSizeLimit(100_000_000)]
        public IActionResult UploadMedia(IFormFile mediaFlie)
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
                string fileExtension = Path.GetExtension(ContentDispositionHeaderValue.Parse(mediaFlie.ContentDisposition).FileName.Trim('"'));
                string fullPath = Path.Combine(newPath, fileName + fileExtension);
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    mediaFlie.CopyTo(stream);
                }
                result.Result = true;
                result.Data = new { FileUrl = Path.Combine(newFolder, fileName), FileExtension = fileExtension };
                return Json(result);
            }

            result.Result = false;
            result.Message = "上传失败";
            return Json(result);
        }


        /// <summary>
        /// 添加纪录
        /// </summary>
        /// <param name="essayEntity"></param>
        /// <param name="mediaList"></param>
        [HttpPost("addessay")]
        public IActionResult AddEssay(EssayEntity essayEntity, IEnumerable<MediaEntity> mediaList)
        {
            var result = _defaultRepository.TransExecute((conn, trans) =>
            {
                var resultDto = _defaultRepository.CreateAndGetId<EssayEntity, long>(essayEntity, conn, trans);

                if (resultDto.Result)
                {
                    mediaList = mediaList.Select(meida =>
                    {
                        meida.EssayId = resultDto.Data;
                        meida.MediaExtension = meida.MediaExtension.Replace(".", "");
                        return meida;
                    });
                    return _defaultRepository.Create(mediaList, conn, trans);
                }

                return false;
            });


            return Json(new ResultDto { Result = result });
        }

        /// <summary>
        /// 上传图片
        /// </summary>
        /// <returns></returns>
        //[HttpPost]
        //[Route("api/common/uploadimg")]
        //public async Task<IActionResult> UploadImg(IFormFile formFile)
        //{
        //    var json = new JsonResultModel<object>()
        //    {
        //        code = "200",
        //        message = "上传成功"
        //    };
        //    if (formFile == null) formFile = Request.Form.Files[0];

        //    if (formFile == null || formFile.Length <= 0)
        //    {
        //        json.code = "400";
        //        json.message = "上传失败，文件不存在";
        //        return await Task.FromResult(json.ToJsonResult());
        //    };

        //    if (!formFile.ContentType.Contains("image"))
        //    {
        //        json.code = "400";
        //        json.message = "上传失败，这不是图片文件";
        //        return await Task.FromResult(json.ToJsonResult());
        //    }

        //    if (formFile.Length > 3 * 1024 * 1024)
        //    {
        //        json.code = "400";
        //        json.message = "上传失败，文件不能大于3MB";
        //        return await Task.FromResult(json.ToJsonResult());
        //    }


        //    var dateStr = DateTime.Now.ToString("yyyyMMdd");
        //    var filePath = $"{_appConfig.ApplicationConfig.UploadConfig.ImageUploadUrl}/{dateStr}/";
        //    if (!Directory.Exists(filePath))
        //    {
        //        Directory.CreateDirectory(filePath);//不存在就创建目录 
        //    }

        //    var fileName = $"{Utils.GetRamCode()}{Path.GetExtension(formFile.FileName)}";

        //    using (var fs = System.IO.File.Create(string.Concat(filePath, fileName)))
        //    {
        //        await formFile.CopyToAsync(fs);
        //        await fs.FlushAsync();
        //        json.data = $"{_appConfig.ApplicationConfig.UploadConfig.ImageDownloadUrl}/{dateStr}/{fileName}";
        //    }

        //    return await Task.FromResult(json.ToJsonResult());
        //}





        /// <summary>
        /// 未登录返回结果
        /// </summary>
        /// <returns></returns>

        [HttpGet("notlogin")]
        [AllowAnonymous]
        public IActionResult NotLogin()
        {

            var rs = new JsonResult(new
            {
                message = "您未登录不能查看该内容"
            });

            rs.StatusCode = 401;
            return rs;
        }
        #endregion
    }
}