using ChessEngine.Bitboards;
using ChessEngine.Board;

namespace ChessEngine.Engine;

public abstract class Engine
{
    public static void MakeMove()
    {
        ulong[] validMoves = ValidMoves.FindValidMoves();
        
        bool checkmate = true;
        foreach (ulong validMove in validMoves)
        {
            if (validMove != 0)
            {
                checkmate = false;
            }
        }

        if (checkmate)
        {
            Board.Board.GameOver = true;
            Console.WriteLine("checkmate/stalemate");
        }

        if (!Board.Board.GameOver)
        {
            List<int[]> moves = new List<int[]>();
        
            for (int i = 0; i < validMoves.Length; i++)
            {
                ulong validMove = validMoves[i];
                if (validMove != 0)
                {
                    int[] moveIndexes = BitboardUtils.GetSetBitIndexes(validMove);
        
                    foreach (int moveIndex in moveIndexes)
                    {
                        moves.Add(new []{i, moveIndex});
                    }
                
                }
            }
        
        
            var random = new Random();
            int index = random.Next(moves.Count);
        
            Board.Board.UpdateBoard(BoardUtils.IndexToPieceBitboard(moves[index][0]), moves[index][0], moves[index][1]); 
        }
        


        
    }
}