using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using TimeReport.Core;
using TimeReport.Core.Configuration;
using UI;

namespace TimeReport
{
    public class AppState : BindableBase
    {
        private bool isLoaded;
        private ObservableCollection<LoggedIssueModel> items;
        private string searchText;
        private ICollectionView source;
        private string state;
        private List<JiraItem> jiras;
        private JiraItem selectedJira;
        private bool isNotLoading;
        private DateTime startDate;

        public AppState()
        {
            startDate = DateTime.Now;
            SyncAll = new RelayCommand(DoSync, _ => true);
            Refresh = new RelayCommand(DoRefresh, _ => IsNotLoading);
            IsNotLoading = true;
        }

        public AppState(JiraProfilesConfigurationSection.Collection jiraProfiles)
            : this()
        {
            Jiras = new List<JiraItem>(from JiraProfilesConfigurationSection.JiraProfileSettings jiraProfile in jiraProfiles
                                       select
                                           new JiraItem
                                           {
                                               Name = new Uri(jiraProfile.FormattedUrl).Host,
                                               Url = jiraProfile.FormattedUrl,
                                               UserName = jiraProfile.Login,
                                               Password = jiraProfile.Password,
                                               AuthorName = jiraProfile.Name
                                           });

            SelectedJira = Jiras.FirstOrDefault();
        }

        public JiraItem SelectedJira
        {
            get { return selectedJira; }
            set
            {
                SetProperty(ref selectedJira, value);
                LoadReport();
            }
        }

        private void LoadReport()
        {
            var context = TaskScheduler.FromCurrentSynchronizationContext();
            IsNotLoading = false;
            Task.Factory.StartNew
                (
                    () =>
                    {
                        var report = new Report();
                        report.Load(ToggleConfigurationSection.Current, selectedJira.Url, selectedJira.Password, selectedJira.UserName,
                            selectedJira.AuthorName,startDate);

                        return report;
                    }
                )
                .ContinueWith
                (
                    r =>
                    {
                        var loggedIssueModels = new ObservableCollection<LoggedIssueModel>();
                        foreach (var issue in r.Result.Issues.OrderByDescending(x => x.LastUpdate))
                            loggedIssueModels.Add(new LoggedIssueModel(issue, this, selectedJira));
                        Items = loggedIssueModels;
                        State = "Sync";
                        IsNotLoading = true;
                    },
                    context
                ).ContinueWith(_ =>
                {
                    State = "Error occurred";
                    IsNotLoading = true;
                }, TaskContinuationOptions.OnlyOnFaulted);
        }

        public DateTime StartDate
        {
            get { return startDate; }
            set { SetProperty(ref startDate, value);LoadReport(); }
        }

        public bool IsNotLoading
        {
            get { return isNotLoading; }
            set { SetProperty(ref isNotLoading, value); }
        }

        public List<JiraItem> Jiras
        {
            get { return jiras; }
            set { SetProperty(ref jiras, value); }
        }

        public bool IsLoaded
        {
            get { return isLoaded; }
            set { SetProperty(ref isLoaded, value); }
        }

        public ICollectionView Source
        {
            get { return source; }
            set { SetProperty(ref source, value); }
        }

        public string SearchText
        {
            get { return searchText; }
            set
            {
                SetProperty(ref searchText, value);
                source.Refresh();
            }
        }

        public string State
        {
            get { return state; }
            set { SetProperty(ref state, value); }
        }

        public ObservableCollection<LoggedIssueModel> Items
        {
            get { return items; }
            set
            {
                SetProperty(ref items, value);
                var customerView = CollectionViewSource.GetDefaultView(items);
                customerView.Filter = Filter;
                Source = customerView;
                IsLoaded = true;
            }
        }

        public RelayCommand SyncAll { get; set; }
        public RelayCommand Refresh { get; set; }

        private void DoSync(object obj)
        {
            foreach (var issue in Items.Where(x => x.IsSyncVisible))
                issue.Sync();
        }
        private void DoRefresh(object obj)
        {
            LoadReport();
        }

        private bool Filter(object obj)
        {
            if (string.IsNullOrWhiteSpace(searchText)) return true;

            var item = (LoggedIssueModel)obj;
            return item.Comment.ToLower().Contains(searchText.ToLower()) || item.Plat.ToLower().Contains(searchText);
        }
    }
}