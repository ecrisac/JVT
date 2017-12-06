using System.Text.RegularExpressions;

namespace TimeReport.Core
{
    public class TogglLoggedItem: Toggl.TogglSummaryReportEntryItem
    {
        private static readonly Regex IssueKeyRegex = new Regex(@"[A-Z]+-\d+");

        private string _issue;

        private string ExtractIssue()
        {
            var match = IssueKeyRegex.Match(Title.Text);

            if(match.Success)
            {
                _issue = match.Value;
            }
            else
            {
                _issue = string.Empty;
            }

            return _issue;
        }

        public TogglLoggedItem(Toggl.TogglSummaryReportEntryItem baseItem)
        {
            Title = baseItem.Title;
            Miliseconds = baseItem.Miliseconds;
        }

        public string Issue
        {
            get { return _issue ?? ExtractIssue(); }
        }

        public bool LinkedIssue
        {
            get { return !string.IsNullOrEmpty(Issue); }
        }
    }
}