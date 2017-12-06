using System;
using System.Windows;

namespace TimeReport
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class LogMessage : Window
    {
        public LogMessage(string plat)
        { 
            InitializeComponent();
            Picker.Text = DateTime.Now.ToString();
            
            lbl.Content = string.Format("Log message: {0}",plat);
        }
        private void DoLogin(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
