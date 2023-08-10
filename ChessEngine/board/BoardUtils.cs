using ChessEngine.bitboard;

namespace ChessEngine.Board;

public class BoardUtils
{
    public static int IndexToFile(int index) 
    {
        return index / 8;
    }

    public static int IndexToRank(int index)
    {
        return index % 8;  // simpler equation then i would have thought
    }

    public static ulong GetWhiteBitboard()
    {
        return  Bitboards.WhitePawnBitboard | Bitboards.WhiteKnightBitboard | Bitboards.WhiteBishopBitboard | Bitboards.WhiteRookBitboard | Bitboards.WhiteQueenBitboard | Bitboards.WhiteKingBitboard;
    }

    public static ulong GetBlackBitboard()
    {
        return Bitboards.BlackPawnBitboard | Bitboards.BlackKnightBitboard | Bitboards.BlackBishopBitboard | Bitboards.BlackRookBitboard | Bitboards.BlackQueenBitboard | Bitboards.BlackKingBitboard;

    }

    public static ushort IndexToPieceType(int index)
    {
        
        ulong binaryIndex = 1ul << index;
        
        if ((binaryIndex | Bitboards.WhitePawnBitboard) == Bitboards.WhitePawnBitboard) { return 1; }
        if ((binaryIndex | Bitboards.WhiteKnightBitboard) == Bitboards.WhiteKnightBitboard) { return 2; }
        if ((binaryIndex | Bitboards.WhiteBishopBitboard) == Bitboards.WhiteBishopBitboard) { return 3; }
        if ((binaryIndex | Bitboards.WhiteRookBitboard) == Bitboards.WhiteRookBitboard) { return 4; }
        if ((binaryIndex | Bitboards.WhiteQueenBitboard) == Bitboards.WhiteQueenBitboard) { return 5; }
        if ((binaryIndex | Bitboards.WhiteKingBitboard) == Bitboards.WhiteKingBitboard) { return 6; }
        if ((binaryIndex | Bitboards.BlackPawnBitboard) == Bitboards.BlackPawnBitboard) { return 7; }
        if ((binaryIndex | Bitboards.BlackKnightBitboard) == Bitboards.BlackKnightBitboard) { return 8; }
        if ((binaryIndex | Bitboards.BlackBishopBitboard) == Bitboards.BlackBishopBitboard) { return 9; }
        if ((binaryIndex | Bitboards.BlackRookBitboard) == Bitboards.BlackRookBitboard) { return 10; }
        if ((binaryIndex | Bitboards.BlackQueenBitboard) == Bitboards.BlackQueenBitboard) { return 11; }
        if ((binaryIndex | Bitboards.BlackKingBitboard) == Bitboards.BlackKingBitboard) { return 12; }

        return 0;

    }
    
    
    public static ulong IndexToPieceBitboard(int index)
    {
        
        ulong binaryIndex = 1ul << index;
        
        if ((binaryIndex | Bitboards.WhitePawnBitboard) == Bitboards.WhitePawnBitboard) { return Bitboards.WhitePawnBitboard; }
        if ((binaryIndex | Bitboards.WhiteKnightBitboard) == Bitboards.WhiteKnightBitboard) { return Bitboards.WhiteKnightBitboard; }
        if ((binaryIndex | Bitboards.WhiteBishopBitboard) == Bitboards.WhiteBishopBitboard) { return Bitboards.WhiteBishopBitboard; }
        if ((binaryIndex | Bitboards.WhiteRookBitboard) == Bitboards.WhiteRookBitboard) { return Bitboards.WhiteRookBitboard; }
        if ((binaryIndex | Bitboards.WhiteQueenBitboard) == Bitboards.WhiteQueenBitboard) { return Bitboards.WhiteQueenBitboard; }
        if ((binaryIndex | Bitboards.WhiteKingBitboard) == Bitboards.WhiteKingBitboard) { return Bitboards.WhiteKingBitboard; }
        if ((binaryIndex | Bitboards.BlackPawnBitboard) == Bitboards.BlackPawnBitboard) { return Bitboards.BlackPawnBitboard; }
        if ((binaryIndex | Bitboards.BlackKnightBitboard) == Bitboards.BlackKnightBitboard) { return Bitboards.BlackKnightBitboard; }
        if ((binaryIndex | Bitboards.BlackBishopBitboard) == Bitboards.BlackBishopBitboard) { return Bitboards.BlackBishopBitboard; }
        if ((binaryIndex | Bitboards.BlackRookBitboard) == Bitboards.BlackRookBitboard) { return Bitboards.BlackRookBitboard; }
        if ((binaryIndex | Bitboards.BlackQueenBitboard) == Bitboards.BlackQueenBitboard) { return Bitboards.BlackQueenBitboard; }
        if ((binaryIndex | Bitboards.BlackKingBitboard) == Bitboards.BlackKingBitboard) { return Bitboards.BlackKingBitboard; }

        return 0;

    }

    public static ushort EncodeMove(int startingSquare, int endingSquare, int pieceIndex)
    {   
        ushort encodedMove = (ushort)(startingSquare << 10);
        encodedMove |= (ushort)(endingSquare << 4);
        encodedMove |= (ushort)(pieceIndex);
        
        return encodedMove;
        
    }


    public static (int, int, int) DecodeMove(ushort move)
    {
        int startingSquare = (ushort)(move >> 10);
        int endingSquare = (ushort)(move << 6) >> 10;
        int piece = (ushort)(move << 12) >> 12;

        return (startingSquare, endingSquare, piece);

    }
    
    
}