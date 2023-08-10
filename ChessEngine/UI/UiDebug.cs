using ChessEngine.bitboard;
using ChessEngine.Board;
using Raylib_cs;

namespace ChessEngine.UI;

public class UiDebug
{   
    private static int CheckIfMouesOnSquare()
    {
        int i = 0;
        foreach (int[] posData in UiElements.SquarePositions)
        {
            int xPos = posData[0];
            int yPos = posData[1];
                
            if (Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), new Rectangle(xPos, yPos, UIConstants.SideLength, UIConstants.SideLength)))
            {
                return i;
            }

            i++;
        }

        return -1;
    }

    private static void DeletePieceFromBitboard(int squareSelected)
    {   
        
        ulong bitboardIndex = 1ul << squareSelected;

        bool BitboardContainsPiece(ulong bitboard)
        {
            return (bitboard | bitboardIndex) == bitboard;
        }
        
        if (BitboardContainsPiece(Bitboards.WhitePawnBitboard)) { Bitboards.WhitePawnBitboard &= ~bitboardIndex; }
        if (BitboardContainsPiece(Bitboards.WhiteKnightBitboard)) { Bitboards.WhiteKnightBitboard &= ~bitboardIndex; }
        if (BitboardContainsPiece(Bitboards.WhiteBishopBitboard)) { Bitboards.WhiteBishopBitboard &= ~bitboardIndex; }
        if (BitboardContainsPiece(Bitboards.WhiteRookBitboard)) { Bitboards.WhiteRookBitboard &= ~bitboardIndex; }
        if (BitboardContainsPiece(Bitboards.WhiteQueenBitboard)) { Bitboards.WhiteQueenBitboard &= ~bitboardIndex; }
        if (BitboardContainsPiece(Bitboards.WhiteKingBitboard)) { Bitboards.WhiteKingBitboard &= ~bitboardIndex; }

        if (BitboardContainsPiece(Bitboards.BlackPawnBitboard)) { Bitboards.BlackPawnBitboard &= ~bitboardIndex; }
        if (BitboardContainsPiece(Bitboards.BlackKnightBitboard)) { Bitboards.BlackKnightBitboard &= ~bitboardIndex; }
        if (BitboardContainsPiece(Bitboards.BlackBishopBitboard)) { Bitboards.BlackBishopBitboard &= ~bitboardIndex; }
        if (BitboardContainsPiece(Bitboards.BlackRookBitboard)) { Bitboards.BlackRookBitboard &= ~bitboardIndex; }
        if (BitboardContainsPiece(Bitboards.BlackQueenBitboard)) { Bitboards.BlackQueenBitboard &= ~bitboardIndex; }
        if (BitboardContainsPiece(Bitboards.BlackKingBitboard)) { Bitboards.BlackKingBitboard &= ~bitboardIndex; }

    }

    private static void AddWhitePieceToBitboard(int pieceType, int squareSelected)
    {
        ulong bitboardIndex = 1ul << squareSelected;
        
        if (pieceType == 49) { Bitboards.WhitePawnBitboard |= bitboardIndex; }
        if (pieceType == 50) { Bitboards.WhiteKnightBitboard |= bitboardIndex; }
        if (pieceType == 51) { Bitboards.WhiteBishopBitboard |= bitboardIndex; }
        if (pieceType == 52) { Bitboards.WhiteRookBitboard |= bitboardIndex; }
        if (pieceType == 53) { Bitboards.WhiteQueenBitboard |= bitboardIndex; }
        if (pieceType == 54) { Bitboards.WhiteKingBitboard |= bitboardIndex; }
        
    }
    
    private static void AddBlackPieceToBitboard(int pieceType, int squareSelected)
    {
        ulong bitboardIndex = 1ul << squareSelected;
        
        if (pieceType == 49) { Bitboards.BlackPawnBitboard |= bitboardIndex; }
        if (pieceType == 50) { Bitboards.BlackKnightBitboard |= bitboardIndex; }
        if (pieceType == 51) { Bitboards.BlackBishopBitboard |= bitboardIndex; }
        if (pieceType == 52) { Bitboards.BlackRookBitboard |= bitboardIndex; }
        if (pieceType == 53) { Bitboards.BlackQueenBitboard |= bitboardIndex; }
        if (pieceType == 54) { Bitboards.BlackKingBitboard |= bitboardIndex; }
        
    }
    
    public static void AddPieceToBoard()
    {   
        bool drawWhitePieces = !Raylib.IsKeyDown(KeyboardKey.KEY_LEFT_SHIFT);
        
        int keyPressed = Raylib.GetKeyPressed();
        
        bool deletePiece = keyPressed == 259;
        
        

        if ((keyPressed == 49 | keyPressed == 50 | keyPressed == 51 | keyPressed == 52 | keyPressed == 53 | keyPressed == 54 ) | deletePiece)
        {
            int currentSquareSelected = CheckIfMouesOnSquare();

            if (currentSquareSelected != -1)
            {
                DeletePieceFromBitboard(currentSquareSelected);
            }
            
            if (!deletePiece)
            {
                if (drawWhitePieces)
                {
                    AddWhitePieceToBitboard(keyPressed, currentSquareSelected);
                }
                else
                {
                    AddBlackPieceToBitboard(keyPressed, currentSquareSelected);
                }

            }
            UiElements.ValidMoves = ValidMoves.FindValidMoves();
        }
        
        
    }
}