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
using Kard.Json;
using Kard.Runtime.Security;
using Kard.Runtime.Security.Authentication.WeChat;
using Kard.Runtime.Session;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Senparc.Weixin.MP.Containers;
using Senparc.Weixin.MP.Helpers;

namespace Kard.Web.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [Authorize(AuthenticationSchemes = WeChatAppDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("common")]
    public class CommonController : BaseController
    {
        private readonly IHostingEnvironment _env;
        private readonly IDefaultRepository _defaultRepository;
        private readonly IConfiguration _configuration;

        public CommonController(IHostingEnvironment env,
            ILogger<EssayController> logger,
            IMemoryCache memoryCache,
            IDefaultRepository defaultRepository,
            IKardSession kardSession,
            IConfiguration configuration) : base(logger, memoryCache, kardSession)
        {
            _env = env;
            _defaultRepository = defaultRepository;
            _configuration = configuration;
        }


        /// 上传文件
        /// </summary>
        /// <returns></returns>
        [HttpPost("uploadfile")]
        //[Consumes("multipart/form-data")]
        //[RequestSizeLimit(100_000_000)]
        public ResultDto UploadFile(IFormFile flie)
        {
            var result = new ResultDto();
            if (flie == null && Request.Form.Files.Any())
            {
                flie = Request.Form.Files[0];
            }
            if (flie == null)
            {
                result.Result = false;
                result.Message = "未选择文件";
                return result;
            }

            var now = DateTime.Now;
            string webRootPath = _env.WebRootPath;

            string newFolder = Path.Combine("user", _kardSession.UserId.ToString(), "media", now.ToString("yyyyMMdd"));
            string newPath = Path.Combine(webRootPath, newFolder);
            if (!Directory.Exists(newPath))
            {
                Directory.CreateDirectory(newPath);
            }

            if (flie.Length > 0)
            {
                string fileName = now.ToString("ddHHmmssffff");
                string fileExtension = Path.GetExtension(flie.FileName.Trim('"')).ToLower(); // Path.GetExtension(ContentDispositionHeaderValue.Parse(mediaFlie.ContentDisposition).FileName.Trim('"'));
                if (string.IsNullOrEmpty(fileExtension)) {
                    switch (flie.ContentType.ToLower()) {
                        case "image/png":fileExtension = ".png";break;
                        case "image/jpg":
                        case "image/jpeg": fileExtension = ".jpg"; break;
                    }
                }

                string fullPath = Path.Combine(newPath, fileName + fileExtension);
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    flie.CopyTo(stream);
                }
                result.Result = true;
                //var host = HttpContext.Request.Host;
                //Url = Path.Combine(_configuration.GetValue<string>("AppSetting:CDN:Url") , newFolder, fileName + fileExtension).Replace("\\", "/") ,
                result.Data = new { FileUrl = Path.Combine(newFolder, fileName).Replace("\\", "/"), FileExtension = fileExtension.Replace(".", "") };
                return result;
            }

            result.Result = false;
            result.Data = new  {Url = "/" };
           result.Message = "上传失败";
            return result;
        }

        ///// <summary>
        ///// ckeditor上传文件
        ///// </summary>
        ///// <returns></returns>
        //[HttpPost("editoruploadfile")]
        ////[Consumes("multipart/form-data")]
        ////[RequestSizeLimit(100_000_000)]
        //public object EditorFileUpload(IFormFile editorFlie)
        //{
        //    var result = new ResultDto();
        //    if (editorFlie == null && Request.Form.Files.Any())
        //    {
        //        editorFlie = Request.Form.Files[0];
        //    }
        //    if (editorFlie == null)
        //    {
        //        result.Result = true;
        //        result.Message = "未选择文件";
        //        return result;
        //    }

        //    var now = DateTime.Now;
        //    string webRootPath = _env.WebRootPath;

        //    string newFolder = Path.Combine("user", _kardSession.UserId.ToString(), "media", now.ToString("yyyyMMdd"));
        //    string newPath = Path.Combine(webRootPath, newFolder);
        //    if (!Directory.Exists(newPath))
        //    {
        //        Directory.CreateDirectory(newPath);
        //    }

        //    if (editorFlie.Length > 0)
        //    {
        //        string fileName = now.ToString("ddHHmmssffff");
        //        string fileExtension = Path.GetExtension(editorFlie.FileName.Trim('"')).ToLower(); // Path.GetExtension(ContentDispositionHeaderValue.Parse(mediaFlie.ContentDisposition).FileName.Trim('"'));
        //        string fullPath = Path.Combine(newPath, fileName + fileExtension);
        //        using (var stream = new FileStream(fullPath, FileMode.Create))
        //        {
        //            editorFlie.CopyTo(stream);
        //        }

             
        //        return new {Uploaded=1,FileName= fileName + fileExtension, Url = Path.Combine(_defaultRepository.Configuration.GetValue<string>("AppSetting:ApiDomain"), newFolder, fileName + fileExtension).Replace("\\", "/") };
        //    }


           
        //    return new {  Uploaded = 1, FileName = "", Url = "/",Error=new { Message=""} };
        //}
    }

}