namespace ChessGame.board;
using bitboards;

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
    
}