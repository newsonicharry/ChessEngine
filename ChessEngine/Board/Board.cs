using ChessEngine.Bitboards;

namespace ChessEngine.Board;


public class Board
{
    public static bool WhiteInCheck = false;
    public static bool BlackInCheck = false;
    
    public static List<ushort> AllMovesMade = new List<ushort>();
    public static int HalfMoveClock = 0;
    public static int FullMoveClock = 0;
    public static bool GameOver = false;
    
    public static bool IsWhite = true;
    
    public static ulong EnemyAttackedSquares = 0ul;

    public static int WhiteDoubleMovedPawnIndex;
    public static int BlackDoubleMovedPawnIndex;

    
    public static bool InCheck(bool isWhite, ulong enemyAttackedSquares)
    {   
        if (isWhite) {
            return (Bitboards.Bitboards.WhiteKingBitboard | enemyAttackedSquares) == enemyAttackedSquares;
        }

        return (Bitboards.Bitboards.BlackKingBitboard & ~enemyAttackedSquares) != Bitboards.Bitboards.BlackKingBitboard;

    }

    public static void CheckForQueening()
    {
        if (IsWhite)
        {
            ulong promotablePawns = Bitboards.Bitboards.WhitePawnBitboard >> 56;
            
            if (promotablePawns != 0)
            {
                ulong promotablePawnBitboard = promotablePawns << 56;
                
                Bitboards.Bitboards.WhitePawnBitboard &= ~ promotablePawnBitboard;
                Bitboards.Bitboards.WhiteQueenBitboard |= promotablePawnBitboard;
            }
            
        }
        else
        {
            ulong promotablePawns = Bitboards.Bitboards.BlackPawnBitboard << 56;
            
            if (promotablePawns != 0)
            {
                ulong promotablePawnBitboard = promotablePawns >> 56;
                
                Bitboards.Bitboards.BlackPawnBitboard &= ~ promotablePawnBitboard;
                Bitboards.Bitboards.BlackQueenBitboard |= promotablePawnBitboard;            }
        }
    }    

    public static void SwitchCurrentPlayerTurn()
    {
        IsWhite = !IsWhite;
    }

     public static void UpdateBoard(ulong pieceBitboard, int originalIndex, int newIndex)
    {   
        
        if (BitboardUtils.isBitOn(Bitboards.Bitboards.WhitePawnBitboard, newIndex))   { Bitboards.Bitboards.WhitePawnBitboard = BitboardUtils.NegateBit(Bitboards.Bitboards.WhitePawnBitboard, newIndex); }
        if (BitboardUtils.isBitOn(Bitboards.Bitboards.WhiteKnightBitboard, newIndex)) { Bitboards.Bitboards.WhiteKnightBitboard = BitboardUtils.NegateBit(Bitboards.Bitboards.WhiteKnightBitboard, newIndex); }
        if (BitboardUtils.isBitOn(Bitboards.Bitboards.WhiteBishopBitboard, newIndex)) { Bitboards.Bitboards.WhiteBishopBitboard = BitboardUtils.NegateBit(Bitboards.Bitboards.WhiteBishopBitboard, newIndex); }
        if (BitboardUtils.isBitOn(Bitboards.Bitboards.WhiteRookBitboard, newIndex))   { Bitboards.Bitboards.WhiteRookBitboard = BitboardUtils.NegateBit(Bitboards.Bitboards.WhiteRookBitboard, newIndex); }
        if (BitboardUtils.isBitOn(Bitboards.Bitboards.WhiteQueenBitboard, newIndex))  { Bitboards.Bitboards.WhiteQueenBitboard = BitboardUtils.NegateBit(Bitboards.Bitboards.WhiteQueenBitboard, newIndex); }
        if (BitboardUtils.isBitOn(Bitboards.Bitboards.WhiteKingBitboard, newIndex))   { Bitboards.Bitboards.WhiteKingBitboard = BitboardUtils.NegateBit(Bitboards.Bitboards.WhiteKingBitboard, newIndex); }
        if (BitboardUtils.isBitOn(Bitboards.Bitboards.BlackPawnBitboard, newIndex))   { Bitboards.Bitboards.BlackPawnBitboard = BitboardUtils.NegateBit(Bitboards.Bitboards.BlackPawnBitboard, newIndex); }
        if (BitboardUtils.isBitOn(Bitboards.Bitboards.BlackKnightBitboard, newIndex)) { Bitboards.Bitboards.BlackKnightBitboard = BitboardUtils.NegateBit(Bitboards.Bitboards.BlackKnightBitboard, newIndex); }
        if (BitboardUtils.isBitOn(Bitboards.Bitboards.BlackBishopBitboard, newIndex)) { Bitboards.Bitboards.BlackBishopBitboard = BitboardUtils.NegateBit(Bitboards.Bitboards.BlackBishopBitboard, newIndex); }
        if (BitboardUtils.isBitOn(Bitboards.Bitboards.BlackRookBitboard, newIndex))   { Bitboards.Bitboards.BlackRookBitboard = BitboardUtils.NegateBit(Bitboards.Bitboards.BlackRookBitboard, newIndex); }
        if (BitboardUtils.isBitOn(Bitboards.Bitboards.BlackQueenBitboard, newIndex))  { Bitboards.Bitboards.BlackQueenBitboard = BitboardUtils.NegateBit(Bitboards.Bitboards.BlackQueenBitboard, newIndex); }
        if (BitboardUtils.isBitOn(Bitboards.Bitboards.BlackKingBitboard, newIndex))   { Bitboards.Bitboards.BlackKingBitboard = BitboardUtils.NegateBit(Bitboards.Bitboards.BlackKingBitboard, newIndex); }
        
        
        if (IsWhite)
        {
            UpdateWhiteBitboards(pieceBitboard, originalIndex, newIndex);
        }
        else
        {
            UpdateBlackBitboards(pieceBitboard, originalIndex, newIndex);
        }

        CheckForQueening();

    }

    private static void UpdateWhiteBitboards(ulong pieceBitboard, int originalIndex, int newIndex)
    {   
        // checks if the rooks are moved or are captures to enable castling
        if (originalIndex == 0 || newIndex == 0) { Castling.HasMovedLeftWhiteRook = true; }
        if (originalIndex == 7 || newIndex == 7) { Castling.HasMovedRightWhiteRook = true; }
        

        // change positions of the piece
        if (pieceBitboard == Bitboards.Bitboards.WhiteKnightBitboard) {
            Bitboards.Bitboards.WhiteKnightBitboard = BitboardUtils.ChangeBitPosition(Bitboards.Bitboards.WhiteKnightBitboard, originalIndex, newIndex);
        }

        if (pieceBitboard == Bitboards.Bitboards.WhiteBishopBitboard) {
            Bitboards.Bitboards.WhiteBishopBitboard = BitboardUtils.ChangeBitPosition(Bitboards.Bitboards.WhiteBishopBitboard, originalIndex, newIndex);
        }

        if (pieceBitboard == Bitboards.Bitboards.WhiteRookBitboard) {
            Bitboards.Bitboards.WhiteRookBitboard = BitboardUtils.ChangeBitPosition(Bitboards.Bitboards.WhiteRookBitboard, originalIndex, newIndex);
        }

        if (pieceBitboard == Bitboards.Bitboards.WhiteQueenBitboard) {
            Bitboards.Bitboards.WhiteQueenBitboard = BitboardUtils.ChangeBitPosition(Bitboards.Bitboards.WhiteQueenBitboard, originalIndex, newIndex);
        }

        if (pieceBitboard == Bitboards.Bitboards.WhitePawnBitboard) {
            Bitboards.Bitboards.WhitePawnBitboard = BitboardUtils.ChangeBitPosition(Bitboards.Bitboards.WhitePawnBitboard, originalIndex, newIndex); 
            
            if (newIndex - originalIndex == 16)
            { // checks if the pawn double moved
                WhiteDoubleMovedPawnIndex = newIndex;
            }
            
            // checks for enpassant
            if (newIndex ==  originalIndex + 8 + BlackDoubleMovedPawnIndex-originalIndex)
            {
                Bitboards.Bitboards.BlackPawnBitboard = BitboardUtils.NegateBit(Bitboards.Bitboards.BlackPawnBitboard,originalIndex + BlackDoubleMovedPawnIndex - originalIndex);
            }
            
        }
        
        if (pieceBitboard == Bitboards.Bitboards.WhiteKingBitboard)
        {
            if (newIndex == 6 & Castling.CanWhiteShortCastle(EnemyAttackedSquares, WhiteInCheck))
            {
                Bitboards.Bitboards.WhiteRookBitboard = BitboardUtils.NegateBit(Bitboards.Bitboards.WhiteRookBitboard, 7);
                Bitboards.Bitboards.WhiteRookBitboard = BitboardUtils.EnableBit(Bitboards.Bitboards.WhiteRookBitboard, 5);
                Castling.HasMovedRightWhiteRook = true;
            }
            
            if (newIndex == 2 & Castling.CanWhiteLongCastle(EnemyAttackedSquares, WhiteInCheck))
            {
                Bitboards.Bitboards.WhiteRookBitboard = BitboardUtils.NegateBit(Bitboards.Bitboards.WhiteRookBitboard, 0);
                Bitboards.Bitboards.WhiteRookBitboard = BitboardUtils.EnableBit(Bitboards.Bitboards.WhiteRookBitboard, 3);
                Castling.HasMovedLeftWhiteRook = true;
            }

            Bitboards.Bitboards.WhiteKingBitboard = BitboardUtils.ChangeBitPosition(Bitboards.Bitboards.WhiteKingBitboard, originalIndex, newIndex);
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
        if (pieceBitboard == Bitboards.Bitboards.BlackKnightBitboard) {
            Bitboards.Bitboards.BlackKnightBitboard = BitboardUtils.ChangeBitPosition(Bitboards.Bitboards.BlackKnightBitboard, originalIndex, newIndex);
        }
        
        if (pieceBitboard == Bitboards.Bitboards.BlackBishopBitboard) {
            Bitboards.Bitboards.BlackBishopBitboard = BitboardUtils.ChangeBitPosition(Bitboards.Bitboards.BlackBishopBitboard, originalIndex, newIndex);
        }

        if (pieceBitboard == Bitboards.Bitboards.BlackRookBitboard) {
            Bitboards.Bitboards.BlackRookBitboard = BitboardUtils.ChangeBitPosition(Bitboards.Bitboards.BlackRookBitboard, originalIndex, newIndex);
        }

        if (pieceBitboard == Bitboards.Bitboards.BlackQueenBitboard) {
            Bitboards.Bitboards.BlackQueenBitboard = BitboardUtils.ChangeBitPosition(Bitboards.Bitboards.BlackQueenBitboard, originalIndex, newIndex);
        }

        if (pieceBitboard == Bitboards.Bitboards.BlackPawnBitboard) {
            Bitboards.Bitboards.BlackPawnBitboard = BitboardUtils.ChangeBitPosition(Bitboards.Bitboards.BlackPawnBitboard, originalIndex, newIndex); 
            
            // checks if the pawn double moved
            if (originalIndex - newIndex == 16) {
                BlackDoubleMovedPawnIndex = newIndex;
            }
            
            if (newIndex ==  originalIndex - 8 + WhiteDoubleMovedPawnIndex-originalIndex)
            {
                Bitboards.Bitboards.WhitePawnBitboard = BitboardUtils.NegateBit(Bitboards.Bitboards.WhitePawnBitboard,originalIndex + WhiteDoubleMovedPawnIndex - originalIndex);
            }
        }


        if (pieceBitboard == Bitboards.Bitboards.BlackKingBitboard)
        {   
            // checks if the king is going to a square that it can castle on, and that it actually is allowed to castle
            if (newIndex == 62 & Castling.CanBlackShortCastle(EnemyAttackedSquares, BlackInCheck))
            {
                Bitboards.Bitboards.BlackRookBitboard = BitboardUtils.NegateBit(Bitboards.Bitboards.BlackRookBitboard, 63);
                Bitboards.Bitboards.BlackRookBitboard = BitboardUtils.EnableBit(Bitboards.Bitboards.BlackRookBitboard, 61);
                Castling.HasMovedRightBlackRook = true;
            }
            
            if (newIndex == 58 & Castling.CanBlackLongCastle(EnemyAttackedSquares, BlackInCheck))
            {
                Bitboards.Bitboards.BlackRookBitboard = BitboardUtils.NegateBit(Bitboards.Bitboards.BlackRookBitboard, 56);
                Bitboards.Bitboards.BlackRookBitboard = BitboardUtils.EnableBit(Bitboards.Bitboards.BlackRookBitboard, 59);
                Castling.HasMovedLeftBlackRook = true;
            }

            Bitboards.Bitboards.BlackKingBitboard = BitboardUtils.ChangeBitPosition(Bitboards.Bitboards.BlackKingBitboard, originalIndex, newIndex);
            Castling.HasMovedBlackKing = true;
            
        }

    }
}