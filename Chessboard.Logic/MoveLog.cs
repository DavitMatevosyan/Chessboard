namespace Chessboard.Logic
{
    public class MoveLog
    {
        private static int idCounter = 1;

        public int ID { get; set; }
        public string WhiteMove { get; set; }
        public string BlackMove { get; set; }

        public MoveLog()
        {
            ID = idCounter++;
        }
    }

}
