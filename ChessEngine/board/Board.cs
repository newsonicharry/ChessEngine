using ChessEngine.bitboard;
using ChessEngine.Board;
using ChessEngine.Engine;

namespace ChessEngine.board;


public static class Board
{
    public static bool WhiteInCheck = false;
    public static bool BlackInCheck = false;
    
    public static int HalfMoveClock = 0;
    public static int FullMoveClock = 0;
    public static readonly bool GameOver = false;
    
    public static bool IsWhite = true;
    
    public static ulong EnemyAttackedSquares = 0ul;

    public static int WhiteDoubleMovedPawnIndex;
    public static int BlackDoubleMovedPawnIndex;


    public static readonly List<(ulong[], bool[], int)> PastMoves = new();
    
    

    
    public static bool InCheck(bool isWhite, ulong enemyAttackedSquares)
    {   
        if (isWhite) {
            return (Bitboards.WhiteKingBitboard | enemyAttackedSquares) == enemyAttackedSquares;
        }

        return (Bitboards.BlackKingBitboard & ~enemyAttackedSquares) != Bitboards.BlackKingBitboard;

    }

    public static void CheckForQueening()
    {
        if (IsWhite)
        {
            ulong promotablePawns = Bitboards.WhitePawnBitboard >> 56;
            
            if (promotablePawns != 0)
            {
                ulong promotablePawnBitboard = promotablePawns << 56;
                
                Bitboards.WhitePawnBitboard &= ~ promotablePawnBitboard;
                Bitboards.WhiteQueenBitboard |= promotablePawnBitboard;
            }
            
        }
        else
        {
            ulong promotablePawns = Bitboards.BlackPawnBitboard << 56;
            
            if (promotablePawns != 0)
            {
                ulong promotablePawnBitboard = promotablePawns >> 56;
                
                Bitboards.BlackPawnBitboard &= ~ promotablePawnBitboard;
                Bitboards.BlackQueenBitboard |= promotablePawnBitboard;            }
        }
    }    

    public static void SwitchCurrentPlayerTurn()
    {
        IsWhite = !IsWhite;
    }
    
    public static void UndoMove()
    {   
        PastMoves.RemoveAt(PastMoves.Count-1);

        (ulong[], bool[], int) moveData = PastMoves[^1];
        
        
        Bitboards.WhitePawnBitboard = moveData.Item1[0];
        Bitboards.WhiteKnightBitboard = moveData.Item1[1];
        Bitboards.WhiteBishopBitboard = moveData.Item1[2];
        Bitboards.WhiteRookBitboard = moveData.Item1[3];
        Bitboards.WhiteQueenBitboard = moveData.Item1[4];
        Bitboards.WhiteKingBitboard = moveData.Item1[5];

        Bitboards.BlackPawnBitboard = moveData.Item1[6];
        Bitboards.BlackKnightBitboard = moveData.Item1[7];
        Bitboards.BlackBishopBitboard = moveData.Item1[8];
        Bitboards.BlackRookBitboard = moveData.Item1[9];
        Bitboards.BlackQueenBitboard = moveData.Item1[10];
        Bitboards.BlackKingBitboard = moveData.Item1[11];
        
        Castling.HasMovedWhiteKing = moveData.Item2[0]; 
        Castling.HasMovedBlackKing = moveData.Item2[1];
        
        Castling.HasMovedLeftWhiteRook = moveData.Item2[2]; 
        Castling.HasMovedRightWhiteRook = moveData.Item2[3];
        Castling.HasMovedLeftBlackRook = moveData.Item2[4];
        Castling.HasMovedRightBlackRook = moveData.Item2[5];

        Transpositions.ZobristHash = moveData.Item3;
            
        Piece.UpdatePieceArray();
        
        SwitchCurrentPlayerTurn();
        
    }
    
    public static void UpdateBoard(ushort move)
     {
         (int startingSquare, int endingSquare, int piece) = BoardUtils.DecodeMove(move);

         int endingSquarePiece = Piece.PieceArray[endingSquare];
         
         
        if (IsWhite)
        {
            if (endingSquarePiece != 0)
            {
                if (endingSquarePiece == 7){Bitboards.BlackPawnBitboard = BitboardUtils.NegateBit(Bitboards.BlackPawnBitboard, endingSquare);}
                if (endingSquarePiece == 8){Bitboards.BlackKnightBitboard = BitboardUtils.NegateBit(Bitboards.BlackKnightBitboard, endingSquare);}
                if (endingSquarePiece == 9){Bitboards.BlackBishopBitboard = BitboardUtils.NegateBit(Bitboards.BlackBishopBitboard, endingSquare);}
                if (endingSquarePiece == 10){Bitboards.BlackRookBitboard = BitboardUtils.NegateBit(Bitboards.BlackRookBitboard, endingSquare);}
                if (endingSquarePiece == 11){Bitboards.BlackQueenBitboard = BitboardUtils.NegateBit(Bitboards.BlackQueenBitboard, endingSquare);}   
            }
        
            
            UpdateWhiteBitboards(piece, startingSquare, endingSquare);
            
        }
        else
        {
            if (endingSquarePiece != 0)
            {
                if (endingSquarePiece == 1) {Bitboards.WhitePawnBitboard = BitboardUtils.NegateBit(Bitboards.WhitePawnBitboard, endingSquare);}
                if (endingSquarePiece == 2){Bitboards.WhiteKnightBitboard = BitboardUtils.NegateBit(Bitboards.WhiteKnightBitboard, endingSquare);}
                if (endingSquarePiece == 3) {Bitboards.WhiteBishopBitboard = BitboardUtils.NegateBit(Bitboards.WhiteBishopBitboard, endingSquare);}
                if (endingSquarePiece == 4) {Bitboards.WhiteRookBitboard = BitboardUtils.NegateBit(Bitboards.WhiteRookBitboard, endingSquare);}
                if (endingSquarePiece == 5) {Bitboards.WhiteQueenBitboard = BitboardUtils.NegateBit(Bitboards.WhiteQueenBitboard, endingSquare);}
            }
        
            
            
            UpdateBlackBitboards(piece, startingSquare, endingSquare);
            
        }
        
        CheckForQueening();
        
        Piece.UpdatePieceArray();
        Transpositions.UpdateZobristHash(piece, startingSquare, Piece.PieceArray[endingSquare], endingSquare);
        
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
            Castling.HasMovedWhiteKing,
            Castling.HasMovedBlackKing,
            Castling.HasMovedLeftWhiteRook,
            Castling.HasMovedRightWhiteRook,
            Castling.HasMovedLeftBlackRook,
            Castling.HasMovedRightBlackRook
        };
        
        PastMoves.Add((currentBitboards, currentCastling, Transpositions.ZobristHash));
        
        SwitchCurrentPlayerTurn();
        
    }

    private static void UpdateWhiteBitboards(int pieceType, int originalIndex, int newIndex) 
    {   
        // checks if the rooks are moved or are captures to enable castling
        if (originalIndex == 0 || newIndex == 0) { Castling.HasMovedLeftWhiteRook = true; }
        if (originalIndex == 7 || newIndex == 7) { Castling.HasMovedRightWhiteRook = true; }
        

        // change positions of the piece
        if (pieceType == Piece.WhiteKnight) {Bitboards.WhiteKnightBitboard = BitboardUtils.ChangeBitPosition(Bitboards.WhiteKnightBitboard, originalIndex, newIndex);}
        if (pieceType == Piece.WhiteBishop) {Bitboards.WhiteBishopBitboard = BitboardUtils.ChangeBitPosition(Bitboards.WhiteBishopBitboard, originalIndex, newIndex);}
        if (pieceType == Piece.WhiteRook)   {Bitboards.WhiteRookBitboard   = BitboardUtils.ChangeBitPosition(Bitboards.WhiteRookBitboard, originalIndex, newIndex);}
        if (pieceType == Piece.WhiteQueen)  {Bitboards.WhiteQueenBitboard  = BitboardUtils.ChangeBitPosition(Bitboards.WhiteQueenBitboard, originalIndex, newIndex);}

        if (pieceType == Piece.WhitePawn) {
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
        
        if (pieceType == Piece.WhiteKing)
        {
            if (newIndex == 6 & Castling.CanWhiteShortCastle(EnemyAttackedSquares, WhiteInCheck))
            {
                Bitboards.WhiteRookBitboard = BitboardUtils.NegateBit(Bitboards.WhiteRookBitboard, 7);
                Bitboards.WhiteRookBitboard = BitboardUtils.EnableBit(Bitboards.WhiteRookBitboard, 5);
                Castling.HasMovedRightWhiteRook = true;
            }
            
            if (newIndex == 2 & Castling.CanWhiteLongCastle(EnemyAttackedSquares, WhiteInCheck))
            {
                Bitboards.WhiteRookBitboard = BitboardUtils.NegateBit(Bitboards.WhiteRookBitboard, 0);
                Bitboards.WhiteRookBitboard = BitboardUtils.EnableBit(Bitboards.WhiteRookBitboard, 3);
                Castling.HasMovedLeftWhiteRook = true;
            }

            Bitboards.WhiteKingBitboard = BitboardUtils.ChangeBitPosition(Bitboards.WhiteKingBitboard, originalIndex, newIndex);
            Castling.HasMovedWhiteKing = true;
            
        }

    }

    private static void UpdateBlackBitboards(int pieceType, int originalIndex, int newIndex)
    {   
        
        // checks if the rooks are moved or are captures to enable castling
        if (originalIndex == 56 || newIndex == 56) { Castling.HasMovedRightBlackRook = true; }
        if (originalIndex == 63 || newIndex == 63) { Castling.HasMovedLeftBlackRook = true; }

        // checks if there is already a piece on the square that the current moved piece is moving too
        // if so then delete it because the piece is capturing it

        // change positions of the piece
        if (pieceType == Piece.BlackKnight) {Bitboards.BlackKnightBitboard = BitboardUtils.ChangeBitPosition(Bitboards.BlackKnightBitboard, originalIndex, newIndex);}
        if (pieceType == Piece.BlackBishop) {Bitboards.BlackBishopBitboard = BitboardUtils.ChangeBitPosition(Bitboards.BlackBishopBitboard, originalIndex, newIndex);}
        if (pieceType == Piece.BlackRook) {Bitboards.BlackRookBitboard = BitboardUtils.ChangeBitPosition(Bitboards.BlackRookBitboard, originalIndex, newIndex);}
        if (pieceType == Piece.BlackQueen) {Bitboards.BlackQueenBitboard = BitboardUtils.ChangeBitPosition(Bitboards.BlackQueenBitboard, originalIndex, newIndex);}

        if (pieceType == Piece.BlackPawn) {
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


        if (pieceType == Piece.BlackKing)
        {   
            // checks if the king is going to a square that it can castle on, and that it actually is allowed to castle
            if (newIndex == 62 & Castling.CanBlackShortCastle(EnemyAttackedSquares, BlackInCheck))
            {
                Bitboards.BlackRookBitboard = BitboardUtils.NegateBit(Bitboards.BlackRookBitboard, 63);
                Bitboards.BlackRookBitboard = BitboardUtils.EnableBit(Bitboards.BlackRookBitboard, 61);
                Castling.HasMovedRightBlackRook = true;
            }
            
            if (newIndex == 58 & Castling.CanBlackLongCastle(EnemyAttackedSquares, BlackInCheck))
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