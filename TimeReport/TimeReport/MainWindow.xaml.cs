using System.Configuration;
using System.Windows;
using TimeReport.Core.Configuration;

namespace TimeReport
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            //var userNameJira = Properties.Settings.Default["UserNameJira"].ToString();
            //if (string.IsNullOrWhiteSpace(userNameJira))
            //    new Login().ShowDialog();
            //userNameJira = Properties.Settings.Default["UserNameJira"].ToString();
            //var passwordJira = Properties.Settings.Default["PasswordJira"].ToString();
            //var userNameToggl = Properties.Settings.Default["UserNameToggl"].ToString();
            //var passwordToggl = Properties.Settings.Default["PasswordToggl"].ToString();


            var appState = new AppState(JiraProfilesConfigurationSection.Current.List)
            {
                State = "Loading...",
            };

            DataContext = appState;
        }
    }
}