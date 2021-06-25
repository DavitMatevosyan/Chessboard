using Chessboard.Logic.Data;
using System.Windows;

namespace Chessboard.UI.Users
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private DataManager _manager;

        private bool _playerTwo;
        private string _username;
        private string _passwordHash;

        public LoginWindow()
        {
            InitializeComponent();

            DataBuilder builder = new DataBuilder();
            _manager = builder.GetManager();
            _playerTwo = false;
        }

        public LoginWindow(DataManager manager)
        {
            InitializeComponent();

            lbl_LoginText.Content = "Log in as Player Two";
            _manager = manager;
            _playerTwo = true;
            btn_ContinueWOLogin.Visibility = Visibility.Hidden;
        }

        private void btn_LoginClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txt_Username.Text) || string.IsNullOrEmpty(txt_Password.Password))
            {
                MessageBox.Show("Insufficient Data Entered", "Insufficient Data", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            _username = txt_Username.Text;
            _passwordHash = Logic.HasherCaller.CallHasher(txt_Password.Password);


            if (_playerTwo)
            {
                if (_manager.LogInPlayerTwo(_username, _passwordHash) == -1)
                {
                    MessageBox.Show("Incorrect User Data", "Wrong Data", MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.OK);
                    return;
                }
            }
            else
            {
                if (_manager.LogIn(_username, _passwordHash) == -1)
                {
                    MessageBox.Show("Incorrect User Data", "Wrong Data", MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.OK);
                    return;
                }

                MainWindow window = new MainWindow(_manager);
                window.Show();
            }

           
            this.Close();
        }

        private void btn_CreateUserClick(object sender, RoutedEventArgs e)
        {
            CreateUserWindow signUpWindow = new CreateUserWindow();
            signUpWindow.Show();
            signUpWindow.Activate();
            this.Close();
        }

        private void btn_ContinueWithoutLogin(object sender, RoutedEventArgs e)
        {
            MainWindow window = new MainWindow();
            window.Show();
            this.Close();
        }
    }
}
