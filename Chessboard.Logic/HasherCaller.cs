namespace Chessboard.Logic
{
    /// <summary>
    /// Single method that calls the Hasher Method from Models
    /// </summary>
    public static class HasherCaller
    {
        public static string CallHasher(string pass)
            => Chessboard.Models.Hash.Hasher.GetHash(pass);
    }
}
