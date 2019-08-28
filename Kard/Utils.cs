﻿using Kard.Dtos;
using Kard.Extensions;
using Kard.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace Kard
{
    public static class Utils
    {
        /// <summary>
        /// 验证IP地址格式
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        public static bool ValidateIPAddress(string ipAddress)
        {
            Regex validipregex = new Regex(@"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$");
            return (ipAddress != "" && validipregex.IsMatch(ipAddress.Trim())) ? true : false;
        }

        /// <summary>
        /// HTTP GET方式请求数据.
        /// </summary>
        /// <param name="url">URL.</param>
        /// <returns></returns>
        public static string HttpGet(string url)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = "GET";
            //request.ContentType = "application/x-www-form-urlencoded";
            request.Accept = "*/*";
            request.Timeout = 15000;
            request.AllowAutoRedirect = false;

            WebResponse response = null;
            string responseStr = null;

            try
            {
                response = request.GetResponse();

                if (response != null)
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                    responseStr = reader.ReadToEnd();
                    reader.Close();
                }
            }
            catch (Exception)
            {
                //throw;
            }
            finally
            {
                request = null;
                response = null;
            }

            return responseStr;
        }



        /// <summary>
        /// HTTP POST方式请求数据
        /// </summary>
        /// <param name="url">URL.</param>
        /// <param name="param">POST的数据</param>
        /// <returns></returns>
        public static string HttpPost(string url, string param,string contentType= "application/json")
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = "POST";
            //request.ContentType = "application/x-www-form-urlencoded";
            request.ContentType = contentType;
            request.Accept = "*/*";
            request.Timeout = 15000;
            request.AllowAutoRedirect = false;

            StreamWriter requestStream = null;
            WebResponse response = null;
            string responseStr = null;

            try
            {
                requestStream = new StreamWriter(request.GetRequestStream());
                requestStream.Write(param);
                requestStream.Close();

                response = request.GetResponse();
                if (response != null)
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                    responseStr = reader.ReadToEnd();
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                responseStr = ex.Message.ToString();
                throw;
            }
            finally
            {
                request = null;
                requestStream = null;
                response = null;
            }

            return responseStr;
        }

        #region Unicode转字符串
        public static string Unicode2String(string source)
        {
            return new Regex(@"\\u([0-9A-F]{4})", RegexOptions.IgnoreCase | RegexOptions.Compiled).Replace(
                         source, x => string.Empty + Convert.ToChar(Convert.ToUInt16(x.Result("$1"), 16)));
        }

        #endregion


        public static string GetCity(HttpContext context, IMemoryCache memoryCache)
        {
            var ip = context.GetClientIpAddress();
            string city = "技术星球";
            if (Utils.ValidateIPAddress(ip))
            {
                string cacheKey = $"IPCity[{ip}]";
                city = memoryCache.GetOrCreate(cacheKey, (cacheEntry) =>
                {

                    #region 通过IP获取详细地址（淘宝接口）
                    string info = Utils.HttpGet("http://ip.taobao.com/service/getIpInfo.php?ip=" + ip);
                    string re = Utils.Unicode2String(info);
                    var taobao = Serialize.FromJson<ResTaobaoIpDto>(re);
                    if (taobao.code == 0)
                    {
                        return taobao.data.city;
                    }

                    return "技术星球";
                    #endregion

                });
            }

            return city;
        }

        public static Regex ContentRegex { get; } = new Regex("\\s|<style>[.\n\r]*?</style>|<xml>[.\n\r]*?</xml>|</?[^>]*>|\"? ?/>|/[a-zA-Z]+>|&nbsp;|[ \n\r\t]", RegexOptions.IgnoreCase | RegexOptions.Multiline);
    }
}