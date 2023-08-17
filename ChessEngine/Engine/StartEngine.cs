using ChessEngine.bitboard;
using ChessEngine.board;

namespace ChessEngine.Engine;

public class StartEngine
{
    public static void Start()
    {
        // const string fen = "r2qk2r/pppbnppp/2n1p3/3pP3/1b1P4/2N2N2/PPP1BPPP/R1BQK2R w KQkq - 5 7";
        // const string fen = "r1b1k2r/ppp2ppp/2n1p3/5qP1/1bpPn2P/5N2/PP2PPB1/RNBQK1R1 w Qkq - 2 10";
        const string fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

        Bitboards.LoadBitboardsFromFen(fen);
        
        Piece.UpdatePieceArray();
        
        MovementMasks.CreateMovementMasks();
        MovementMasks.GenerateRookMovesLookup();
        MovementMasks.GenerateBishopMovesLookup();

        Transpositions.InitializeZobristKeys();
        Transpositions.LoadZobrist();
        
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

        bool[] currentCastling ={true,true,true,true,true,true};

        board.Board.PastMoves.Add((currentBitboards, currentCastling, Transpositions.ZobristHash));
        
    }
}