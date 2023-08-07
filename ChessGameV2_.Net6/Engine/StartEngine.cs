using ChessGame.board;

namespace ChessGame;
using bitboards;

public class StartEngine
{
    public static void Start()
    {
        const string fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
        // const string fen = "1k6/8/8/3Qr3/8/7R/1B2K3/8 w - - 0 1";
        
        Bitboards.LoadBitboardsFromFen(fen);
        MovementMasks.CreateMovementMasks();
        MovementMasks.GenerateRookMovesLookup();
        MovementMasks.GenerateBishopMovesLookup();
        
        
    }
}
