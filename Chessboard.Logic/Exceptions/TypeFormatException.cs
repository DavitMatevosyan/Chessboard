using System;

namespace Chessboard.Logic.Exceptions
{
    public class TypeFormatException : Exception
    {
        public TypeFormatException()
        {

        }

        public TypeFormatException(string message) : base(message)
        {

        }

        public TypeFormatException(string message, Exception inner) : base(message, inner)
        {

        }

    }
}
