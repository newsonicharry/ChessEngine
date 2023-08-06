namespace ChessGame.board;

public abstract class Castling
{   
    public static bool FenWhiteCastleKingSide;
    public static bool FenWhiteCastleQueenSide;
    public static bool FenBlackCastleKingSide;
    public static bool FenBlackCastleQueenSide;
    
    public static bool HasMovedWhiteKing;
    public static bool HasMovedBlackKing;
    
    public static bool HasMovedLeftWhiteRook;
    public static bool HasMovedLeftBlackRook;
    public static bool HasMovedRightWhiteRook;
    public static bool HasMovedRightBlackRook;

    public static bool CanWhiteShortCastle(ulong blackAttackedSquares)
    {
        ulong whiteBitboard = BoardUtils.GetWhiteBitboard();
        ulong blackBitboard = BoardUtils.GetBlackBitboard();
        
        ulong notOccupiedSquares = 96;

        bool squaresNotBlocked = ((blackBitboard | blackAttackedSquares | whiteBitboard) & notOccupiedSquares) == 0ul;
        
        if (!HasMovedWhiteKing & squaresNotBlocked & !HasMovedRightWhiteRook & !Board.InCheck(true, blackAttackedSquares) & FenWhiteCastleKingSide)
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
        
        if (!HasMovedWhiteKing & squaresNotBlocked & !HasMovedLeftWhiteRook & !Board.InCheck(true, blackAttackedSquares) & FenWhiteCastleQueenSide)
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
        
        if (!HasMovedBlackKing & squaresNotBlocked & !HasMovedRightBlackRook & !Board.InCheck(false, whiteAttackedSquares) & FenBlackCastleKingSide)
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
        
        if (!HasMovedBlackKing & squaresNotBlocked & !HasMovedLeftBlackRook & !Board.InCheck(false, whiteAttackedSquares) & FenBlackCastleQueenSide)
        {   
            return true;
        }

        return false;

    }
}