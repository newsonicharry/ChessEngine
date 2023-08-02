using ChessGame.board;

namespace ChessGame;
using bitboards;

public class StartEngine
{
    public static void Start()
    {   
        
        
        const string fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
        // const string fen = "8/4p3/8/8/ppp1Rp2/8/4p3/8 w - - 0 1";

        Bitboards.LoadBitboardsFromFen(fen);
        MovementMasks.CreateMovementMasks();
        MovementMasks.GenerateRookMovesLookup();
        MovementMasks.GenerateBishopMovesLookup();
        
        
    }
}
