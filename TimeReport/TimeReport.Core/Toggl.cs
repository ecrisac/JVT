using System;
using System.Collections.Generic;
using System.Net;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TimeReport.Core.Configuration;

namespace TimeReport.Core
{
    public class Toggl
    {
        /// <summary>
        /// The _password.
        /// </summary>
        private readonly string _password;

        /// <summary>
        /// The _username.
        /// </summary>
        private readonly string _username;

        /// <summary>
        /// The _api token.
        /// </summary>
        private string _apiToken;

        /// <summary>
        /// The _user id.
        /// </summary>
        private string _userId;

        private string _url;

        /// <summary>
        /// Initializes a new instance of the <see cref="Toggl"/> class.
        /// </summary>
        /// <param name="username">
        /// The username.
        /// </param>
        /// <param name="password">
        /// The password.
        /// </param>
        public Toggl(string username, string password)
        {
            this._username = username;
            this._password = password;
        }

        public Toggl(ToggleConfigurationSection setting)
        {
            this._username = setting.Login;
            this._password = setting.Password;
            this._url = setting.Url;
        }

        /// <summary>
        /// Gets the api token.
        /// </summary>
        public string ApiToken
        {
            get
            {
                return this._apiToken ?? this.GetApiToken();
            }
        }

        /// <summary>
        /// The get api token.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GetApiToken()
        {
            var response = (JObject)Requests.GetJson<object>("https://toggl.com/api/v8/me", credentials: new NetworkCredential(this._username, this._password));
            var data = (JObject)response.GetValue("data");
            this._apiToken = data.GetValue("api_token").ToString();
            this._userId = data.GetValue("id").ToString();
            return this._apiToken;
        }

        /// <summary>
        /// The get summary.
        /// </summary>
        /// <param name="workspaceId">
        /// The workspace id.
        /// </param>
        /// <param name="starDate">
        /// The star date.
        /// </param>
        /// <param name="endDate">
        /// The end date.
        /// </param>
        /// <returns>
        /// The <see cref="TogglSummaryReport"/>.
        /// </returns>
        public TogglSummaryReport GetSummary(string workspaceId, DateTime starDate, DateTime endDate)
        {
            return Requests.GetJson<TogglSummaryReport>(
                "https://toggl.com/reports/api/v2/summary?user_agent=jira",
                credentials: new NetworkCredential(this.ApiToken, "api_token"),
                queryArgs:
                    new
                    {
                        user_agent = "jira",
                        workspace_id = workspaceId,
                        user_ids = this._userId,
                        since = starDate.ToString("yyyy-MM-dd"),
                        until = endDate.ToString("yyyy-MM-dd")
                    });
        }

        /// <summary>
        /// The toggl summary report entry title.
        /// </summary>
        public class TogglSummaryReportEntryTitle
        {
            /// <summary>
            /// Gets or sets the text.
            /// </summary>
            [JsonProperty("time_entry")]
            public string Text { get; set; }
        }

        /// <summary>
        /// The toggl summary report entry item.
        /// </summary>
        public class TogglSummaryReportEntryItem
        {
            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            [JsonProperty("title")]
            public TogglSummaryReportEntryTitle Title { get; set; }

            /// <summary>
            /// Gets or sets the miliseconds.
            /// </summary>
            [JsonProperty("time")]
            public int Miliseconds { get; set; }
        }

        /// <summary>
        /// The toggl summary report group title.
        /// </summary>
        public class TogglSummaryReportGroupTitle
        {
            /// <summary>
            /// Gets or sets the project.
            /// </summary>
            [JsonProperty("project")]
            public string Project { get; set; }

            /// <summary>
            /// Gets or sets the client.
            /// </summary>
            [JsonProperty("client")]
            public string Client { get; set; }
        }

        /// <summary>
        /// The toggl summary report group item.
        /// </summary>
        public class TogglSummaryReportGroupItem
        {
            /// <summary>
            /// Gets or sets the id.
            /// </summary>
            [JsonProperty("id")]
            public string Id { get; set; }

            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            [JsonProperty("title")]
            public TogglSummaryReportGroupTitle Title { get; set; }

            /// <summary>
            /// Gets or sets the miliseconds.
            /// </summary>
            [JsonProperty("time")]
            public long Miliseconds { get; set; }

            /// <summary>
            /// Gets or sets the items.
            /// </summary>
            [JsonProperty("items")]
            public IEnumerable<TogglSummaryReportEntryItem> Items { get; set; }
        }

        /// <summary>
        /// The toggl summary report.
        /// </summary>
        public class TogglSummaryReport
        {
            /// <summary>
            /// Gets or sets the total grand miliseconds.
            /// </summary>
            [JsonProperty("total_grand")]
            public long? TotalGrandMiliseconds { get; set; }

            /// <summary>
            /// Gets or sets the total billable miliseconds.
            /// </summary>
            [JsonProperty("total_billable")]
            public long? TotalBillableMiliseconds { get; set; }

            /// <summary>
            /// Gets or sets the groups.
            /// </summary>
            [JsonProperty("data")]
            public IEnumerable<TogglSummaryReportGroupItem> Groups { get; set; }
        }
    }
}
