using ChessEngine.bitboard;

namespace ChessEngine.Engine;

public class StartEngine
{
    public static void Start()
    {
        const string fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
        // const string fen = "rnbqkbnr/pppppppp/8/8/qK6/8/PPPPPPPP/RNBQ1BNR w kq - 0 1";
        
        Bitboards.LoadBitboardsFromFen(fen);
        board.Board.LoadMaterialFromFen(fen);
        
        MovementMasks.CreateMovementMasks();
        MovementMasks.GenerateRookMovesLookup();
        MovementMasks.GenerateBishopMovesLookup();
        
        
        board.Board.AllBitboardsMoves.Add(new []{
            Bitboards.WhitePawnBitboard,
            Bitboards.WhiteKnightBitboard,
            Bitboards.WhiteBishopBitboard,
            Bitboards.WhiteRookBitboard,
            Bitboards.WhiteQueenBitboard,
            Bitboards.WhiteKingBitboard,
            Bitboards.BlackPawnBitboard,
            Bitboards.BlackKnightBitboard,
            Bitboards.BlackBishopBitboard,
            Bitboards.BlackRookBitboard,
            Bitboards.BlackQueenBitboard,
            Bitboards.BlackKingBitboard});


    }
}
