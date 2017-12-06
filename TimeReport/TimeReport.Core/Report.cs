using System;
using System.Collections.Generic;
using System.Linq;
using TimeReport.Core.Configuration;

namespace TimeReport.Core
{
    public class Report
    {
        public IEnumerable<LoggedIssue> Issues { get; private set; }

        public IEnumerable<TogglLoggedItem> Other { get; private set; }

        public Report()
        {
            Issues = new List<LoggedIssue>();
            Other = new List<TogglLoggedItem>();
        }

        public Report Load(ToggleConfigurationSection toggle, string url, string password, string login, string authorName, DateTime startDate)
        {
            var issues = (List<LoggedIssue>)Issues;
            issues.Clear();

            var toggl = new Toggl(toggle);
            var jiraServer = new JiraNew(url, password, login, authorName);
            var firstDay = new DateTime(startDate.Year, startDate.Month, 1);
            var endOfMonth = new DateTime(startDate.Year, startDate.Month, DateTime.DaysInMonth(startDate.Year, startDate.Month));

            var togglSummary = toggl.GetSummary(toggle.Workspace, firstDay.AddMonths(-1), endOfMonth);
            var togglItems = togglSummary.Groups.SelectMany(g => g.Items).Select(i => new TogglLoggedItem(i)).ToList();

            var togglGroups = togglItems.GroupBy(i => i.Issue).OrderByDescending(i => i.Key).ToList();
            var jiraResult = jiraServer.GetIssues(togglGroups.Select(x => x.Key).Where(x => !string.IsNullOrWhiteSpace(x)).ToList());
            foreach (var group in togglGroups)
            {
                if (!string.IsNullOrEmpty(group.Key))
                {
                    JiraNew.Issue issue = jiraResult.Issues.Single(x => x.Key == group.Key);
                    var jiraWorkLog = issue.Fields.Worklog ?? jiraServer.GetWorkLog(group.Key);
                    jiraWorkLog.Items = jiraWorkLog.Items.Where(i => i.Author.Name == jiraServer.AuthorName).ToList();
                    var loggedIssue = new LoggedIssue(group.Key, group, jiraWorkLog, issue, jiraServer);
                    issues.Add(loggedIssue);
                }
            }

            return this;
        }
       
    }
}
