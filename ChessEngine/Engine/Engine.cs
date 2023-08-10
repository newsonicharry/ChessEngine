using ChessEngine.bitboard;
using ChessEngine.Board;

namespace ChessEngine.Engine;

public abstract class Engine
{
    private const int PawnValue = 100;
    private const int KnightValue = 300;
    private const int BishopValue = 320;
    private const int RookValue = 500;
    private const int QueenValue = 900;
    private const int KingValue = 100000;

    
    public static void MakeMove()
    {
        ushort[] validMoves = ValidMoves.FindValidMoves();

        var random = new Random();
        int index = random.Next(validMoves.Length);
        
        
        board.Board.UpdateBoard(validMoves[index]);
        
        
    }
}