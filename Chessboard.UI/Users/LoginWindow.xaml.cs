using System.Windows;
using System.Windows.Input;

namespace Chessboard.UI.Users
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void TextBox_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void btn_LoginClick(object sender, RoutedEventArgs e)
        {

        }

        private void btn_CreateUserClick(object sender, RoutedEventArgs e)
        {
            CreateUserWindow signUpWindow = new CreateUserWindow();
            signUpWindow.Show();
            signUpWindow.Activate();
            this.Close();
        }
    }
}
