using System;
using System.Collections.Generic;
using System.Linq;

namespace TimeReport.Core
{
    public class LoggedIssue
    {
        private readonly JiraNew jiraServer;
        public JiraNew.Issue Issue { get; private set; }
        private static readonly DateTime EpochStart = new DateTime(1970, 1, 1);

        public string PLAT { get; private set; }

        public IEnumerable<TogglLoggedItem> Toggl { get; private set; }

        public JiraNew.JiraWorkLog WorkLog { get; private set; }

        public LoggedIssue(string plat, IEnumerable<TogglLoggedItem> toggl, JiraNew.JiraWorkLog workLog, JiraNew.Issue issue, JiraNew jiraServer)
        {
            this.jiraServer = jiraServer;
            Issue = issue;
            PLAT = plat;
            Toggl = toggl;
            WorkLog = workLog;
            CalculateDiff();
        }

        public double DiffSeconds { get; set; }

        public bool IsSyncVisible { get { return DiffSeconds > 60; } }

        public DateTime? LastUpdate
        {
            get
            {
                var last = WorkLog.Items.LastOrDefault();
                return last == null? EpochStart : last.Updated ?? last.Created;
            }
        }

        public string TotalJira
        {
            get
            {
                return TimeSpan.FromSeconds(WorkLog.Items.Sum(i => i.TimeSpendSeconds)).ToHumanReadable();
            }
        }

        public string Diff { get; private set; }

        public string TotalToggl
        {
            get
            {
                return TimeSpan.FromMilliseconds(Toggl.Sum(i => i.Miliseconds)).ToHumanReadable();
            }
        }

        public void Sync()
        {
            var issue = jiraServer.GetIssue(PLAT);
            WorkLog.Items = issue.Fields.Worklog.Items.Where(x => x.Author.Name == jiraServer.AuthorName);
            CalculateDiff();
        }

        private void CalculateDiff()
        {
            var jiraTime = TimeSpan.FromSeconds(WorkLog.Items.Sum(i => i.TimeSpendSeconds));
            var toggleTime = TimeSpan.FromMilliseconds(Toggl.Sum(i => i.Miliseconds));
            var diff = toggleTime - jiraTime;
            Diff = diff.ToHumanReadable();
            DiffSeconds = diff.TotalSeconds;
        }
    }
}