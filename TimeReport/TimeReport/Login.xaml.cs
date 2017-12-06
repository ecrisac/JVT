using System.ComponentModel;
using System.Windows;

namespace TimeReport
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
            
            
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            App.Current.Shutdown();
        }

        private void DoLogin(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default["UserNameJira"] = UserNameTxtJira.Text;
            Properties.Settings.Default["PasswordJira"] = PasswordBoxJira.Password;
            Properties.Settings.Default["UserNameToggl"] = UserNameTxtToggl.Text;
            Properties.Settings.Default["PasswordToggl"] = PasswordBoxToggl.Password;
            Properties.Settings.Default.Save(); // Saves settings in application configuration file
            Close();
        }
    }
}
