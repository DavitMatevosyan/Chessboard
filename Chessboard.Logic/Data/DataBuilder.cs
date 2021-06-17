using Chessboard.Data;

namespace Chessboard.Logic.Data
{
    /// <summary>
    /// Builder class for data
    /// Can the context can be easily changed.
    /// </summary>
    public class DataBuilder
    {
        /// <summary>
        /// Chess context, by changing this, the database cwill be changed
        /// </summary>
        private IChessContext _context;
 
        /// <summary>
        /// The manager that holds the Chess context as a property
        /// </summary>
        private DataManager _manager;

        /// <summary>
        /// Default constructor
        /// </summary>
        public DataBuilder()
        {
            _context = new ChessContext();
            _manager = new DataManager(_context);
        }

        /// <summary>
        /// Gets the appropriate manager for the database
        /// </summary>
        /// <returns></returns>
        public DataManager GetManager() => _manager;
    }
}
