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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
namespace Kard.Web.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [Authorize(AuthenticationSchemes = WeChatAppDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("user")]
    public class UserController : BaseController
    {
        private readonly IHostingEnvironment _env;
       
        private readonly IDefaultRepository _defaultRepository;
        public UserController(IHostingEnvironment env,
            ILogger<UserController> logger,
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
      

 


        #region user





        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("{userId}")]
        public ResultDto<KuserEntity> Index(long? userId)
        {
            var resultDto = new ResultDto<KuserEntity>() { Result = true, Data = GetUser(userId) };
            return resultDto;
        }

        /// <summary>
        /// 获取用户封面
        /// </summary>
        /// <returns></returns>
        [HttpGet("cover")]
        public ResultDto<KuserEntity> GetCover()
        {
            var resultDto = new ResultDto<KuserEntity>() { Result = true, Data = GetUser(_kardSession.UserId) };
            return resultDto;
        }


        private KuserEntity GetUser(long? userId)
        {
            string cacheKey = $"user[{userId}]";
            KuserEntity kuserEntity = _memoryCache.GetOrCreate(cacheKey, (cacheEntry) =>
            {
                cacheEntry.SetAbsoluteExpiration(DateTime.Now.Date.AddDays(60));
             
                return _defaultRepository.FirstOrDefault<KuserEntity,long>(_kardSession.UserId.Value);
            });
            return kuserEntity;
        }




        /// <summary>
        /// 获取单品图片
        /// </summary>
        /// <returns></returns>
        [HttpGet("pictures")]
        public ResultDto<IEnumerable<TopMediaDto>> GetPicture()
        {  
            return new ResultDto<IEnumerable<TopMediaDto>>() { Result = true, Data = _defaultRepository.GetUserMediaPictureList(_kardSession.UserId.Value,4) }; 
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