﻿namespace Chessboard.Data
{
    public class Games
    {
        public int Id { get; set; }
        public int PlayerOneId { get; set; }
        public int PlayerTwoId { get; set; }
        public int WinnerId { get; set; }
        public string GameHistory { get; set; }
        public string CurrentBoard { get; set; }
    }
}