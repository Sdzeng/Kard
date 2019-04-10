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

            var newImagePath = Path.Join(Root.Replace("\\", "/"), subpath).Replace("//", "/");

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
                int x = 0;
                int y = 0;
                int width = original.Width;
                int height = original.Height;

                if ((double)original.Width / (double)original.Height > (double)imageHandleDto.ImageWidth / (double)imageHandleDto.ImageHeight)
                {
                    height = original.Height;
                    width = original.Height * imageHandleDto.ImageWidth / imageHandleDto.ImageHeight;
                    y = 0;
                    x = (original.Width - width) / 2;
                }
                else
                {
                    width = original.Width;
                    height = original.Width * imageHandleDto.ImageHeight / imageHandleDto.ImageWidth;
                    x = 0;
                    y = (original.Height - height) / 2;
                }

                int left=x, top=y+height, right=x+width, bottom = y;

                using (var crop = original.Copy(left, top, right, bottom))
                {
                    using (var resized = crop.GetScaledInstance(imageHandleDto.ImageWidth, imageHandleDto.ImageHeight, FREE_IMAGE_FILTER.FILTER_BICUBIC))
                    {
                        //, FREE_IMAGE_FORMAT.FIF_JPEG, FREE_IMAGE_SAVE_FLAGS.JPEG_QUALITYGOOD | FREE_IMAGE_SAVE_FLAGS.JPEG_BASELINE
                        resized.Save(newImagePath, imageFormat, FREE_IMAGE_SAVE_FLAGS.DEFAULT);
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
