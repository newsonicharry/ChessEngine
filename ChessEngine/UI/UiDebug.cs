using System.Numerics;
using Raylib_cs;
using ChessEngine.Bitboards;
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
        
        if (BitboardContainsPiece(Bitboards.Bitboards.WhitePawnBitboard)) { Bitboards.Bitboards.WhitePawnBitboard &= ~bitboardIndex; }
        if (BitboardContainsPiece(Bitboards.Bitboards.WhiteKnightBitboard)) { Bitboards.Bitboards.WhiteKnightBitboard &= ~bitboardIndex; }
        if (BitboardContainsPiece(Bitboards.Bitboards.WhiteBishopBitboard)) { Bitboards.Bitboards.WhiteBishopBitboard &= ~bitboardIndex; }
        if (BitboardContainsPiece(Bitboards.Bitboards.WhiteRookBitboard)) { Bitboards.Bitboards.WhiteRookBitboard &= ~bitboardIndex; }
        if (BitboardContainsPiece(Bitboards.Bitboards.WhiteQueenBitboard)) { Bitboards.Bitboards.WhiteQueenBitboard &= ~bitboardIndex; }
        if (BitboardContainsPiece(Bitboards.Bitboards.WhiteKingBitboard)) { Bitboards.Bitboards.WhiteKingBitboard &= ~bitboardIndex; }

        if (BitboardContainsPiece(Bitboards.Bitboards.BlackPawnBitboard)) { Bitboards.Bitboards.BlackPawnBitboard &= ~bitboardIndex; }
        if (BitboardContainsPiece(Bitboards.Bitboards.BlackKnightBitboard)) { Bitboards.Bitboards.BlackKnightBitboard &= ~bitboardIndex; }
        if (BitboardContainsPiece(Bitboards.Bitboards.BlackBishopBitboard)) { Bitboards.Bitboards.BlackBishopBitboard &= ~bitboardIndex; }
        if (BitboardContainsPiece(Bitboards.Bitboards.BlackRookBitboard)) { Bitboards.Bitboards.BlackRookBitboard &= ~bitboardIndex; }
        if (BitboardContainsPiece(Bitboards.Bitboards.BlackQueenBitboard)) { Bitboards.Bitboards.BlackQueenBitboard &= ~bitboardIndex; }
        if (BitboardContainsPiece(Bitboards.Bitboards.BlackKingBitboard)) { Bitboards.Bitboards.BlackKingBitboard &= ~bitboardIndex; }

    }

    private static void AddWhitePieceToBitboard(int pieceType, int squareSelected)
    {
        ulong bitboardIndex = 1ul << squareSelected;
        
        if (pieceType == 49) { Bitboards.Bitboards.WhitePawnBitboard |= bitboardIndex; }
        if (pieceType == 50) { Bitboards.Bitboards.WhiteKnightBitboard |= bitboardIndex; }
        if (pieceType == 51) { Bitboards.Bitboards.WhiteBishopBitboard |= bitboardIndex; }
        if (pieceType == 52) { Bitboards.Bitboards.WhiteRookBitboard |= bitboardIndex; }
        if (pieceType == 53) { Bitboards.Bitboards.WhiteQueenBitboard |= bitboardIndex; }
        if (pieceType == 54) { Bitboards.Bitboards.WhiteKingBitboard |= bitboardIndex; }
        
    }
    
    private static void AddBlackPieceToBitboard(int pieceType, int squareSelected)
    {
        ulong bitboardIndex = 1ul << squareSelected;
        
        if (pieceType == 49) { Bitboards.Bitboards.BlackPawnBitboard |= bitboardIndex; }
        if (pieceType == 50) { Bitboards.Bitboards.BlackKnightBitboard |= bitboardIndex; }
        if (pieceType == 51) { Bitboards.Bitboards.BlackBishopBitboard |= bitboardIndex; }
        if (pieceType == 52) { Bitboards.Bitboards.BlackRookBitboard |= bitboardIndex; }
        if (pieceType == 53) { Bitboards.Bitboards.BlackQueenBitboard |= bitboardIndex; }
        if (pieceType == 54) { Bitboards.Bitboards.BlackKingBitboard |= bitboardIndex; }
        
    }
    
    public static void AddPieceToBoard()
    {   
        bool drawWhitePieces = !Raylib.IsKeyDown(KeyboardKey.KEY_LEFT_SHIFT);
        bool deletePiece = Raylib.IsKeyDown(KeyboardKey.KEY_DELETE);
        
        int keyPressed = Raylib.GetKeyPressed();
        
        if (keyPressed != 0 | deletePiece)
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
            UiElements.ValidMoves = Board.ValidMoves.FindValidMoves();
        }
        
        
    }
}