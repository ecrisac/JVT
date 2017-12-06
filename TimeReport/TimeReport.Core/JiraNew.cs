using System;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;

namespace TimeReport.Core
{
    public class JiraNew
    {
        /// <summary>
        /// The _password.
        /// </summary>
        private readonly string _password;

        /// <summary>
        /// The _username.
        /// </summary>
        private readonly string _username;

        private readonly string _url;

        private List<string> _keyPrefixes;

        public string AuthorName { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Jira"/> class.
        /// </summary>
        /// <param name="setting"></param>
        public JiraNew(string username, string password)
        {
            this._username = username;
            this._password = password;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Jira"/> class.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="password">jira password</param>
        /// <param name="login">Jira login name</param>
        /// <param name="authorName">Jira author name</param>
        public JiraNew(string url, string password, string login, string authorName)
        {
           _username = login;
           _password = password;
           _url = url;
           AuthorName = authorName;
        }

        /// <summary>
        /// The get work log.
        /// </summary>
        /// <param name="issueKey">
        /// The issue key.
        /// </param>
        /// <returns>
        /// The <see cref="JiraWorkLog"/>.
        /// </returns>
        public JiraWorkLog GetWorkLog(string issueKey)
        {
            //"https://teamvelocitymarketing.atlassian.net/rest/api/2/issue/{0}/worklog"
            return Requests.GetJson<JiraWorkLog>(string.Format(_url, issueKey),
                    credentials: new NetworkCredential(this._username, this._password));
        }
        public Issue GetIssue(string issueKey)
        {
            return Requests.GetJson<Issue>(string.Format("https://teamvelocitymarketing.atlassian.net/rest/api/2/issue/{0}", issueKey),
                    credentials: new NetworkCredential(_username, _password));
        }
        public RootObject GetIssues(List<string> issueKey)
        {
            var queryString = "key=";
            queryString += string.Join("+OR+key=", issueKey);

            var result = Requests.GetJson<RootObject>(string.Format("https://teamvelocitymarketing.atlassian.net/rest/api/2/search?jql={0}&fields=id,key,summary,worklog&maxResults=200", queryString),
                    credentials: new NetworkCredential(_username, _password));
            return result;
        }

        #region internal classes

        public class RootObject
        {
            public string Expand { get; set; }
            public int StartAt { get; set; }
            public int MaxResults { get; set; }
            public int Total { get; set; }
            public List<Issue> Issues { get; set; }
        }

        public class Issue
        {
            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("key")]
            public string Key { get; set; }

            [JsonProperty("fields")]
            public IssueFields Fields { get; set; }

            [JsonIgnore]
            public string Url { get { return string.Format("https://teamvelocitymarketing.atlassian.net/browse/{0}", Key); } }
        }
        public class IssueFields
        {
            [JsonProperty("summary")]
            public string Summary { get; set; }

            [JsonProperty("worklog")]
            public JiraNew.JiraWorkLog Worklog { get; set; }
        }
        /// <summary>
        /// The jira work log item author.
        /// </summary>
        public class JiraWorkLogItemAuthor
        {
            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            [JsonProperty("name")]
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether active.
            /// </summary>
            [JsonProperty("active")]
            public bool Active { get; set; }
        }

        /// <summary>
        /// The jira work log item.
        /// </summary>
        public class JiraWorkLogItem
        {
            /// <summary>
            /// Gets or sets the author.
            /// </summary>
            [JsonProperty("author")]
            public JiraWorkLogItemAuthor Author { get; set; }

            /// <summary>
            /// Gets or sets the time spend seconds.
            /// </summary>
            [JsonProperty("timeSpentSeconds")]
            public int TimeSpendSeconds { get; set; }

            /// <summary>
            /// Gets or sets the created.
            /// </summary>
            [JsonProperty("created")]
            public DateTime Created { get; set; }

            /// <summary>
            /// Gets or sets the started.
            /// </summary>
            [JsonProperty("started")]
            public DateTime? Started { get; set; }

            /// <summary>
            /// Gets or sets the updated.
            /// </summary>
            [JsonProperty("updated")]
            public DateTime? Updated { get; set; }

            /// <summary>
            /// Gets or sets the comment.
            /// </summary>
            [JsonProperty("comment")]
            public string Comment { get; set; }

            /// <summary>
            /// Gets a value indicating whether is current month.
            /// </summary>
            public bool IsCurrentMonth
            {
                get
                {
                    return this.Created.Month == DateTime.Now.Month && this.Created.Year == DateTime.Now.Year;
                }
            }
        }

        /// <summary>
        /// The jira work log.
        /// </summary>
        public class JiraWorkLog
        {
            /// <summary>
            /// Gets or sets the items.
            /// </summary>
            [JsonProperty("worklogs")]
            public IEnumerable<JiraWorkLogItem> Items { get; set; }
        }

        #endregion
    }
}
