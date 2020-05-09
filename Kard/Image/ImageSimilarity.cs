using OCS = OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Drawing.Imaging;

namespace WeChatUserMath.Core
{
    public class ImageSimilarity
    {
        private static int _size = 16;
        public static void Test()
        {
            var stopwatch = new Stopwatch();

            #region 感知哈希算法
            stopwatch.Start();
            var percent = GetPercent();
            stopwatch.Stop();
            var milliseconds = stopwatch.Elapsed.Milliseconds;

            stopwatch.Restart();
            percent = GetPercent(0.5f);
            stopwatch.Stop();
            milliseconds = stopwatch.Elapsed.Milliseconds;
            #endregion

            #region 图像模板匹配
            //stopwatch.Restart();
            //MatMatch();
            //stopwatch.Stop();
            //milliseconds = stopwatch.Elapsed.Milliseconds;
            #endregion

            #region 灰度直方图
            //GetHisogram(null);
            #endregion

            //ImageHelper.CalculateImageSimilarity(@"images\0.png", @"images\1.png");
        }

        /// <summary>
        /// 图像模板匹配
        /// </summary>
        /// <returns></returns>
        public static void MatMatch()
        {
            //OCS.Extensions.BitmapConverter.ToMat(Resources.all);
            var template = @"C:\Users\Bit\Desktop\aaaaaaaaaaaaaaaaaa\6\template.jpg";
            var reference = @"C:\Users\Bit\Desktop\aaaaaaaaaaaaaaaaaa\6\reference.jpg";

            using (var referenceMat = new OCS.Mat(reference))
            using (var templateMat = new OCS.Mat(template))
            using (var res = new OCS.Mat(referenceMat.Rows - templateMat.Rows + 1, referenceMat.Cols - templateMat.Cols + 1, OCS.MatType.CV_32FC1))
            {
                //Convert input images to gray
                var gref = referenceMat.CvtColor(OCS.ColorConversionCodes.BGR2GRAY);
                var gtpl = templateMat.CvtColor(OCS.ColorConversionCodes.BGR2GRAY);

                OCS.Cv2.MatchTemplate(gref, gtpl, res, OCS.TemplateMatchModes.CCoeffNormed);
                OCS.Cv2.Threshold(res, res, 0.8, 1.0, OCS.ThresholdTypes.Tozero);

                while (true)
                {
                    double minval, maxval, threshold = 0.8;
                    OCS.Point minloc, maxloc;
                    OCS.Cv2.MinMaxLoc(res, out minval, out maxval, out minloc, out maxloc);

                    if (maxval >= threshold)
                    {
                        //Setup the rectangle to draw
                        var r = new OCS.Rect(new OCS.Point(maxloc.X, maxloc.Y), new OCS.Size(templateMat.Width, templateMat.Height));

                        //Draw a rectangle of the matching area
                        OCS.Cv2.Rectangle(referenceMat, r, OCS.Scalar.LimeGreen, 2);

                        //Fill in the res Mat so you don't find the same area again in the MinMaxLoc
                        OCS.Rect outRect;
                        OCS.Cv2.FloodFill(res, maxloc, new OCS.Scalar(0), out outRect, new OCS.Scalar(0.1), new OCS.Scalar(1.0));
                    }
                    else
                        break;
                }

                //OCS.Cv2.ImShow("Matches", referenceMat);
                //OCS.Cv2.WaitKey();
            }
        }

        public static double GetPercent(float? brightness = null)
        {
            var iHash1 = GetHash(new Bitmap(@"C:\Users\Bit\Desktop\aaaaaaaaaaaaaaaaaa\6\template2.jpg"), brightness);
            var iHash2 = GetHash(new Bitmap(@"C:\Users\Bit\Desktop\aaaaaaaaaaaaaaaaaa\6\reference2.jpg"), brightness);//微信图片_20200429185356.jpg

            //determine the number of equal pixel (x of 256)
            int equalElements = iHash1.Zip(iHash2, (i, j) => i == j).Count(eq => eq);
            var percent = Math.Round(equalElements / (_size * _size * 1d), 2);

            return percent;
        }


        /// <summary>
        /// 感知哈希算法
        /// </summary>
        /// <param name="bmpSource"></param>
        /// <returns></returns>
        public static List<bool> GetHash(Bitmap bmpSource, float? brightness = null)
        {

            var brightnessList = new List<float>();
            //create new image with 16x16 pixel
            var bmpMin = new Bitmap(bmpSource, new Size(_size, _size));
            for (int j = 0; j < bmpMin.Height; j++)
            {
                for (int i = 0; i < bmpMin.Width; i++)
                {
                    //reduce colors to true / false                
                    brightnessList.Add(bmpMin.GetPixel(i, j).GetBrightness());
                }
            }

            brightness = brightness ?? brightnessList.Average();
            return brightnessList.Select(item => item > brightness).ToList();
        }

        //public static double GetPercent2()
        //{

        //    List<bool> iHash1 = GetHash2(new Bitmap(@"C:\Users\Bit\Desktop\aaaaaaaaaaaaaaaaaa\6\微信图片_20200429185415.png"));
        //    List<bool> iHash2 = GetHash2(new Bitmap(@"C:\Users\Bit\Desktop\aaaaaaaaaaaaaaaaaa\6\190919031005_200x200.jpg"));//微信图片_20200429185356.jpg

        //    //determine the number of equal pixel (x of 256)
        //    int equalElements = iHash1.Zip(iHash2, (i, j) => i == j).Count(eq => eq);
        //    var percent = Math.Round(equalElements / (_size * _size * 1d), 2);

        //    return percent;
        //}

        //public static List<bool> GetHash2(Bitmap bmpSource)
        //{
        //    //create new image with 16x16 pixel
        //    var bmpMin = new Bitmap(bmpSource, new Size(_size, _size));
        //    var boolList = bmpMin.GetColors().Select(color => color.GetBrightness() < 0.5f);
        //    //var bits = new BitArray(bytes.ToArray());
        //    //bits.a


        //    return boolList.ToList();
        //}

        /// <summary>
        /// 灰度直方图
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        //public int[] GetHisogram(Bitmap img)
        //{

        //    BitmapData data = img.LockBits(new System.Drawing.Rectangle(0, 0, img.Width, img.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
        //    int[] histogram = new int[256];

        //    unsafe
        //    {

        //        byte* ptr = (byte*)data.Scan0;
        //        int remain = data.Stride - data.Width * 3;
        //        for (int i = 0; i < histogram.Length; i++)
        //            histogram[i] = 0;
        //        for (int i = 0; i < data.Height; i++)
        //        {
        //            for (int j = 0; j < data.Width; j++)
        //            {

        //                int mean = ptr[0] + ptr[1] + ptr[2];
        //                mean /= 3;
        //                histogram[mean]++;
        //                ptr += 3;
        //            }
        //            ptr += remain;
        //        }

        //    }

        //    img.UnlockBits(data);
        //    return histogram;
        //}

        //private float GetAbs(int firstNum, int secondNum)
        //{
        //    float abs = Math.Abs((float)firstNum - (float)secondNum);
        //    float result = Math.Max(firstNum, secondNum);
        //    if (result == 0)
        //        result = 1;
        //    return abs / result;
        //}



        ////最终计算结果

        //public float GetResult(int[] firstNum, int[] scondNum)
        //{
        //    if (firstNum.Length != scondNum.Length)
        //    {
        //        return 0;
        //    }
        //    else
        //    {
        //        float result = 0;
        //        int j = firstNum.Length;
        //        for (int i = 0; i < j; i++)
        //        {
        //            result += 1 - GetAbs(firstNum[i], scondNum[i]);
        //            Console.WriteLine(i + "----" + result);
        //        }
        //        return result / j;
        //    }

    }
}

