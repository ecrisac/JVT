using System;
using System.Configuration;
using System.Diagnostics;
using TimeReport.Core;
using UI;

namespace TimeReport
{
    public class LoggedIssueModel : BindableBase
    {
        private readonly LoggedIssue issue;
        private readonly AppState appState;
        private readonly JiraItem selectedJira;
        private string newJiraTime;

        public LoggedIssueModel(LoggedIssue issue, AppState appState, JiraItem selectedJira)
        {
            this.issue = issue;
            this.appState = appState;
            this.selectedJira = selectedJira;
            RequestNavigateCommand = new RelayCommand(Navigate, _ => true);
            SyncItemCommand = new RelayCommand(_ => Sync(), _ => true);
            SyncTestingItemCommand = new RelayCommand(_ => SyncAsTesting(), _ => true);
            newJiraTime = issue.Diff;
        }

        private void SyncAsTesting()
        {
            var logMessage = new LogMessage(issue.PLAT);
            var result = logMessage.ShowDialog();
            if (!result.HasValue || !result.Value) return;
            SyncManager.Sync(issue.DiffSeconds, issue.PLAT, selectedJira,
                ConfigurationManager.AppSettings["TestingKey"] + " " + logMessage.UserComment.Text, DateTime.Parse(logMessage.Picker.Text));
            issue.Sync();
            OnPropertyChanged(() => TotalJira);
            OnPropertyChanged(() => IsSyncVisible);
        }

        public RelayCommand SyncItemCommand { get; private set; }
        public RelayCommand SyncTestingItemCommand { get; private set; }
        public RelayCommand RequestNavigateCommand { get; private set; }

        public string Plat
        {
            get { return issue.PLAT; }
        }

        public string TotalJira
        {
            get { return issue.TotalJira; }
        }

        public string NewJiraTime
        {
            get { return newJiraTime; }
            set
            {
                SetProperty(ref newJiraTime , value);

                var tempString = value.Replace("m", ":").Replace("h",":").TrimEnd(':');
                issue.DiffSeconds = TimeSpan.Parse(tempString).TotalSeconds;
            }
        }
       
        public string TotalToggl
        {
            get { return issue.TotalToggl; }
        }

        public string Diff
        {
            get { return issue.Diff; }
        }

        public string Comment
        {
            get { return string.Join(",", issue.Issue.Fields.Summary); }
        }

        public bool IsSyncVisible
        {
            get { return issue.IsSyncVisible; }
        }

        private void Navigate(object _)
        {
            Process.Start(new ProcessStartInfo(issue.Issue.Url));
        }

        public void Sync()
        {
            var logMessage = new LogMessage(issue.PLAT);
            var result = logMessage.ShowDialog();
            if (!result.HasValue || !result.Value) return;
            SyncManager.Sync(issue.DiffSeconds, issue.PLAT, selectedJira, logMessage.UserComment.Text, DateTime.Parse(logMessage.Picker.Text));
            issue.Sync();
            OnPropertyChanged(() => TotalJira);
            OnPropertyChanged(() => IsSyncVisible);
        }
    }
}