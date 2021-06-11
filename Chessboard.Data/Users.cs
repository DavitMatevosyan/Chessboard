namespace Chessboard.Data
{
    public class Users
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string IconUri { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }
    }
}
