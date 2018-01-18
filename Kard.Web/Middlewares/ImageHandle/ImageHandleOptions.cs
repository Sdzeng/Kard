using Kard.Core.Dtos;
using Kard.Extensions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Kard.Web.Middlewares.ImageHandle
{
    public class ImageHandleOptions
    {
        public ImageHandleOptions()
        {
            //OnPrepareResponse = _ => { };
        }

        public string PathMatch { get; set; }= @"(?<path>(.+(/|\\))+)(?<imagename>(.+))_(?<width>(\d+))x(?<height>(\d+))\.(?<extensions>(jpeg|gif|jpg|png|ico))";

        public Func<string, string, ResultDto<ImageHandleDto>> IsMatch { get; set; } =( input, pathMatch) =>
        {
            var resultDto = new ResultDto<ImageHandleDto> ();

            if (input.IsNullOrEmpty() || pathMatch.IsNullOrEmpty())
            {
                return resultDto.Set(false, "input或pathMatch为空");
            }

            var match = Regex.Match(input, pathMatch);
            if (!match.Success)
            {
                return resultDto.Set(false, $"input:{input}和pathMatch:{pathMatch}匹配失败");
            }


            var imageHandleDto = new ImageHandleDto();
            imageHandleDto.ImagePath = match.Groups["path"].Value;
            imageHandleDto.ImageName = match.Groups["imagename"].Value;
            imageHandleDto.ImageWidth = Convert.ToInt32(match.Groups["width"].Value);
            imageHandleDto.ImageHeight = Convert.ToInt32(match.Groups["height"].Value);
            imageHandleDto.ImageExtensions = match.Groups["extensions"].Value;
            
            return resultDto.Set(true, "匹配成功", imageHandleDto);
        };

    }
}
