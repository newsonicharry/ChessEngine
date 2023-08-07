using ChessEngine.Bitboards;

namespace ChessEngine.Engine;

public class StartEngine
{
    public static void Start()
    {
        // const string fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
        const string fen = "rnbqkbnr/ppppp1pp/5p2/7B/8/8/PPPPPPPP/RNB1KBNR b KQkq - 0 1";
        
        Bitboards.Bitboards.LoadBitboardsFromFen(fen);
        MovementMasks.CreateMovementMasks();
        MovementMasks.GenerateRookMovesLookup();
        MovementMasks.GenerateBishopMovesLookup();
        
        
    }
}
