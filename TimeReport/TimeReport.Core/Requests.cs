using System;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;

using Newtonsoft.Json;

namespace TimeReport.Core
{
    public static class Requests
    {
        public static T RequestJson<T>(string method, string url, string content = null, string contentType = "application/x-www-form-urlencoded", NetworkCredential credentials = null)
        {
            var request = WebRequest.Create(url);

            request.Method = method;

            if(credentials != null)
            {
                request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(credentials.UserName + ":" + credentials.Password));
            }

            request.Proxy = WebRequest.GetSystemWebProxy();

            if(!string.IsNullOrEmpty(content))
            {
                var contentBytes = Encoding.UTF8.GetBytes(content);

                request.ContentLength = contentBytes.Length;
                request.ContentType = contentType;

                using(var stream = request.GetRequestStream())
                {
                    stream.Write(contentBytes, 0, contentBytes.Length);
                }
            }
            else
            {
                request.ContentLength = 0;
            }

            try
            {
                using(var response = request.GetResponse())
                {
                    using(var responseStream = response.GetResponseStream())
                    {
                        if(responseStream == null)
                        {
                            throw new NullReferenceException("responseStream");
                        }

                        using(var responseStreamReader = new StreamReader(responseStream))
                        {
                            var respString = responseStreamReader.ReadToEnd();
                            return JsonConvert.DeserializeObject<T>(respString);
                        }
                    }
                }
            }
            catch(WebException ex)
            {
                if(ex.Response != null)
                {
                    using(var errorResponse = (HttpWebResponse)ex.Response)
                    {
                        using(var responseStream = errorResponse.GetResponseStream())
                        {
                            if(responseStream != null)
                            {
                                using(var reader = new StreamReader(responseStream))
                                {
                                    Console.WriteLine("Url: {0}", url);
                                    Console.WriteLine("Status Code: {0} {1}", (int)errorResponse.StatusCode, errorResponse.StatusCode);
                                    Console.WriteLine("Response: {0}", reader.ReadToEnd());
                                }
                            }
                        }
                    }
                }

                throw;
            }
        }

        public static T GetJson<T>(string url, object queryArgs = null, NetworkCredential credentials = null)
        {
            if(queryArgs != null)
            {
                var queryArgsType = queryArgs.GetType();
                url += "?" + string.Join("&", queryArgsType.GetProperties().Select(x => x.Name + "=" + x.GetValue(queryArgs, null)));
            }

            System.Diagnostics.Trace.WriteLine(url, "JTT");

            return RequestJson<T>("GET", url, credentials:credentials);
        }
    }
}
