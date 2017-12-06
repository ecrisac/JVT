using System;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TimeReport
{
    internal class SyncManager
    {
        public static void Sync(double timeSpentSeconds, string plat, JiraItem jira, string workLogMessage,DateTime when)
        {
            var test = new { timeSpentSeconds, started = when, comment = workLogMessage };
            var payload = JsonConvert.SerializeObject(test, new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-ddThh:mm:ss.fffzz00" });//<-- fixing issue with time zone 
            RunQuery(data: payload, argument: plat, jira: jira);
        }

        protected static string RunQuery(JiraItem jira, string argument = null, string data = null, string method = "POST")
        {
            // Where;
            // resource = issue
            // argument = "JIRA-16"
            var url = jira.Url;

            if (argument != null)
            {
                url = string.Format(url, argument);
            }

            // URLFormat = https://teamvelocitymarketing.atlassian.net/rest/api/2/issue/PLAT-1767/worklog

            var request = WebRequest.Create(url) as HttpWebRequest;
            request.ContentType = "application/json";
            request.Method = method;
            request.ContentLength = data.Length;
            var base64Credentials = GetEncodedCredentials(jira.UserName, jira.Password); // check below
            request.Headers.Add("Authorization", "Basic " + base64Credentials);

            using (var writer = new StreamWriter(request.GetRequestStream()))
            {
                writer.Write(data);
            }

            var response = request.GetResponse() as HttpWebResponse; // here returns the error
            //The remote server returned an error: (401) Unauthorized.

            string result;
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                result = reader.ReadToEnd();
            }

            return result;
        }

        private static string GetEncodedCredentials(string user, string password)
        {
            var mergedCredentials = string.Format("{0}:{1}", user, password);
            var byteCredentials = Encoding.UTF8.GetBytes(mergedCredentials);
            return Convert.ToBase64String(byteCredentials);
        }
    }
}