using ChessGame.bitboards;
namespace ChessGame.board;


public class Board
{

    public static bool IsWhite = true;
    
    public static ulong EnemyAttackedSquares = 0ul;

    public static int WhiteDoubleMovedPawnIndex;
    public static int BlackDoubleMovedPawnIndex;

    
    public static bool InCheck(ulong enemyAttackedSquares)
    {
        if (IsWhite) {
            return (Bitboards.WhiteKingBitboard & ~enemyAttackedSquares) != Bitboards.WhiteKingBitboard;
        }

        return (Bitboards.BlackKingBitboard & ~enemyAttackedSquares) != Bitboards.BlackKingBitboard;

    }
    
    

    public static void SwitchCurrentPlayerTurn()
    {
        IsWhite = !IsWhite;
    }

    public static void UpdateBoard(ulong pieceBitboard, int originalIndex, int newIndex)
    {   
        
        if (BitboardUtils.isBitOn(Bitboards.WhitePawnBitboard, newIndex))   { Bitboards.WhitePawnBitboard = BitboardUtils.NegateBit(Bitboards.WhitePawnBitboard, newIndex); }
        if (BitboardUtils.isBitOn(Bitboards.WhiteKnightBitboard, newIndex)) { Bitboards.WhiteKnightBitboard = BitboardUtils.NegateBit(Bitboards.WhiteKnightBitboard, newIndex); }
        if (BitboardUtils.isBitOn(Bitboards.WhiteBishopBitboard, newIndex)) { Bitboards.WhiteBishopBitboard = BitboardUtils.NegateBit(Bitboards.WhiteBishopBitboard, newIndex); }
        if (BitboardUtils.isBitOn(Bitboards.WhiteRookBitboard, newIndex))   { Bitboards.WhiteRookBitboard = BitboardUtils.NegateBit(Bitboards.WhiteRookBitboard, newIndex); }
        if (BitboardUtils.isBitOn(Bitboards.WhiteQueenBitboard, newIndex))  { Bitboards.WhiteQueenBitboard = BitboardUtils.NegateBit(Bitboards.WhiteQueenBitboard, newIndex); }
        if (BitboardUtils.isBitOn(Bitboards.WhiteKingBitboard, newIndex))   { Bitboards.WhiteKingBitboard = BitboardUtils.NegateBit(Bitboards.WhiteKingBitboard, newIndex); }
        if (BitboardUtils.isBitOn(Bitboards.BlackPawnBitboard, newIndex))   { Bitboards.BlackPawnBitboard = BitboardUtils.NegateBit(Bitboards.BlackPawnBitboard, newIndex); }
        if (BitboardUtils.isBitOn(Bitboards.BlackKnightBitboard, newIndex)) { Bitboards.BlackKnightBitboard = BitboardUtils.NegateBit(Bitboards.BlackKnightBitboard, newIndex); }
        if (BitboardUtils.isBitOn(Bitboards.BlackBishopBitboard, newIndex)) { Bitboards.BlackBishopBitboard = BitboardUtils.NegateBit(Bitboards.BlackBishopBitboard, newIndex); }
        if (BitboardUtils.isBitOn(Bitboards.BlackRookBitboard, newIndex))   { Bitboards.BlackRookBitboard = BitboardUtils.NegateBit(Bitboards.BlackRookBitboard, newIndex); }
        if (BitboardUtils.isBitOn(Bitboards.BlackQueenBitboard, newIndex))  { Bitboards.BlackQueenBitboard = BitboardUtils.NegateBit(Bitboards.BlackQueenBitboard, newIndex); }
        if (BitboardUtils.isBitOn(Bitboards.BlackKingBitboard, newIndex))   { Bitboards.BlackKingBitboard = BitboardUtils.NegateBit(Bitboards.BlackKingBitboard, newIndex); }
        
        
        if (IsWhite)
        {
            UpdateWhiteBitboards(pieceBitboard, originalIndex, newIndex);
        }
        else
        {
            UpdateBlackBitboards(pieceBitboard, originalIndex, newIndex);
        }
    }

    private static void UpdateWhiteBitboards(ulong pieceBitboard, int originalIndex, int newIndex)
    {   
        // checks if the rooks are moved or are captures to enable castling
        if (originalIndex == 0 || newIndex == 0) { Castling.HasMovedLeftWhiteRook = true; }
        if (originalIndex == 7 || newIndex == 7) { Castling.HasMovedRightWhiteRook = true; }
        

        // change positions of the piece
        if (pieceBitboard == Bitboards.WhiteKnightBitboard) {
            Bitboards.WhiteKnightBitboard = BitboardUtils.ChangeBitPosition(Bitboards.WhiteKnightBitboard, originalIndex, newIndex);
        }

        if (pieceBitboard == Bitboards.WhiteBishopBitboard) {
            Bitboards.WhiteBishopBitboard = BitboardUtils.ChangeBitPosition(Bitboards.WhiteBishopBitboard, originalIndex, newIndex);
        }

        if (pieceBitboard == Bitboards.WhiteRookBitboard) {
            Bitboards.WhiteRookBitboard = BitboardUtils.ChangeBitPosition(Bitboards.WhiteRookBitboard, originalIndex, newIndex);
        }

        if (pieceBitboard == Bitboards.WhiteQueenBitboard) {
            Bitboards.WhiteQueenBitboard = BitboardUtils.ChangeBitPosition(Bitboards.WhiteQueenBitboard, originalIndex, newIndex);
        }

        if (pieceBitboard == Bitboards.WhitePawnBitboard) {
            Bitboards.WhitePawnBitboard = BitboardUtils.ChangeBitPosition(Bitboards.WhitePawnBitboard, originalIndex, newIndex); 
            
            if (newIndex - originalIndex == 16)
            { // checks if the pawn double moved
                WhiteDoubleMovedPawnIndex = newIndex;
            }
            
            // checks for enpassant
            if (newIndex ==  originalIndex + 8 + BlackDoubleMovedPawnIndex-originalIndex)
            {
                Bitboards.BlackPawnBitboard = BitboardUtils.NegateBit(Bitboards.BlackPawnBitboard,originalIndex + BlackDoubleMovedPawnIndex - originalIndex);
            }
            
        }
        
        if (pieceBitboard == Bitboards.WhiteKingBitboard)
        {
            if (newIndex == 6 & Castling.CanWhiteShortCastle(EnemyAttackedSquares))
            {
                Bitboards.WhiteRookBitboard = BitboardUtils.NegateBit(Bitboards.WhiteRookBitboard, 7);
                Bitboards.WhiteRookBitboard = BitboardUtils.EnableBit(Bitboards.WhiteRookBitboard, 5);
                Castling.HasMovedRightWhiteRook = true;
            }
            
            if (newIndex == 2 & Castling.CanWhiteLongCastle(EnemyAttackedSquares))
            {
                Bitboards.WhiteRookBitboard = BitboardUtils.NegateBit(Bitboards.WhiteRookBitboard, 0);
                Bitboards.WhiteRookBitboard = BitboardUtils.EnableBit(Bitboards.WhiteRookBitboard, 3);
                Castling.HasMovedLeftWhiteRook = true;
            }

            Bitboards.WhiteKingBitboard = BitboardUtils.ChangeBitPosition(Bitboards.WhiteKingBitboard, originalIndex, newIndex);
            Castling.HasMovedWhiteKing = true;
            
        }

    }

    private static void UpdateBlackBitboards(ulong pieceBitboard, int originalIndex, int newIndex)
    {   
        
        // checks if the rooks are moved or are captures to enable castling
        if (originalIndex == 56 || newIndex == 56) { Castling.HasMovedRightBlackRook = true; }
        if (originalIndex == 63 || newIndex == 63) { Castling.HasMovedLeftBlackRook = true; }

        // checks if there is already a piece on the square that the current moved piece is moving too
        // if so then delete it because the piece is capturing it

        // change positions of the piece
        if (pieceBitboard == Bitboards.BlackKnightBitboard) {
            Bitboards.BlackKnightBitboard = BitboardUtils.ChangeBitPosition(Bitboards.BlackKnightBitboard, originalIndex, newIndex);
        }
        
        if (pieceBitboard == Bitboards.BlackBishopBitboard) {
            Bitboards.BlackBishopBitboard = BitboardUtils.ChangeBitPosition(Bitboards.BlackBishopBitboard, originalIndex, newIndex);
        }

        if (pieceBitboard == Bitboards.BlackRookBitboard) {
            Bitboards.BlackRookBitboard = BitboardUtils.ChangeBitPosition(Bitboards.BlackRookBitboard, originalIndex, newIndex);
        }

        if (pieceBitboard == Bitboards.BlackQueenBitboard) {
            Bitboards.BlackQueenBitboard = BitboardUtils.ChangeBitPosition(Bitboards.BlackQueenBitboard, originalIndex, newIndex);
        }

        if (pieceBitboard == Bitboards.BlackPawnBitboard) {
            Bitboards.BlackPawnBitboard = BitboardUtils.ChangeBitPosition(Bitboards.BlackPawnBitboard, originalIndex, newIndex); 
            
            // checks if the pawn double moved
            if (originalIndex - newIndex == 16) {
                BlackDoubleMovedPawnIndex = newIndex;
            }
            
            if (newIndex ==  originalIndex - 8 + WhiteDoubleMovedPawnIndex-originalIndex)
            {
                Bitboards.WhitePawnBitboard = BitboardUtils.NegateBit(Bitboards.WhitePawnBitboard,originalIndex + WhiteDoubleMovedPawnIndex - originalIndex);
            }
        }


        if (pieceBitboard == Bitboards.BlackKingBitboard)
        {   
            // checks if the king is going to a square that it can castle on, and that it actually is allowed to castle
            if (newIndex == 62 & Castling.CanBlackShortCastle(EnemyAttackedSquares))
            {
                Bitboards.BlackRookBitboard = BitboardUtils.NegateBit(Bitboards.BlackRookBitboard, 63);
                Bitboards.BlackRookBitboard = BitboardUtils.EnableBit(Bitboards.BlackRookBitboard, 61);
                Castling.HasMovedRightBlackRook = true;
            }
            
            if (newIndex == 58 & Castling.CanBlackLongCastle(EnemyAttackedSquares))
            {
                Bitboards.BlackRookBitboard = BitboardUtils.NegateBit(Bitboards.BlackRookBitboard, 56);
                Bitboards.BlackRookBitboard = BitboardUtils.EnableBit(Bitboards.BlackRookBitboard, 59);
                Castling.HasMovedLeftBlackRook = true;
            }

            Bitboards.BlackKingBitboard = BitboardUtils.ChangeBitPosition(Bitboards.BlackKingBitboard, originalIndex, newIndex);
            Castling.HasMovedBlackKing = true;
            
        }

    }
}