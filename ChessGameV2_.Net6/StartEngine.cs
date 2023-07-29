namespace ChessGame;
using bitboards;

public class StartEngine
{
    public static void Start()
    {   
        
        
        const string fen = "rnbqkbnr/pppppppp/8/8/3P4/8/PPP1PPPP/RNBQKBNR w KQkq - 0 1";
        
        Bitboards.LoadBitboardsFromFen(fen);
            
        MovementMasks.CreateMovementMasks();
        MovementMasks.GenerateRookMovesLookup();
        MovementMasks.GenerateBishopMovesLookup();

        
    }
}
