using ChessGame.board;

namespace ChessGame;
using bitboards;

public class StartEngine
{
    public static void Start()
    {
        const string fen = "b5k1/8/8/8/1q2Q1K1/8/8/8 w - - 0 1";

        Bitboards.LoadBitboardsFromFen(fen);
        MovementMasks.CreateMovementMasks();
        MovementMasks.GenerateRookMovesLookup();
        MovementMasks.GenerateBishopMovesLookup();
        
        
    }
}
