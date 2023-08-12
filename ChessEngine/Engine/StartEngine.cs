using ChessEngine.bitboard;

namespace ChessEngine.Engine;

public class StartEngine
{
    public static void Start()
    {
        // const string fen = "r2qk2r/pppbnppp/2n1p3/3pP3/1b1P4/2N2N2/PPP1BPPP/R1BQK2R w KQkq - 5 7";

        const string fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

        Bitboards.LoadBitboardsFromFen(fen);

        MovementMasks.CreateMovementMasks();
        MovementMasks.GenerateRookMovesLookup();
        MovementMasks.GenerateBishopMovesLookup();


        ulong[] currentBitboards ={
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
            Bitboards.BlackKingBitboard
        };

        bool[] currentCastling ={
            true,
            true,
            true,
            true,
            true,
            true
        };

        board.Board.AllBitboardsMoves.Add((currentBitboards, currentCastling));
    }
}