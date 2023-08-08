using ChessEngine.Bitboards;

namespace ChessEngine.Engine;

public class StartEngine
{
    public static void Start()
    {
        // const string fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
        const string fen = "8/8/5k2/4n3/3K4/4n3/P7/8 w - - 0 1";
        
        Bitboards.Bitboards.LoadBitboardsFromFen(fen);
        MovementMasks.CreateMovementMasks();
        MovementMasks.GenerateRookMovesLookup();
        MovementMasks.GenerateBishopMovesLookup();
        
        
    }
}
