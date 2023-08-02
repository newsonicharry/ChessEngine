using ChessGame.board;

namespace ChessGame;
using bitboards;

public class StartEngine
{
    public static void Start()
    {
        const string fen = "8/1b4k1/8/3P4/r1B1K3/8/5P2/8 w - - 0 1";
        // const string fen = "8/1b4k1/8/3P4/r1BB1K2/8/5P2/8 w - - 0 1";

        Bitboards.LoadBitboardsFromFen(fen);
        MovementMasks.CreateMovementMasks();
        MovementMasks.GenerateRookMovesLookup();
        MovementMasks.GenerateBishopMovesLookup();
        
        
    }
}
