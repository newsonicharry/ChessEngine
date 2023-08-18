using ChessEngine.bitboard;
using ChessEngine.board;
using ChessEngine.Board;

namespace ChessEngine.Engine;

public class StartEngine
{
    public static void Start()
    {
        const string fen = "r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N4p/PPPBBPPP/R2QK2R w KQkq - 5 7";
        // const string fen = "r1b1kb1r/ppppqppp/5n2/8/1n2P3/2N5/PPP1QPPP/R1B1KBNR b KQkq - 0 1";
        // const string fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
        

        Bitboards.LoadBitboardsFromFen(fen);
        
        Piece.UpdatePieceArray();
        
        MovementMasks.CreateMovementMasks();
        MovementMasks.GenerateRookMovesLookup();
        MovementMasks.GenerateBishopMovesLookup();

        Transpositions.InitializeZobristKeys();
        Transpositions.LoadZobrist();
        
        
        board.Board.SwitchCurrentPlayerTurn();
        ValidMoves.FindValidMoves();
        board.Board.SwitchCurrentPlayerTurn();

        
        board.Board.BoardData boardData = new board.Board.BoardData
        {
            WhitePawnBitboard = Bitboards.WhitePawnBitboard,
            WhiteKnightBitboard = Bitboards.WhiteKnightBitboard,
            WhiteBishopBitboard = Bitboards.WhiteBishopBitboard,
            WhiteRookBitboard = Bitboards.WhiteRookBitboard,
            WhiteQueenBitboard = Bitboards.WhiteQueenBitboard,
            WhiteKingBitboard = Bitboards.WhiteKingBitboard,
            BlackPawnBitboard = Bitboards.BlackPawnBitboard,
            BlackKnightBitboard = Bitboards.BlackKnightBitboard,
            BlackBishopBitboard = Bitboards.BlackBishopBitboard,
            BlackRookBitboard = Bitboards.BlackRookBitboard,
            BlackQueenBitboard = Bitboards.BlackQueenBitboard,
            BlackKingBitboard = Bitboards.BlackKingBitboard,
            HasMovedWhiteKing = Castling.HasMovedWhiteKing,
            HasMovedBlackKing = Castling.HasMovedBlackKing,
            HasMovedLeftWhiteRook = Castling.HasMovedLeftWhiteRook,
            HasMovedRightWhiteRook = Castling.HasMovedRightWhiteRook,
            HasMovedLeftBlackRook = Castling.HasMovedLeftBlackRook,
            HasMovedRightBlackRook = Castling.HasMovedRightBlackRook
        };


        board.Board.AllBoardPositions.Push(boardData);         
    }
}