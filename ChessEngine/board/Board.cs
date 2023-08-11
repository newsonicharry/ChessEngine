using ChessEngine.bitboard;
using ChessEngine.Board;

namespace ChessEngine.board;


public abstract class Board
{
    public static int WhiteMaterial = 0;
    public static int BlackMaterial = 0;
    
    public static bool WhiteInCheck = false;
    public static bool BlackInCheck = false;
    
    public static int HalfMoveClock = 0;
    public static int FullMoveClock = 0;
    public static bool GameOver = false;
    
    public static bool IsWhite = true;
    
    public static ulong EnemyAttackedSquares = 0ul;

    public static int WhiteDoubleMovedPawnIndex;
    public static int BlackDoubleMovedPawnIndex;


    public static List<ulong[]> AllBitboardsMoves = new();
    
    

    public static void LoadMaterialFromFen(string fen)
    {
        string fenBoard = fen.Split(" ")[0];

        foreach (string row in fenBoard.Split("/"))
        {
            foreach (char piece in row)
            {
                if (piece == 'P') { WhiteMaterial += 100; }
                if (piece == 'N') { WhiteMaterial += 300; }
                if (piece == 'B') { WhiteMaterial += 320; }
                if (piece == 'R') { WhiteMaterial += 500; }
                if (piece == 'Q') { WhiteMaterial += 900; }
                if (piece == 'p') { BlackMaterial += 100; }
                if (piece == 'n') { BlackMaterial += 300; }
                if (piece == 'b') { BlackMaterial += 320; }
                if (piece == 'r') { BlackMaterial += 500; }
                if (piece == 'q') { BlackMaterial += 900; }


            }
            
        }
    }
    
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
        // Console.WriteLine(AllBitboardsMoves.Count);
        AllBitboardsMoves.RemoveAt(AllBitboardsMoves.Count-1);

        ulong[] oldBitboard = AllBitboardsMoves[AllBitboardsMoves.Count-1];
        
        Bitboards.WhitePawnBitboard = oldBitboard[0];
        Bitboards.WhiteKnightBitboard = oldBitboard[1];
        Bitboards.WhiteBishopBitboard = oldBitboard[2];
        Bitboards.WhiteRookBitboard = oldBitboard[3];
        Bitboards.WhiteQueenBitboard = oldBitboard[4];
        Bitboards.WhiteKingBitboard = oldBitboard[5];

        Bitboards.BlackPawnBitboard = oldBitboard[6];
        Bitboards.BlackKnightBitboard = oldBitboard[7];
        Bitboards.BlackBishopBitboard = oldBitboard[8];
        Bitboards.BlackRookBitboard = oldBitboard[9];
        Bitboards.BlackQueenBitboard = oldBitboard[10];
        Bitboards.BlackKingBitboard = oldBitboard[11];
    
        
        SwitchCurrentPlayerTurn();
        
    }
    
    public static void UpdateBoard(ushort move)
     {
         (int startingSquare, int endingSquare, int piece) = BoardUtils.DecodeMove(move);

         int pieceCapturedValue = 0;
         
         
        if (IsWhite)
        {   
            if (BitboardUtils.isBitOn(Bitboards.BlackPawnBitboard, endingSquare)){Bitboards.BlackPawnBitboard = BitboardUtils.NegateBit(Bitboards.BlackPawnBitboard, endingSquare);       pieceCapturedValue = 100;}
            if (BitboardUtils.isBitOn(Bitboards.BlackKnightBitboard, endingSquare)){Bitboards.BlackKnightBitboard = BitboardUtils.NegateBit(Bitboards.BlackKnightBitboard, endingSquare); pieceCapturedValue = 300;}
            if (BitboardUtils.isBitOn(Bitboards.BlackBishopBitboard, endingSquare)){Bitboards.BlackBishopBitboard = BitboardUtils.NegateBit(Bitboards.BlackBishopBitboard, endingSquare); pieceCapturedValue = 320;}
            if (BitboardUtils.isBitOn(Bitboards.BlackRookBitboard, endingSquare)){Bitboards.BlackRookBitboard = BitboardUtils.NegateBit(Bitboards.BlackRookBitboard, endingSquare);       pieceCapturedValue = 500;}
            if (BitboardUtils.isBitOn(Bitboards.BlackQueenBitboard, endingSquare)){Bitboards.BlackQueenBitboard = BitboardUtils.NegateBit(Bitboards.BlackQueenBitboard, endingSquare);    pieceCapturedValue = 900;}
            
            UpdateWhiteBitboards(piece, startingSquare, endingSquare);
            BlackMaterial -= pieceCapturedValue;
            
        }
        else
        {   
            if (BitboardUtils.isBitOn(Bitboards.WhitePawnBitboard, endingSquare)) {Bitboards.WhitePawnBitboard = BitboardUtils.NegateBit(Bitboards.WhitePawnBitboard, endingSquare);      pieceCapturedValue = 100;}
            if (BitboardUtils.isBitOn(Bitboards.WhiteKnightBitboard, endingSquare)){Bitboards.WhiteKnightBitboard = BitboardUtils.NegateBit(Bitboards.WhiteKnightBitboard, endingSquare); pieceCapturedValue = 300;}
            if (BitboardUtils.isBitOn(Bitboards.WhiteBishopBitboard, endingSquare)){Bitboards.WhiteBishopBitboard = BitboardUtils.NegateBit(Bitboards.WhiteBishopBitboard, endingSquare); pieceCapturedValue = 320;}
            if (BitboardUtils.isBitOn(Bitboards.WhiteRookBitboard, endingSquare)){Bitboards.WhiteRookBitboard = BitboardUtils.NegateBit(Bitboards.WhiteRookBitboard, endingSquare);       pieceCapturedValue = 500;}
            if (BitboardUtils.isBitOn(Bitboards.WhiteQueenBitboard, endingSquare)){Bitboards.WhiteQueenBitboard = BitboardUtils.NegateBit(Bitboards.WhiteQueenBitboard, endingSquare);    pieceCapturedValue = 900;}
            
            
            UpdateBlackBitboards(piece, startingSquare, endingSquare);
            WhiteMaterial -= pieceCapturedValue;
            
        }
        
        CheckForQueening();
        
        AllBitboardsMoves.Add(new[]
        {
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
        });
        
        SwitchCurrentPlayerTurn();
        
    }

    private static void UpdateWhiteBitboards(int pieceType, int originalIndex, int newIndex) 
    {   
        // checks if the rooks are moved or are captures to enable castling
        if (originalIndex == 0 || newIndex == 0) { Castling.HasMovedLeftWhiteRook = true; }
        if (originalIndex == 7 || newIndex == 7) { Castling.HasMovedRightWhiteRook = true; }
        

        // change positions of the piece
        if (pieceType == 2) {
            Bitboards.WhiteKnightBitboard = BitboardUtils.ChangeBitPosition(Bitboards.WhiteKnightBitboard, originalIndex, newIndex);
        }

        if (pieceType == 3) {
            Bitboards.WhiteBishopBitboard = BitboardUtils.ChangeBitPosition(Bitboards.WhiteBishopBitboard, originalIndex, newIndex);
        }

        if (pieceType == 4) {
            Bitboards.WhiteRookBitboard = BitboardUtils.ChangeBitPosition(Bitboards.WhiteRookBitboard, originalIndex, newIndex);
        }

        if (pieceType == 5) {
            Bitboards.WhiteQueenBitboard = BitboardUtils.ChangeBitPosition(Bitboards.WhiteQueenBitboard, originalIndex, newIndex);
        }

        if (pieceType == 1) {
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
        
        if (pieceType == 6)
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
        if (pieceType == 8) {
            Bitboards.BlackKnightBitboard = BitboardUtils.ChangeBitPosition(Bitboards.BlackKnightBitboard, originalIndex, newIndex);
        }
        
        if (pieceType == 9) {
            Bitboards.BlackBishopBitboard = BitboardUtils.ChangeBitPosition(Bitboards.BlackBishopBitboard, originalIndex, newIndex);
        }

        if (pieceType == 10) {
            Bitboards.BlackRookBitboard = BitboardUtils.ChangeBitPosition(Bitboards.BlackRookBitboard, originalIndex, newIndex);
        }

        if (pieceType == 11) {
            Bitboards.BlackQueenBitboard = BitboardUtils.ChangeBitPosition(Bitboards.BlackQueenBitboard, originalIndex, newIndex);
        }

        if (pieceType == 7) {
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


        if (pieceType == 12)
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