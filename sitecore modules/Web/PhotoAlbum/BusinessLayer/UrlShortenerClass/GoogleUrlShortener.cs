using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Sitecore.Diagnostics;

namespace Sitecore.Modules.PhotoAlbum.BusinessLayer.UrlShortenerClass
{
    public class GoogleUrlShortener : IUrlShortener
    {
        public Uri ShortenUrl(Uri url, string key)
        {
            string text = "{\"longUrl\": \"" + url + "\"}";
            Uri result = url;
            Uri requestUri = new Uri("https://www.googleapis.com/urlshortener/v1/url?key=" + key);
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(requestUri);
            try
            {
                httpWebRequest.ServicePoint.Expect100Continue = false;
                httpWebRequest.Method = "POST";
                httpWebRequest.ContentLength = (long)text.Length;
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Headers.Add("Cache-Control", "no-cache");
                using (Stream requestStream = httpWebRequest.GetRequestStream())
                {
                    byte[] bytes = Encoding.ASCII.GetBytes(text);
                    requestStream.Write(bytes, 0, bytes.Length);
                }
                using (HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
                {
                    using (Stream responseStream = httpWebResponse.GetResponseStream())
                    {
                        if (responseStream != null)
                        {
                            using (StreamReader streamReader = new StreamReader(responseStream))
                            {
                                string input = streamReader.ReadToEnd();
                                string value = Regex.Match(input, "\"id\": ?\"(?<id>.+)\"").Groups["id"].Value;
                                result = new Uri(value);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Warn(string.Format(CultureInfo.InvariantCulture, "GoogleUrlShortenerException: {0}", new object[]
				{
					ex.Message
				}), this);
            }
            return result;
        }
        public Uri ShortenUrl(Uri url)
        {
            string key = "AIzaSyC70uBdWi8DijdNJbnnR8w_FHM8iWv-Sho";
            return this.ShortenUrl(url, key);
        }
    }
}