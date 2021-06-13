using Microsoft.Win32;
using System.Windows;
using System.Windows.Input;

namespace Chessboard.UI.Users
{
    public partial class CreateUserWindow : Window
    {
        private string _username = "";
        private string _email = "";
        private string _passwordHash = "";
        private string _iconUri = "";
        private bool _hasCreatedAccount = false;

        public CreateUserWindow()
        {
            InitializeComponent();
        }

        private void btn_BrowseIconClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.InitialDirectory = "Pictures";
            fd.Filter = "All files (*.*) | *.*";

            if (fd.ShowDialog().Value)
                _iconUri = fd.FileName;
        }

        private void btn_CreateAccountClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txt_Username.Text) ||
                string.IsNullOrEmpty(txt_Password.Password))
            {
                MessageBox.Show("Insufficient Data Entered", "Insufficient Data", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            _username = txt_Username.Text;
            _passwordHash = Logic.HasherCaller.CallHasher(txt_Password.Password);

            if (string.IsNullOrEmpty(txt_Email.Text))
            {
                var result = MessageBox.Show("You haven't Entered an Email.\n Do you want to enter an email before proceeding?", "Attention", MessageBoxButton.YesNo, MessageBoxImage.Information);
                if (result == MessageBoxResult.Yes)
                    return;
            }
            _email =  txt_Email.Text;

            MessageBox.Show($"{_username}\n{_email}\n{_passwordHash}");
        }

        /// <summary>
        /// Event calls when no account is created and this window is called to close
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult result;
            if (!_hasCreatedAccount)
            {
                result = MessageBox.Show("No accounts were created, Are you sure you want to close?", "Remainder", MessageBoxButton.YesNoCancel, MessageBoxImage.Exclamation);
                if (result != MessageBoxResult.Yes)
                    e.Cancel = true;
            }

        }
    }
}
