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
        return  Bitboards.Bitboards.WhitePawnBitboard | Bitboards.Bitboards.WhiteKnightBitboard | Bitboards.Bitboards.WhiteBishopBitboard | Bitboards.Bitboards.WhiteRookBitboard | Bitboards.Bitboards.WhiteQueenBitboard | Bitboards.Bitboards.WhiteKingBitboard;
    }

    public static ulong GetBlackBitboard()
    {
        return Bitboards.Bitboards.BlackPawnBitboard | Bitboards.Bitboards.BlackKnightBitboard | Bitboards.Bitboards.BlackBishopBitboard | Bitboards.Bitboards.BlackRookBitboard | Bitboards.Bitboards.BlackQueenBitboard | Bitboards.Bitboards.BlackKingBitboard;

    }

    public static ushort IndexToPieceType(int index)
    {
        
        ulong binaryIndex = 1ul << index;
        
        if ((binaryIndex | Bitboards.Bitboards.WhitePawnBitboard) == Bitboards.Bitboards.WhitePawnBitboard) { return 1; }
        if ((binaryIndex | Bitboards.Bitboards.WhiteKnightBitboard) == Bitboards.Bitboards.WhiteKnightBitboard) { return 2; }
        if ((binaryIndex | Bitboards.Bitboards.WhiteBishopBitboard) == Bitboards.Bitboards.WhiteBishopBitboard) { return 3; }
        if ((binaryIndex | Bitboards.Bitboards.WhiteRookBitboard) == Bitboards.Bitboards.WhiteRookBitboard) { return 4; }
        if ((binaryIndex | Bitboards.Bitboards.WhiteQueenBitboard) == Bitboards.Bitboards.WhiteQueenBitboard) { return 5; }
        if ((binaryIndex | Bitboards.Bitboards.WhiteKingBitboard) == Bitboards.Bitboards.WhiteKingBitboard) { return 6; }
        if ((binaryIndex | Bitboards.Bitboards.BlackPawnBitboard) == Bitboards.Bitboards.BlackPawnBitboard) { return 7; }
        if ((binaryIndex | Bitboards.Bitboards.BlackKnightBitboard) == Bitboards.Bitboards.BlackKnightBitboard) { return 8; }
        if ((binaryIndex | Bitboards.Bitboards.BlackBishopBitboard) == Bitboards.Bitboards.BlackBishopBitboard) { return 9; }
        if ((binaryIndex | Bitboards.Bitboards.BlackRookBitboard) == Bitboards.Bitboards.BlackRookBitboard) { return 10; }
        if ((binaryIndex | Bitboards.Bitboards.BlackQueenBitboard) == Bitboards.Bitboards.BlackQueenBitboard) { return 11; }
        if ((binaryIndex | Bitboards.Bitboards.BlackKingBitboard) == Bitboards.Bitboards.BlackKingBitboard) { return 12; }

        return 0;

    }
    
    
    public static ulong IndexToPieceBitboard(int index)
    {
        
        ulong binaryIndex = 1ul << index;
        
        if ((binaryIndex | Bitboards.Bitboards.WhitePawnBitboard) == Bitboards.Bitboards.WhitePawnBitboard) { return Bitboards.Bitboards.WhitePawnBitboard; }
        if ((binaryIndex | Bitboards.Bitboards.WhiteKnightBitboard) == Bitboards.Bitboards.WhiteKnightBitboard) { return Bitboards.Bitboards.WhiteKnightBitboard; }
        if ((binaryIndex | Bitboards.Bitboards.WhiteBishopBitboard) == Bitboards.Bitboards.WhiteBishopBitboard) { return Bitboards.Bitboards.WhiteBishopBitboard; }
        if ((binaryIndex | Bitboards.Bitboards.WhiteRookBitboard) == Bitboards.Bitboards.WhiteRookBitboard) { return Bitboards.Bitboards.WhiteRookBitboard; }
        if ((binaryIndex | Bitboards.Bitboards.WhiteQueenBitboard) == Bitboards.Bitboards.WhiteQueenBitboard) { return Bitboards.Bitboards.WhiteQueenBitboard; }
        if ((binaryIndex | Bitboards.Bitboards.WhiteKingBitboard) == Bitboards.Bitboards.WhiteKingBitboard) { return Bitboards.Bitboards.WhiteKingBitboard; }
        if ((binaryIndex | Bitboards.Bitboards.BlackPawnBitboard) == Bitboards.Bitboards.BlackPawnBitboard) { return Bitboards.Bitboards.BlackPawnBitboard; }
        if ((binaryIndex | Bitboards.Bitboards.BlackKnightBitboard) == Bitboards.Bitboards.BlackKnightBitboard) { return Bitboards.Bitboards.BlackKnightBitboard; }
        if ((binaryIndex | Bitboards.Bitboards.BlackBishopBitboard) == Bitboards.Bitboards.BlackBishopBitboard) { return Bitboards.Bitboards.BlackBishopBitboard; }
        if ((binaryIndex | Bitboards.Bitboards.BlackRookBitboard) == Bitboards.Bitboards.BlackRookBitboard) { return Bitboards.Bitboards.BlackRookBitboard; }
        if ((binaryIndex | Bitboards.Bitboards.BlackQueenBitboard) == Bitboards.Bitboards.BlackQueenBitboard) { return Bitboards.Bitboards.BlackQueenBitboard; }
        if ((binaryIndex | Bitboards.Bitboards.BlackKingBitboard) == Bitboards.Bitboards.BlackKingBitboard) { return Bitboards.Bitboards.BlackKingBitboard; }

        return 0;

    }

    public static ushort EncodeMove(int startingSquare, int endingSquare)
    {   
        ushort encodedMove = (ushort)(startingSquare << 8);              // starting square index
        encodedMove |= (ushort)(endingSquare << 4);                      // ending square index
        encodedMove |= (ushort)(IndexToPieceType(startingSquare) << 4);  // piece type

        return encodedMove;
        
    }


    public static (int, int, int) DecodeMove(ushort move)
    {
        int startingSquare = move >> 8;
        int endingSquare = (move << 8) >> 12;
        int piece = (move << 12) >> 12;

        return (startingSquare, endingSquare, piece);

    }
    
    
}