using ChessEngine.bitboard;

namespace ChessEngine.board;

public class Piece
{   
    public const int PawnValue = 100;
    public const int KnightValue = 280;
    public const int BishopValue = 320;
    public const int RookValue = 479;
    public const int QueenValue = 929;
    public const int KingValue = 60000;
    
    public const int Empty = 0;
    
    public const int WhitePawn = 1;
    public const int WhiteKnight = 2;
    public const int WhiteBishop = 3;
    public const int WhiteRook = 4;
    public const int WhiteQueen = 5;
    public const int WhiteKing = 6;
    
    public const int BlackPawn = 7;
    public const int BlackKnight = 8;
    public const int BlackBishop = 9;
    public const int BlackRook = 10;
    public const int BlackQueen = 11;
    public const int BlackKing = 12;
    
    public static Dictionary<int, int> PieceValues = new()
    {   
        {0, 0},
        {WhitePawn, PawnValue},
        {BlackPawn, PawnValue},
        {WhiteKnight, KnightValue},
        {BlackKnight, KnightValue},
        {WhiteBishop, BishopValue},
        {BlackBishop, BishopValue},
        {WhiteRook, RookValue},
        {BlackRook, RookValue},
        {WhiteQueen, QueenValue},
        {BlackQueen, QueenValue},
        {WhiteKing, KingValue},
        {BlackKing, KingValue},
    };
    
    public static readonly int[] PieceArray = new int[64];
    
    
    
    public static int GetCapturedPiece(ushort move)
    {
        int endingSquare = (ushort)(move << 6) >> 10;
        int piece = PieceArray[endingSquare];

        return piece;
        
    }
    public static void UpdatePieceArray()
    {

        for (int i = 0; i < 64; i++){
            PieceArray[i] = 0;
        }
        
        int[] whitePawnIndexes = BitboardUtils.GetSetBitIndexes(Bitboards.WhitePawnBitboard);
        int[] whiteKnightIndexes = BitboardUtils.GetSetBitIndexes(Bitboards.WhiteKnightBitboard);
        int[] whiteBishopIndexes = BitboardUtils.GetSetBitIndexes(Bitboards.WhiteBishopBitboard);
        int[] whiteRookIndexes = BitboardUtils.GetSetBitIndexes(Bitboards.WhiteRookBitboard);
        int[] whiteQueenIndexes = BitboardUtils.GetSetBitIndexes(Bitboards.WhiteQueenBitboard);
        int[] whiteKingIndexes = BitboardUtils.GetSetBitIndexes(Bitboards.WhiteKingBitboard);
        
        int[] blackPawnIndexes = BitboardUtils.GetSetBitIndexes(Bitboards.BlackPawnBitboard);
        int[] blackKnightIndexes = BitboardUtils.GetSetBitIndexes(Bitboards.BlackKnightBitboard);
        int[] blackBishopIndexes = BitboardUtils.GetSetBitIndexes(Bitboards.BlackBishopBitboard);
        int[] blackRookIndexes = BitboardUtils.GetSetBitIndexes(Bitboards.BlackRookBitboard);
        int[] blackQueenIndexes = BitboardUtils.GetSetBitIndexes(Bitboards.BlackQueenBitboard);
        int[] blackKingIndexes = BitboardUtils.GetSetBitIndexes(Bitboards.BlackKingBitboard);

        
        for (int i = 0; i < whitePawnIndexes.Length; i++){
            int whitePawnIndex = whitePawnIndexes[i];
            PieceArray[whitePawnIndex] = 1;
        }
        
        for (int i = 0; i < whiteKnightIndexes.Length; i++){
            int whiteKnightIndex = whiteKnightIndexes[i];
            PieceArray[whiteKnightIndex] = 2;
        }
        
        for (int i = 0; i < whiteBishopIndexes.Length; i++){
            int whiteBishopIndex = whiteBishopIndexes[i];
            PieceArray[whiteBishopIndex] = 3;
        }
        
        for (int i = 0; i < whiteRookIndexes.Length; i++){
            int whiteRookIndex = whiteRookIndexes[i];
            PieceArray[whiteRookIndex] = 4;
        }
        
        for (int i = 0; i < whiteQueenIndexes.Length; i++){
            int whiteQueenIndex = whiteQueenIndexes[i];
            PieceArray[whiteQueenIndex] = 5;
        }
        
        for (int i = 0; i < whiteKingIndexes.Length; i++){
            int whiteKingIndex = whiteKingIndexes[i];
            PieceArray[whiteKingIndex] = 6;
        }
        
        
        for (int i = 0; i < blackPawnIndexes.Length; i++){
            int blackPawnIndex = blackPawnIndexes[i];
            PieceArray[blackPawnIndex] = 7;
        }
        
        for (int i = 0; i < blackKnightIndexes.Length; i++){
            int blackKnightIndex = blackKnightIndexes[i];
            PieceArray[blackKnightIndex] = 8;
        }
        
        for (int i = 0; i < blackBishopIndexes.Length; i++){
            int blackBishopIndex = blackBishopIndexes[i];
            PieceArray[blackBishopIndex] = 9;
        }
        
        for (int i = 0; i < blackRookIndexes.Length; i++){
            int blackRookIndex = blackRookIndexes[i];
            PieceArray[blackRookIndex] = 10;
        }
        
        for (int i = 0; i < blackQueenIndexes.Length; i++){
            int whiteQueenIndex = blackQueenIndexes[i];
            PieceArray[whiteQueenIndex] = 11;
        }
        
        for (int i = 0; i < blackKingIndexes.Length; i++){
            int whiteKingIndex = blackKingIndexes[i];
            PieceArray[whiteKingIndex] = 12;
        }

        
    }
}