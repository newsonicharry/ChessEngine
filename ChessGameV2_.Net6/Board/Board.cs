using ChessGame.bitboards;
namespace ChessGame.board;


public class Board
{

    public static bool IsWhite = true;
    
    public static bool HasMovedWhiteKing = false;
    public static bool HasMovedBlackKing = false;

    public static bool HasMovedLeftWhiteRook = false;
    public static bool HasMovedLeftBlackRook = false;
    public static bool HasMovedRightWhiteRook = false;
    public static bool HasMovedRightBlackRook = false;

    public static ulong EnemyAttackedSquares = 0ul;

    
    public static bool InCheck(ulong enemyAttackedSquares)
    {
        if (IsWhite) {
            return (Bitboards.WhiteKingBitboard & ~enemyAttackedSquares) != Bitboards.WhiteKingBitboard;
        }

        return (Bitboards.BlackKingBitboard & ~enemyAttackedSquares) != Bitboards.BlackKingBitboard;

    }
    
    public static bool CanWhiteShortCastle(ulong blackAttackedSquares)
    {
        ulong whiteBitboard = BoardUtils.GetWhiteBitboard();
        ulong blackBitboard = BoardUtils.GetBlackBitboard();
        
        ulong notOccupiedSquares = 96;

        bool squaresNotBlocked = ((blackBitboard | blackAttackedSquares | whiteBitboard) & notOccupiedSquares) == 0ul;
        
        if (!HasMovedWhiteKing & squaresNotBlocked & !HasMovedRightWhiteRook & !InCheck(blackAttackedSquares))
        {   
            return true;
        }

        return false;

    }
    
    public static bool CanWhiteLongCastle(ulong blackAttackedSquares)
    {
        ulong whiteBitboard = BoardUtils.GetWhiteBitboard();
        ulong blackBitboard = BoardUtils.GetBlackBitboard();
        
        ulong notOccupiedSquares = 14ul;

        bool squaresNotBlocked = ((blackBitboard | blackAttackedSquares | whiteBitboard) & notOccupiedSquares) == 0ul;
        
        if (!HasMovedWhiteKing & squaresNotBlocked & !HasMovedLeftWhiteRook & !InCheck(blackAttackedSquares))
        {   
            return true;
        }

        return false;

    }
    
    public static bool CanBlackShortCastle(ulong whiteAttackedSquares)
    {
        ulong whiteBitboard = BoardUtils.GetWhiteBitboard();
        ulong blackBitboard = BoardUtils.GetBlackBitboard();
        
        ulong notOccupiedSquares = 6917529027641081856ul;

        bool squaresNotBlocked = ((blackBitboard | whiteAttackedSquares | whiteBitboard) & notOccupiedSquares) == 0ul;
        
        if (!HasMovedBlackKing & squaresNotBlocked & !HasMovedRightBlackRook & !InCheck(whiteAttackedSquares))
        {   
            return true;
        }

        return false;

    }
    
    public static bool CanBlackLongCastle(ulong whiteAttackedSquares)
    {
        ulong whiteBitboard = BoardUtils.GetWhiteBitboard();
        ulong blackBitboard = BoardUtils.GetBlackBitboard();
        
        ulong notOccupiedSquares = 1008806316530991104ul;

        bool squaresNotBlocked = ((blackBitboard | whiteAttackedSquares | whiteBitboard) & notOccupiedSquares) == 0ul;
        
        if (!HasMovedBlackKing & squaresNotBlocked & !HasMovedLeftBlackRook & !InCheck(whiteAttackedSquares))
        {   
            return true;
        }

        return false;

    }
    

    public static void SwitchCurrentPlayerTurn()
    {
        IsWhite = !IsWhite;
    }
    
    
    public static void UpdateBitboards(ulong pieceBitboard, int originalIndex, int newIndex)
    {   


        bool NeedsToDeleteBit(ulong bitboard) 
        {
            return BitboardUtils.isBitOn(bitboard, (newIndex));
        }
        
        ulong DeleteBit(ulong bitboard)
        {
            return BitboardUtils.negateBit(bitboard, (newIndex));
        }

        if (originalIndex == 0 || newIndex == 0) { HasMovedLeftWhiteRook = true; }
        if (originalIndex == 7 || newIndex == 7) { HasMovedRightWhiteRook = true; }
        if (originalIndex == 56 || newIndex == 56) { HasMovedRightBlackRook = true; }
        if (originalIndex == 63 || newIndex == 63) { HasMovedLeftBlackRook = true; }


        if (NeedsToDeleteBit(Bitboards.WhitePawnBitboard)) { Bitboards.WhitePawnBitboard = DeleteBit(Bitboards.WhitePawnBitboard); }
        if (NeedsToDeleteBit(Bitboards.WhiteKnightBitboard)) { Bitboards.WhiteKnightBitboard = DeleteBit(Bitboards.WhiteKnightBitboard); }
        if (NeedsToDeleteBit(Bitboards.WhiteBishopBitboard)) { Bitboards.WhiteBishopBitboard = DeleteBit(Bitboards.WhiteBishopBitboard); }
        if (NeedsToDeleteBit(Bitboards.WhiteRookBitboard)) { Bitboards.WhiteRookBitboard = DeleteBit(Bitboards.WhiteRookBitboard); }
        if (NeedsToDeleteBit(Bitboards.WhiteQueenBitboard)) { Bitboards.WhiteQueenBitboard = DeleteBit(Bitboards.WhiteQueenBitboard); }
        if (NeedsToDeleteBit(Bitboards.WhiteKingBitboard)) { Bitboards.WhiteKingBitboard = DeleteBit(Bitboards.WhiteKingBitboard); }
        
        if (NeedsToDeleteBit(Bitboards.BlackPawnBitboard)) { Bitboards.BlackPawnBitboard = DeleteBit(Bitboards.BlackPawnBitboard); }
        if (NeedsToDeleteBit(Bitboards.BlackKnightBitboard)) { Bitboards.BlackKnightBitboard = DeleteBit(Bitboards.BlackKnightBitboard); }
        if (NeedsToDeleteBit(Bitboards.BlackBishopBitboard)) { Bitboards.BlackBishopBitboard = DeleteBit(Bitboards.BlackBishopBitboard); }
        if (NeedsToDeleteBit(Bitboards.BlackRookBitboard)) { Bitboards.BlackRookBitboard = DeleteBit(Bitboards.BlackRookBitboard); }
        if (NeedsToDeleteBit(Bitboards.BlackQueenBitboard)) { Bitboards.BlackQueenBitboard = DeleteBit(Bitboards.BlackQueenBitboard); }
        if (NeedsToDeleteBit(Bitboards.BlackKingBitboard)) { Bitboards.BlackKingBitboard = DeleteBit(Bitboards.BlackKingBitboard); }
        
        
        ulong ChangeBitPosition(ulong bitboard)
        {
            ulong newBitboard = BitboardUtils.negateBit(bitboard, (originalIndex));
            newBitboard = BitboardUtils.enableBit(newBitboard, (newIndex));
            
            return newBitboard;
        }
        
        if (pieceBitboard == Bitboards.WhitePawnBitboard) { Bitboards.WhitePawnBitboard = ChangeBitPosition(Bitboards.WhitePawnBitboard); }
        if (pieceBitboard == Bitboards.WhiteKnightBitboard) { Bitboards.WhiteKnightBitboard = ChangeBitPosition(Bitboards.WhiteKnightBitboard); }
        if (pieceBitboard == Bitboards.WhiteBishopBitboard) { Bitboards.WhiteBishopBitboard = ChangeBitPosition(Bitboards.WhiteBishopBitboard); }
        if (pieceBitboard == Bitboards.WhiteRookBitboard) { Bitboards.WhiteRookBitboard = ChangeBitPosition(Bitboards.WhiteRookBitboard); }
        if (pieceBitboard == Bitboards.WhiteQueenBitboard) { Bitboards.WhiteQueenBitboard = ChangeBitPosition(Bitboards.WhiteQueenBitboard); }

        if (pieceBitboard == Bitboards.WhiteKingBitboard)
        {
            if (newIndex == 6 & CanWhiteShortCastle(EnemyAttackedSquares))
            {
                Bitboards.WhiteRookBitboard = BitboardUtils.negateBit(Bitboards.WhiteRookBitboard, 7);
                Bitboards.WhiteRookBitboard = BitboardUtils.enableBit(Bitboards.WhiteRookBitboard, 5);
                HasMovedRightWhiteRook = true;
            }
            
            if (newIndex == 2 & CanWhiteLongCastle(EnemyAttackedSquares))
            {
                Bitboards.WhiteRookBitboard = BitboardUtils.negateBit(Bitboards.WhiteRookBitboard, 0);
                Bitboards.WhiteRookBitboard = BitboardUtils.enableBit(Bitboards.WhiteRookBitboard, 3);
                HasMovedLeftWhiteRook = true;
            }

            Bitboards.WhiteKingBitboard = ChangeBitPosition(Bitboards.WhiteKingBitboard);
            HasMovedWhiteKing = true;
            
            return;
        }

        if (pieceBitboard == Bitboards.BlackPawnBitboard) { Bitboards.BlackPawnBitboard = ChangeBitPosition(Bitboards.BlackPawnBitboard); }
        if (pieceBitboard == Bitboards.BlackKnightBitboard) { Bitboards.BlackKnightBitboard = ChangeBitPosition(Bitboards.BlackKnightBitboard); }
        if (pieceBitboard == Bitboards.BlackBishopBitboard) { Bitboards.BlackBishopBitboard = ChangeBitPosition(Bitboards.BlackBishopBitboard); }
        if (pieceBitboard == Bitboards.BlackRookBitboard) { Bitboards.BlackRookBitboard = ChangeBitPosition(Bitboards.BlackRookBitboard); }
        if (pieceBitboard == Bitboards.BlackQueenBitboard) { Bitboards.BlackQueenBitboard = ChangeBitPosition(Bitboards.BlackQueenBitboard); }

        if (pieceBitboard == Bitboards.BlackKingBitboard)
        {   
            
            if (newIndex == 62 & CanBlackShortCastle(EnemyAttackedSquares))
            {
                Bitboards.BlackRookBitboard = BitboardUtils.negateBit(Bitboards.BlackRookBitboard, 63);
                Bitboards.BlackRookBitboard = BitboardUtils.enableBit(Bitboards.BlackRookBitboard, 61);
                HasMovedRightBlackRook = true;
            }
            
            if (newIndex == 58 & CanBlackLongCastle(EnemyAttackedSquares))
            {
                Bitboards.BlackRookBitboard = BitboardUtils.negateBit(Bitboards.BlackRookBitboard, 56);
                Bitboards.BlackRookBitboard = BitboardUtils.enableBit(Bitboards.BlackRookBitboard, 59);
                HasMovedLeftBlackRook = true;
            }
            
            Bitboards.BlackKingBitboard = ChangeBitPosition(Bitboards.BlackKingBitboard);
            HasMovedBlackKing = true;

        }
        
        
        
    }
}