using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace WeChatUserMath.Core
{
    public static class ImageExtensions
    {
        public static IEnumerable<Color> GetColors(this Bitmap bitmap)
        {

            var data = bitmap.LockBits(new Rectangle(Point.Empty, bitmap.Size),
                                                            ImageLockMode.ReadWrite,
                                                            bitmap.PixelFormat);
            var pixelSize = data.PixelFormat == PixelFormat.Format32bppArgb ? 4 : 3; // only works with 32 or 24 pixel-size bitmap!
            var padding = data.Stride - (data.Width * pixelSize);
            var bytes = new byte[data.Height * data.Stride];

            // copy the bytes from bitmap to array
            Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);

            var index = 0;
            var colorList = new List<Color>();

            for (var y = 0; y < data.Height; y++)
            {
                for (var x = 0; x < data.Width; x++)
                {
                    Color pixelColor = Color.FromArgb(
                        pixelSize == 3 ? 255 : bytes[index + 3], // A component if present
                        bytes[index + 2], // R component
                        bytes[index + 1], // G component
                        bytes[index]      // B component
                        );

                    colorList.Add(pixelColor);

                    index += pixelSize;
                }

                index += padding;
            }
            bitmap.UnlockBits(data);
            return colorList;
        }

        public static Bitmap GetThumbnail(this string Image)
        {
            try
            {
                List<byte> image = new List<byte>();
                byte[] _startToken = new byte[2] { 0xFF, 0xD8 }; //JPEG Start
                byte[] _endToken = new byte[2] { 0xFF, 0xD9 }; //JPEG End
                byte[] buff = new byte[2];
                FileStream fs = new FileStream(Image, FileMode.Open, FileAccess.Read);
                BinaryReader br = new BinaryReader(fs);
                while (br.BaseStream.Position < br.BaseStream.Length)
                {
                    byte bCurrent = br.ReadByte();
                    buff[0] = buff[1];
                    buff[1] = bCurrent;
                    if (Enumerable.SequenceEqual(buff, _startToken))
                    {
                        image.Clear();
                        image.AddRange(buff);
                    };
                    if (Enumerable.SequenceEqual(buff, _endToken))
                    {
                        break;
                    };
                    image.Add(bCurrent);
                }

                using (var ms = new MemoryStream(image.ToArray()))
                {
                    var imageStream = System.Drawing.Image.FromStream(ms);
                    return new Bitmap(imageStream);// (Bitmap)((new  ImageConverter()).ConvertFrom(image.ToArray()));
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
