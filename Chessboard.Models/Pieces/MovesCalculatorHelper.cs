using Chessboard.Models.Positions;
using System.Collections.Generic;
using System.Linq;

namespace Chessboard.Models.Pieces
{
    /// <summary>
    /// Helper class for Calculating the moves
    /// </summary>
    internal static class MovesCalculatorHelper
    {
        #region Horizontal Vertical

        /// <summary>
        /// Calculates a given piece's Moves vertically relative to <paramref name="AllPieces"/>
        /// </summary>
        /// <param name="piece"></param>
        /// <param name="AllPieces"></param>
        internal static void CalculateMovesVertically(this IPiece piece, List<IPiece> AllPieces)
        {
            Position pos = new Position(piece.Position.Column, piece.Position.Row);
            var collidingPieces = AllPieces.Where(x => x.Position.Column == pos.Column).ToList();
            int direction = 1;
            if (pos.Row != 8)
            {
                while (pos.Row < 8)
                {
                    pos.Row += direction;
                    if (CompareAndAdd(piece, pos, collidingPieces))
                        break;
                }
            }
            pos.Row = piece.Position.Row;
            if (pos.Row != 1)
            {
                while (pos.Row > 1)
                {
                    pos.Row -= direction;
                    if (CompareAndAdd(piece, pos, collidingPieces))
                        break;
                }
            }
        }

        /// <summary>
        /// Calculates a given piece's Moves Horizontally relative to <paramref name="AllPieces"/>
        /// </summary>
        /// <param name="piece"></param>
        /// <param name="AllPieces"></param>
        internal static void CalculateMovesHorizontally(this IPiece piece, List<IPiece> AllPieces)
        {
            Position pos = new Position(piece.Position.Column, piece.Position.Row);
            var collidingPieces = AllPieces.Where(x => x.Position.Row == pos.Row).ToList();
            int direction = 1;
            if (pos.Column != 8)
            {
                while (pos.Column < 8)
                {
                    pos.Column += direction;
                    if (CompareAndAdd(piece, pos, collidingPieces))
                        break;
                }
            }
            pos.Column = piece.Position.Column;
            if (pos.Column != 1)
            {
                while (pos.Column > 1)
                {
                    pos.Column -= direction;
                    if (CompareAndAdd(piece, pos, collidingPieces))
                        break;
                }
            }


        }

        #endregion

        #region Diagonals

        /// <summary>
        /// Calculates a given piece's Moves forward diagonally relative to <paramref name="AllPieces"/>
        /// </summary>
        /// <param name="piece"></param>
        /// <param name="AllPieces"></param>
        internal static void CalculateMovesForwardDiagonally(this IPiece piece, List<IPiece> AllPieces)
        {
            Position pos = new Position(piece.Position.Column, piece.Position.Row);
            int direction = 1;
            if (pos.Column != 8 && pos.Row != 8)
            {
                while (pos.Column < 8 && pos.Row < 8)
                {
                    pos.Column += direction;
                    pos.Row += direction;
                    if (CompareAndAdd(piece, pos, AllPieces))
                        break;
                }
            }
            pos.Column = piece.Position.Column;
            pos.Row = piece.Position.Row;
            if (pos.Column != 1 && pos.Row != 1)
            {
                while (pos.Column > 1 && pos.Row > 1)
                {
                    pos.Column -= direction;
                    pos.Row -= direction;
                    if (CompareAndAdd(piece, pos, AllPieces))
                        break;
                }
            }
        }

        /// <summary>
        /// Calculates a given piece's Moves backwards diagonally relative to <paramref name="AllPieces"/>
        /// </summary>
        /// <param name="piece"></param>
        /// <param name="AllPieces"></param>
        internal static void CalculateMovesBackwardDiagonally(this IPiece piece, List<IPiece> AllPieces)
        {
            Position pos = new Position(piece.Position.Column, piece.Position.Row);
            int direction = 1;

            if (pos.Column != 1 && pos.Row != 8)
            {
                while (pos.Column > 1 && pos.Row < 8)
                {
                    pos.Column -= direction;
                    pos.Row += direction;
                    if (CompareAndAdd(piece, pos, AllPieces))
                        break;
                }
            }
            pos.Column = piece.Position.Column;
            pos.Row = piece.Position.Row;
            if (pos.Column != 8 && pos.Row != 1)
            {
                while (pos.Column < 8 && pos.Row > 1)
                {
                    pos.Column += direction;
                    pos.Row -= direction;
                    if (CompareAndAdd(piece, pos, AllPieces))
                        break;
                }
            }

        }

        #endregion

        #region King Moves

        /// <summary>
        /// Calculates King Moves, Relative to <paramref name="UnavailableMoves"/>
        /// </summary>
        /// <param name="king"></param>
        /// <param name="AllPieces"></param>
        /// <param name="UnavailableMoves"></param>
        internal static void CalculateKingMoves(this King king, List<IPiece> AllPieces, List<Move> UnavailableMoves)
        {
            Position pos = new Position(king.Position.Column, king.Position.Row);
            for (int col = -1; col <= 1; col++)
            {
                pos.Column += col;
                for (int row = -1; row <= 1; row++)
                {
                    pos.Row += row;
                    if (pos.IsInBounds())
                    {
                        if (king.Position != pos)
                            king.Neighbourhood.Add(new Move(king.Name, new Position(pos.Column, pos.Row)));
                    }
                    pos.Row = king.Position.Row;
                }
                pos.Column = king.Position.Column;
            }

            foreach (var item in king.Neighbourhood)
            {
                if (!UnavailableMoves.Any(x => x.Position == item.Position) && !AllPieces.Any(x => x.Position == item.Position && x.Color == king.Color
                                                                                || (x.Position == item.Position && x.Color != king.Color && x.IsDefended)))
                {
                    king.AvailableMoves.Add(new Move(king.Name, item.Position));
                }
            }
            CalculateKingCastling(king, AllPieces, UnavailableMoves);
        }

        private static void CalculateKingCastling(King king, List<IPiece> allPieces, List<Move> unavailableMoves)
        {
            if (king.MoveCount > 0)
                return;
            var allRooks = allPieces.Where(x => x is Rook && x.Color == king.Color).ToList();
            if (allRooks.Count == 0)
                return;
            foreach (var rook in allRooks)
            {
                if (rook.MoveCount > 0)
                    continue;
                Position pos = new Position(king.Position.Column, king.Position.Row);
                int direction = rook.Position.Column > king.Position.Column ? 1 : -1;
                bool canCastle = false;
                while (pos.Column != rook.Position.Column - direction)
                {
                    if(king.Position.Row != rook.Position.Row)
                    {
                        canCastle = false;
                        break;
                    }

                    pos.Column += direction;
                    var collidingPiece = PieceHelpers.PieceAtPosition(allPieces, pos);
                    if (collidingPiece is not null || unavailableMoves.ContainsPosition(pos))
                    {
                        canCastle = false;
                        break;
                    }
                    canCastle = true;
                }
                if(canCastle)
                {
                    Position newPos = new Position(king.Position.Column + (direction * 2), king.Position.Row);
                    Move move = new Move($"{king.Name}{rook.Name}", newPos);
                    king.AvailableMoves.Add(move);
                }
            }

        }

        #endregion

        #region Knight Moves

        /// <summary>
        /// Calculates All Knight Moves
        /// </summary>
        /// <param name="knight"></param>
        /// <param name="AllPieces"></param>
        internal static void CalculateKnightMoves(this Knight knight, List<IPiece> AllPieces)
        {
            Position pos = new Position(knight.Position.Column, knight.Position.Row);
            CheckAddKnightMove(knight, new Position(pos.Column + 1, pos.Row + 2), AllPieces);
            CheckAddKnightMove(knight, new Position(pos.Column + 2, pos.Row + 1), AllPieces);
            CheckAddKnightMove(knight, new Position(pos.Column + 2, pos.Row - 1), AllPieces);
            CheckAddKnightMove(knight, new Position(pos.Column + 1, pos.Row - 2), AllPieces);
            CheckAddKnightMove(knight, new Position(pos.Column - 1, pos.Row - 2), AllPieces);
            CheckAddKnightMove(knight, new Position(pos.Column - 2, pos.Row - 1), AllPieces);
            CheckAddKnightMove(knight, new Position(pos.Column - 2, pos.Row + 1), AllPieces);
            CheckAddKnightMove(knight, new Position(pos.Column - 1, pos.Row + 2), AllPieces);

        }

        /// <summary>
        /// Checks for individual Coordinate if valid adds to the available moves
        /// </summary>
        /// <param name="knight"></param>
        /// <param name="pos"></param>
        /// <param name="AllPieces"></param>
        private static void CheckAddKnightMove(Knight knight, Position pos, List<IPiece> AllPieces)
        {
            var collidingPiece = AllPieces.PieceAtPosition(pos);
            if (pos.IsInBounds())
            {
                if (collidingPiece is not null)
                {
                    if (collidingPiece.Color != knight.Color)
                        knight.AvailableMoves.Add(new Move(knight.Name, pos));
                }
                else
                    knight.AvailableMoves.Add(new Move(knight.Name, pos));

            }

        }

        #endregion

        #region Pawn Moves

        /// <summary>
        /// Calculates the Available moves for pawn
        /// </summary>
        /// <param name="pawn"></param>
        /// <param name="AllPieces"></param>
        internal static void CalculatePawnMoves(this Pawn pawn, List<IPiece> AllPieces)
        {
            Position pos = new Position(pawn.Position.Column, pawn.Position.Row);
            int direction = 0;
            if (pawn.Color == Color.White)
                direction += 1;
            else
                direction -= 1;

            CalculatePawnMovingMoves(pawn, AllPieces, direction);
            CalculatePawnHittingMoves(pawn, AllPieces, direction);

        }

        /// <summary>
        /// Calculates the positions where the pawn can move without eating a piece
        /// </summary>
        /// <param name="pawn"></param>
        /// <param name="AllPieces"></param>
        /// <param name="direction"></param>
        private static void CalculatePawnMovingMoves(Pawn pawn, List<IPiece> AllPieces, int direction)
        {
            var testPos = new Position(pawn.Position.Column, pawn.Position.Row + direction);
            if (AllPieces.PieceAtPosition(testPos) is null)
            {
                pawn.AvailableMoves.Add(new Move(pawn.Name, new Position(testPos.Column, testPos.Row)));
                if (pawn.MoveCount == 0)
                {
                    testPos.Row += direction;
                    if (AllPieces.PieceAtPosition(testPos) is null)
                    {
                        pawn.AvailableMoves.Add(new Move(pawn.Name, new Position(testPos.Column, testPos.Row)));
                    }
                }
            }
        }

        private static void CalculatePawnHittingMoves(Pawn pawn, List<IPiece> AllPieces, int direction)
        {
            Position testPos;
            IPiece collidingPiece;
            for (int i = -1; i <= 1; i += 2)
            {
                testPos = new Position(pawn.Position.Column + i, pawn.Position.Row + direction);
                collidingPiece = AllPieces.PieceAtPosition(testPos);

                if (collidingPiece is not null && testPos.IsInBounds())
                {
                    if (collidingPiece.Color != pawn.Color)
                        pawn.AvailableMoves.Add(new Move(pawn.Name, testPos));
                }
                else if (collidingPiece is null && testPos.IsInBounds())
                {
                    pawn.HittingMoves.Add(new Move(pawn.Name, new Position(testPos.Column, testPos.Row)));
                }
            }

            for (int i = -1; i <= 1; i += 2)
            {
                testPos = new Position(pawn.Position.Column + i, pawn.Position.Row);
                collidingPiece = AllPieces.PieceAtPosition(testPos);
                if (testPos.IsInBounds())
                {
                    if (collidingPiece is not null)
                    {
                        if (collidingPiece is Pawn)
                        {
                            var collidingPawn = collidingPiece as Pawn;
                            if (collidingPawn.MoveCount == 1)
                            {
                                pawn.AvailableMoves.Add(new Move(pawn.Name, new Position(testPos.Column, testPos.Row)));
                            }
                        }

                    }

                }

            }
        }

        #endregion

        #region General 

        /// <summary>
        /// finds if there is a piece at given <paramref name="pos"/> if not adds the given <paramref name="piece"/>'s 
        /// data to the piece's available move
        /// </summary>
        /// <param name="piece"></param>
        /// <param name="pos"></param>
        /// <param name="collidingPieces"></param>
        /// <returns></returns>
        private static bool CompareAndAdd(IPiece piece, Position pos, List<IPiece> collidingPieces)
        {
            var collidingPiece = collidingPieces.PieceAtPosition(pos);
            if (collidingPiece is null)
                piece.AvailableMoves.Add(new Move(piece.Name, new Position(pos.Column, pos.Row)));
            else
            {
                if (collidingPiece.Color != piece.Color)
                {
                    piece.AvailableMoves.Add(new Move(piece.Name, new Position(pos.Column, pos.Row)));
                    if (collidingPiece is King)
                        return false;
                }
                if (collidingPiece.Color == piece.Color)
                {
                    var defendedPiece = PieceHelpers.PieceAtPosition(collidingPieces, new Position(pos.Column, pos.Row));
                    defendedPiece.IsDefended = true;
                    piece.DefendingPieces.Add(defendedPiece);
                }
                return true;
            }
            return false;
        }

        internal static void ResetDefendingPieces(List<IPiece> allPieces)
        {
            foreach (var item in allPieces)
            {
                item.IsDefended = false;
            }
            foreach (var item in allPieces)
            {
                foreach (var defended in item.DefendingPieces)
                {
                    defended.IsDefended = true;
                }
            }
        }

        #endregion

        #region Filtering Under Check

        /// <summary>
        /// checks what is the direction of the attack and calls respective method to calculate the move
        /// </summary>
        /// <param name="piece">Attacking piece</param>
        /// <param name="king">King under hit</param>
        /// <param name="coloredPieces">all pieces that have the same color as the king</param>
        /// <param name="moves">All available moves for that colored pieces</param>
        /// <returns></returns>
        internal static List<Move> FilterUnderCheckMoves(IPiece piece, King king, List<IPiece> coloredPieces, List<Move> moves)
        {
            if (piece is Knight knight)
                return FilterUnderCheckKnight(knight, king, coloredPieces, moves);
            if (piece.Position.Column == king.Position.Column)
                return FilterUnderCheckVertical(piece, king, coloredPieces, moves);
            if (piece.Position.Row == king.Position.Row)
                return FilterUnderCheckHorizontal(piece, king, coloredPieces, moves);
            if ((piece.Position.Row >= king.Position.Row && piece.Position.Column >= king.Position.Column)
                        || (piece.Position.Row <= king.Position.Row && piece.Position.Column <= king.Position.Column))
                return FilterUnderCheckForwardDiagonal(piece, king, coloredPieces, moves);
            if ((piece.Position.Row >= king.Position.Row && piece.Position.Column <= king.Position.Column)
                        || (piece.Position.Row <= king.Position.Row && piece.Position.Column >= king.Position.Column))
                return FilterUnderCheckBackwardDiagonal(piece, king, coloredPieces, moves);

            return null;
        }

        /// <summary>
        /// Filtes the available moves when king is under hit by a knight
        /// </summary>
        /// <param name="knight">The knight that hits the king</param>
        /// <param name="king">under hit king</param>
        /// <param name="coloredPieces">same colored pieces as the king</param>
        /// <param name="moves">all available moves that have the same color as the king</param>
        /// <returns></returns>
        private static List<Move> FilterUnderCheckKnight(Knight knight, King king, List<IPiece> coloredPieces, List<Move> moves)
        {
            List<Position> newMoves = new List<Position>();
            newMoves.Add(knight.Position);

            return FilterMoves(king, coloredPieces, newMoves);
        }

        /// <summary>
        /// Filters moves when the hit is from a vertical line
        /// </summary>
        /// <param name="piece"></param>
        /// <param name="king"></param>
        /// <param name="coloredPieces"></param>
        /// <param name="moves"></param>
        /// <returns></returns>
        private static List<Move> FilterUnderCheckVertical(IPiece piece, King king, List<IPiece> coloredPieces, List<Move> moves)
        {
            List<Position> newMoves = new List<Position>();
            int direction = piece.Position.Row < king.Position.Row ? 1 : -1;
            Position iterator = new Position(piece.Position.Column, piece.Position.Row);
            while (iterator != king.Position)
            {
                newMoves.Add(new Position(iterator.Column, iterator.Row));
                iterator.Row += direction;
            }

            return FilterMoves(king, coloredPieces, newMoves);
        }

        /// <summary>
        /// Filters moves when the hit is from a horizontal line
        /// </summary>
        /// <param name="piece"></param>
        /// <param name="king"></param>
        /// <param name="coloredPieces"></param>
        /// <param name="moves"></param>
        /// <returns></returns>
        private static List<Move> FilterUnderCheckHorizontal(IPiece piece, King king, List<IPiece> coloredPieces, List<Move> moves)
        {
            List<Position> newMoves = new List<Position>();
            int direction = piece.Position.Column < king.Position.Column ? 1 : -1;
            Position iterator = new Position(piece.Position.Column, piece.Position.Row);
            while (iterator != king.Position)
            {
                newMoves.Add(new Position(iterator.Column, iterator.Row));
                iterator.Column += direction;
            }
            return FilterMoves(king, coloredPieces, newMoves);
        }

        /// <summary>
        /// Filters moves when the hit is from a forward diagonal line
        /// </summary>
        /// <param name="piece"></param>
        /// <param name="king"></param>
        /// <param name="coloredPieces"></param>
        /// <param name="moves"></param>
        /// <returns></returns>
        private static List<Move> FilterUnderCheckForwardDiagonal(IPiece piece, King king, List<IPiece> coloredPieces, List<Move> moves)
        {
            List<Position> newMoves = new List<Position>();
            int direction = piece.Position.Column < king.Position.Column ? 1 : -1;
            Position iterator = new Position(piece.Position.Column, piece.Position.Row);
            while (iterator != king.Position)
            {
                newMoves.Add(new Position(iterator.Column, iterator.Row));
                iterator.Row += direction;
                iterator.Column += direction;
            }

            return FilterMoves(king, coloredPieces, newMoves);
        }

        /// <summary>
        /// Filters moves when the hit is from a backwards diagonal line
        /// </summary>
        /// <param name="piece"></param>
        /// <param name="king"></param>
        /// <param name="coloredPieces"></param>
        /// <param name="moves"></param>
        /// <returns></returns>
        private static List<Move> FilterUnderCheckBackwardDiagonal(IPiece piece, King king, List<IPiece> coloredPieces, List<Move> moves)
        {
            List<Position> newMoves = new List<Position>();
            int colDirection = piece.Position.Column < king.Position.Column ? 1 : -1;
            int rowDirection = piece.Position.Row < king.Position.Row ? 1 : -1;
            Position iterator = new Position(piece.Position.Column, piece.Position.Row);
            while (iterator != king.Position)
            {
                newMoves.Add(new Position(iterator.Column, iterator.Row));
                iterator.Row += rowDirection;
                iterator.Column += colDirection;
            }

            return FilterMoves(king, coloredPieces, newMoves);
        }

        /// <summary>
        /// Filters the available moves for each piece taking available moves as the moves that can move to <paramref name="newMoves"/>
        /// </summary>
        /// <param name="coloredPieces">Same colored as King</param>
        /// <param name="newMoves">New list of available Positions</param>
        /// <returns></returns>
        private static List<Move> FilterMoves(King king, List<IPiece> coloredPieces, List<Position> newMoves)
        {
            List<Move> result = new List<Move>();
            foreach (var piece in coloredPieces)
            {
                if (piece is King)
                    continue;
                List<Move> avMoves = new List<Move>();
                foreach (var move in piece.AvailableMoves)
                {
                    if (newMoves.Any(x => x == move.Position))
                        avMoves.Add(move);
                }
                piece.AvailableMoves = avMoves;
                result.AddRange(avMoves);
            }
            result.AddRange(king.AvailableMoves);

            return result;
        }

        #endregion

        #region Filtering Chain Check

        /// <summary>
        /// Filters for each king whether the king is in a chain hit or not, if yes filters that piece's available moves
        /// </summary>
        /// <param name="allPieces"></param>
        internal static void FilterKingsAreUnderChain(List<IPiece> allPieces)
        {
            var king = allPieces.First(x => x is King && x.Color == Color.White) as King;
            FilterColoredKingisUnderChain(king, allPieces);

            king = allPieces.First(x => x is King && x.Color == Color.Black) as King;
            FilterColoredKingisUnderChain(king, allPieces);
        }

        /// <summary>
        /// Filters for each color's pieces' available moves
        /// </summary>
        /// <param name="king"></param>
        /// <param name="allPieces"></param>
        private static void FilterColoredKingisUnderChain(King king, List<IPiece> allPieces)
        {
            FilterColoredKingIsUnderChainVertical(king, allPieces);
            FilterColoredKingIsUnderChainHorizontal(king, allPieces);
            FilterColoredKingIsUnderChainForwardDiagonally(king, allPieces);
            FilterColoredKingIsUnderChainBackwardDiagonally(king, allPieces);
        }

        #region Filter Colored King Is Under Chain

        /// <summary>
        /// Vertical Filtering
        /// </summary>
        /// <param name="king"></param>
        /// <param name="allPieces"></param>
        private static void FilterColoredKingIsUnderChainVertical(King king, List<IPiece> allPieces)
        {
            Position pos = new Position(king.Position.Column, king.Position.Row);
            List<Position> chainList = new List<Position>();
            IPiece attackingPiece = null;
            int direction = 1;
            if (pos.Row != 8)
            {
                while (pos.Row < 8 || attackingPiece is not null)
                {
                    pos.Row += direction;
                    attackingPiece = PieceHelpers.PieceAtPosition(allPieces.Where(x => x.Color != king.Color).ToList(), pos);
                    chainList.Add(new Position(pos.Column, pos.Row));
                    if (attackingPiece is not null)
                    {
                        if (attackingPiece is IAttackingPiece)
                            CheckAndFilterVertical(king, attackingPiece, allPieces, chainList);
                        break;
                    }
                }
            }
            pos.Row = king.Position.Row;
            chainList = new List<Position>();
            attackingPiece = null;
            if (pos.Row != 1 || attackingPiece is not null)
            {
                while (pos.Row > 1)
                {
                    pos.Row -= direction;
                    attackingPiece = PieceHelpers.PieceAtPosition(allPieces.Where(x => x.Color != king.Color).ToList(), pos);
                    chainList.Add(new Position(pos.Column, pos.Row));
                    if (attackingPiece is not null)
                    {
                        if (attackingPiece is IAttackingPiece)
                            CheckAndFilterVertical(king, attackingPiece, allPieces, chainList);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Horizontal Filtering
        /// </summary>
        /// <param name="king"></param>
        /// <param name="allPieces"></param>
        private static void FilterColoredKingIsUnderChainHorizontal(King king, List<IPiece> allPieces)
        {
            Position pos = new Position(king.Position.Column, king.Position.Row);
            List<Position> chainList = new List<Position>();
            IPiece attackingPiece = null;
            int direction = 1;
            if (pos.Column != 8)
            {
                while (pos.Column < 8 || attackingPiece is not null)
                {
                    pos.Column += direction;
                    attackingPiece = PieceHelpers.PieceAtPosition(allPieces.Where(x => x.Color != king.Color).ToList(), pos);
                    chainList.Add(new Position(pos.Column, pos.Row));
                    if (attackingPiece is not null)
                    {
                        if (attackingPiece is IAttackingPiece)
                            CheckAndFilterHorizontal(king, attackingPiece, allPieces, chainList);
                        break;
                    }
                }
            }
            pos.Column = king.Position.Column;
            chainList = new List<Position>();
            attackingPiece = null;
            if (pos.Column != 1)
            {
                while (pos.Column > 1 || attackingPiece is not null)
                {
                    pos.Column -= direction;
                    attackingPiece = PieceHelpers.PieceAtPosition(allPieces.Where(x => x.Color != king.Color).ToList(), pos);
                    chainList.Add(new Position(pos.Column, pos.Row));
                    if (attackingPiece is not null)
                    {
                        if (attackingPiece is IAttackingPiece)
                            CheckAndFilterHorizontal(king, attackingPiece, allPieces, chainList);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Forward Diagonal Filtering
        /// </summary>
        /// <param name="king"></param>
        /// <param name="allPieces"></param>
        private static void FilterColoredKingIsUnderChainForwardDiagonally(King king, List<IPiece> allPieces)
        {
            Position pos = new Position(king.Position.Column, king.Position.Row);
            List<Position> chainList = new List<Position>();
            IPiece attackingPiece = null;
            int direction = 1;
            if (pos.Column != 8 || pos.Row != 8)
            {
                while ((pos.Column < 8 && pos.Row < 8) || attackingPiece is not null)
                {
                    pos.Row += direction;
                    pos.Column += direction;
                    attackingPiece = PieceHelpers.PieceAtPosition(allPieces.Where(x => x.Color != king.Color).ToList(), pos);
                    chainList.Add(new Position(pos.Column, pos.Row));
                    if (attackingPiece is not null)
                    {
                        if (attackingPiece is IAttackingPiece)
                            CheckAndFilterDiagonal(king, attackingPiece, allPieces, chainList);
                        break;
                    }
                }
            }
            pos.Row = king.Position.Row;
            pos.Column = king.Position.Column;
            chainList = new List<Position>();
            attackingPiece = null;
            if (pos.Column != 1 || pos.Row != 1)
            {
                while ((pos.Column > 1 && pos.Row > 1) || attackingPiece is not null)
                {
                    pos.Row -= direction;
                    pos.Column -= direction;
                    attackingPiece = PieceHelpers.PieceAtPosition(allPieces.Where(x => x.Color != king.Color).ToList(), pos);
                    chainList.Add(new Position(pos.Column, pos.Row));
                    if (attackingPiece is not null)
                    {
                        if (attackingPiece is IAttackingPiece)
                            CheckAndFilterDiagonal(king, attackingPiece, allPieces, chainList);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Backwards Diagonal Filtering
        /// </summary>
        /// <param name="king"></param>
        /// <param name="allPieces"></param>
        private static void FilterColoredKingIsUnderChainBackwardDiagonally(King king, List<IPiece> allPieces)
        {
            Position pos = new Position(king.Position.Column, king.Position.Row);
            List<Position> chainList = new List<Position>();
            IPiece attackingPiece = null;
            int direction = 1;
            if (pos.Column != 1 || pos.Row != 8)
            {
                while ((pos.Column > 1 && pos.Row < 8) || attackingPiece is not null)
                {
                    pos.Row += direction;
                    pos.Column -= direction;
                    attackingPiece = PieceHelpers.PieceAtPosition(allPieces.Where(x => x.Color != king.Color).ToList(), pos);
                    chainList.Add(new Position(pos.Column, pos.Row));
                    if (attackingPiece is not null)
                    {
                        if (attackingPiece is IAttackingPiece)
                            CheckAndFilterDiagonal(king, attackingPiece, allPieces, chainList);
                        break;
                    }
                }
            }
            pos.Row = king.Position.Row;
            pos.Column = king.Position.Column;
            chainList = new List<Position>();
            attackingPiece = null;
            if (pos.Column != 8 || pos.Row != 1)
            {
                while ((pos.Column < 8 && pos.Row > 1) || attackingPiece is not null)
                {
                    pos.Row -= direction;
                    pos.Column += direction;
                    attackingPiece = PieceHelpers.PieceAtPosition(allPieces.Where(x => x.Color != king.Color).ToList(), pos);
                    chainList.Add(new Position(pos.Column, pos.Row));
                    if (attackingPiece is not null)
                    {
                        if (attackingPiece is IAttackingPiece)
                            CheckAndFilterDiagonal(king, attackingPiece, allPieces, chainList);
                        break;
                    }
                }
            }
        }


        #endregion

        #region Check and Filter Directional

        /// <summary>
        /// Checks and filters in a vertical line
        /// </summary>
        /// <param name="king"></param>
        /// <param name="attackingPiece"></param>
        /// <param name="allPieces"></param>
        /// <param name="chainList"></param>
        private static void CheckAndFilterVertical(King king, IPiece attackingPiece, List<IPiece> allPieces, List<Position> chainList)
        {
            IPiece piece = null;
            List<IPiece> pieces = new List<IPiece>();
            foreach (var item in chainList)
            {
                piece = PieceHelpers.PieceAtPosition(allPieces.Where(x => x.Color == king.Color).ToList(), item);
                if (piece is not null)
                {
                    pieces.Add(piece);
                }
            }
            if (pieces.Count == 1)
            {
                var filteringPiece = pieces.First(x => x.Color == king.Color);
                filteringPiece.AvailableMoves = filteringPiece.AvailableMoves.Where(x => x.Position.Column == attackingPiece.Position.Column).ToList();
            }
        }

        /// <summary>
        /// Checks and filters in a horizontal line
        /// </summary>
        /// <param name="king"></param>
        /// <param name="attackingPiece"></param>
        /// <param name="allPieces"></param>
        /// <param name="chainList"></param>
        private static void CheckAndFilterHorizontal(King king, IPiece attackingPiece, List<IPiece> allPieces, List<Position> chainList)
        {
            IPiece piece = null;
            List<IPiece> pieces = new List<IPiece>();
            foreach (var item in chainList)
            {
                piece = PieceHelpers.PieceAtPosition(allPieces.Where(x => x.Color == king.Color).ToList(), item);
                if (piece is not null)
                {
                    pieces.Add(piece);
                }
            }
            if (pieces.Count == 1)
            {
                var filteringPiece = pieces.First(x => x.Color == king.Color);
                filteringPiece.AvailableMoves = filteringPiece.AvailableMoves.Where(x => x.Position.Row == attackingPiece.Position.Row).ToList();
            }

        }

        /// <summary>
        /// Checks and filters in a diagonal line
        /// </summary>
        /// <param name="king"></param>
        /// <param name="attackingPiece"></param>
        /// <param name="allPieces"></param>
        /// <param name="chainList"></param>
        private static void CheckAndFilterDiagonal(King king, IPiece attackingPiece, List<IPiece> allPieces, List<Position> chainList)
        {
            IPiece piece = null;
            List<IPiece> pieces = new List<IPiece>();
            foreach (var item in chainList)
            {
                piece = PieceHelpers.PieceAtPosition(allPieces.Where(x => x.Color == king.Color).ToList(), item);
                if (piece is not null)
                {
                    pieces.Add(piece);
                }
            }
            if (pieces.Count == 1)
            {
                var filteringPiece = pieces.First(x => x.Color == king.Color);
                var newMoves = filteringPiece.AvailableMoves.Where(x => chainList.ContainsPosition(x.Position)).ToList();
                filteringPiece.AvailableMoves = newMoves;
            }

        }

        #endregion

        #endregion
    }
}
