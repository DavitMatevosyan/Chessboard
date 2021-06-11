using System;

namespace Chessboard.Logic.Exceptions
{
    public class PositionFormatException : Exception
    {
        public PositionFormatException()
        {

        }

        public PositionFormatException(string message) : base(message)
        {

        }

        public PositionFormatException(string message, Exception inner) : base(message, inner)
        {

        }

    }
}
