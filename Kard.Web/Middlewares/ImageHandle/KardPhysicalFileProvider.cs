using FreeImageAPI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;

namespace Kard.Web.Middlewares.ImageHandle
{
    public class KardPhysicalFileProvider : PhysicalFileProvider, IFileProvider
    {
        private readonly ILogger _logger;
        private readonly Stopwatch _stopwatch;
        private readonly ImageHandleOptions _options;

        public KardPhysicalFileProvider(string root, ImageHandleOptions options, ILoggerFactory loggerFactory) : base(root)
        {
            _logger = loggerFactory.CreateLogger<KardPhysicalFileProvider>();
            _stopwatch = new Stopwatch();
            _options = options;
        }

        public new IFileInfo GetFileInfo(string subpath)
        {
            var fileInfo = base.GetFileInfo(subpath);
            if (fileInfo.Exists)
            {
                return fileInfo;
            }


            var matchResult = _options.IsMatch(subpath, _options.PathMatch);
            if (!matchResult.Result)
            {
                return fileInfo;
            }

            var imageHandleDto = matchResult.Data;

            //生成文件
            fileInfo = base.GetFileInfo($"{imageHandleDto.ImagePath}{imageHandleDto.ImageName}.{imageHandleDto.ImageExtensions}");
            if (!fileInfo.Exists)
            {
                return fileInfo;
            }

            var newImagePath =  Path.Join(Root.Replace("\\", "/"), subpath).Replace("//","/");

            //_stopwatch.Start();

            //var settings = new ProcessImageSettings()
            //{
            //    Width = imageHandleDto.ImageWidth,
            //    Height = imageHandleDto.ImageHeight,
            //    ResizeMode = CropScaleMode.Max,
            //    SaveFormat = FileFormat.Jpeg,
            //    JpegQuality = 75,
            //    JpegSubsampleMode = ChromaSubsampleMode.Subsample420
            //};


            //using (var output = new FileStream(newImagePath, FileMode.Create))
            //{
            //    MagicImageProcessor.ProcessImage(fileInfo.PhysicalPath, output, settings);
            //}

            //_stopwatch.Stop();
            //_logger.LogDebug($"Magick.NET耗时{_stopwatch.ElapsedMilliseconds}ms");

            //File.Delete(newImagePath);


            //_stopwatch.Restart();
            //using (var original = FreeImageBitmap.FromFile(fileInfo.PhysicalPath))
            //{
            //    using (var resized = new FreeImageBitmap(original, imageHandleDto.ImageWidth, imageHandleDto.ImageHeight))
            //    {

            //        resized.Save(newImagePath, FREE_IMAGE_FORMAT.FIF_JPEG,
            //        FREE_IMAGE_SAVE_FLAGS.JPEG_QUALITYGOOD |
            //        FREE_IMAGE_SAVE_FLAGS.JPEG_BASELINE);
            //    }
            //}

            //_stopwatch.Stop();
            //_logger.LogDebug($"FreeImage FromFilePath耗时{_stopwatch.ElapsedMilliseconds}ms");

            //File.Delete(newImagePath);

            //_stopwatch.Restart();
            //using (var original = FreeImageBitmap.FromStream(fileInfo.CreateReadStream()))
            //{
            //    using (var resized = new FreeImageBitmap(original, imageHandleDto.ImageWidth, imageHandleDto.ImageHeight))
            //    {

            //        resized.Save(newImagePath, FREE_IMAGE_FORMAT.FIF_JPEG,
            //        FREE_IMAGE_SAVE_FLAGS.JPEG_QUALITYGOOD |
            //        FREE_IMAGE_SAVE_FLAGS.JPEG_BASELINE);
            //    }
            //}

            //_stopwatch.Stop();
            //_logger.LogDebug($"FreeImage FromFileStream耗时{_stopwatch.ElapsedMilliseconds}ms");


            _stopwatch.Start();


            FREE_IMAGE_FORMAT imageFormat = 0;
            switch (imageHandleDto.ImageExtensions.ToLower())
            {
                case "jpg":
                case "jpeg": imageFormat = FREE_IMAGE_FORMAT.FIF_JPEG; break;
                case "png": imageFormat = FREE_IMAGE_FORMAT.FIF_PNG; break;
                case "gif": imageFormat = FREE_IMAGE_FORMAT.FIF_GIF; break;
                case "ico": imageFormat = FREE_IMAGE_FORMAT.FIF_ICO; break;
                default: imageFormat = FREE_IMAGE_FORMAT.FIF_UNKNOWN; break;
            }



            using (var original = FreeImageBitmap.FromFile(fileInfo.PhysicalPath))
            {

                var size = original.Width / (double)imageHandleDto.ImageWidth;
                if ((imageHandleDto.ImageHeight * size) > original.Height)
                {
                    size = original.Height / (double)imageHandleDto.ImageHeight;
                }

                var width = original.Width / size;
                var height = original.Height / size;
                width = width > original.Width ? original.Width : width;
                height = height > original.Height ? original.Height : height;

                using (var resized = original.GetScaledInstance((int)width, (int)height, FREE_IMAGE_FILTER.FILTER_BICUBIC))
                {
                    double left, top, right, bottom;


                    var halfWidth = Math.Floor(Convert.ToDouble(imageHandleDto.ImageWidth / 2));
                    var halfHeight = Math.Floor(Convert.ToDouble(imageHandleDto.ImageHeight / 2));
                    var centerX = Math.Round(width / 2);
                    var centerY = Math.Round(height / 2);

                    if (resized.Width > imageHandleDto.ImageWidth)
                    {
                        left = centerX - halfWidth;
                        right = centerX + halfWidth;
                    }
                    else {
                        left = 0;
                        right = resized.Width;
                    }

                    if (resized.Height > imageHandleDto.ImageHeight)
                    {
                        bottom = centerY - halfHeight;
                        top = centerY + halfHeight;
                    }
                    else {
                        bottom = 0;
                        top = resized.Height;
                    }
                 
                    using (var crop = resized.Copy((int)left, (int)top, (int)right, (int)bottom))
                    {
                        //, FREE_IMAGE_FORMAT.FIF_JPEG, FREE_IMAGE_SAVE_FLAGS.JPEG_QUALITYGOOD | FREE_IMAGE_SAVE_FLAGS.JPEG_BASELINE
                        crop.Save(newImagePath, imageFormat, FREE_IMAGE_SAVE_FLAGS.DEFAULT);
                    }
                }
            }

            _stopwatch.Stop();
            _logger.LogDebug($"FreeImage FromFilePath耗时{_stopwatch.ElapsedMilliseconds}ms");



            fileInfo = base.GetFileInfo(subpath);
            return fileInfo;
        }



    }
}
