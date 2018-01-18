using System;
using System.Collections.Generic;
using System.Text;

namespace Kard.Core.Dtos
{
    public class ImageHandleDto
    {
        public string ImagePath { get; set; }

        public string ImageName { get; set; }

        public int ImageWidth { get; set; }

        public int ImageHeight { get; set; }


        public string ImageExtensions { get; set; }
    }
}
