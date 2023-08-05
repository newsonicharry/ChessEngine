using ChessGame.board;

namespace ChessGame;
using bitboards;

public class StartEngine
{
    public static void Start()
    {
        const string fen = "8/8/8/8/1r4K1/2P5/2N5/4R3 w - - 0 1";

        Bitboards.LoadBitboardsFromFen(fen);
        MovementMasks.CreateMovementMasks();
        MovementMasks.GenerateRookMovesLookup();
        MovementMasks.GenerateBishopMovesLookup();
        
        
    }
}
