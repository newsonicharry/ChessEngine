using ChessEngine.Bitboards;

namespace ChessEngine.Engine;

public class StartEngine
{
    public static void Start()
    {
        const string fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
        // const string fen = "r1b1kQ1r/p1qp2pp/2n1p3/1pp1NP2/5P2/8/PPPP2PP/RNB1KB1R b KQkq - 0 1";
        
        Bitboards.Bitboards.LoadBitboardsFromFen(fen);
        MovementMasks.CreateMovementMasks();
        MovementMasks.GenerateRookMovesLookup();
        MovementMasks.GenerateBishopMovesLookup();
        
        
    }
}
