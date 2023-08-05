using ChessGame.bitboards;
using ChessGame.board;

namespace ChessGame;

public class Engine
{
    public static void MakeMove()
    {
        ulong[] validMoves = ValidMoves.FindValidMoves();
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
        
        Board.UpdateBoard(BoardUtils.IndexToPieceBitboard(moves[index][0]), moves[index][0], moves[index][1]); 

        
    }
}