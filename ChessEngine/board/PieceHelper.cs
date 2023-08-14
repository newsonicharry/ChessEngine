using ChessEngine.bitboard;

namespace ChessEngine.board;

public class PieceHelper
{
    public static readonly int[] PieceArray = new int[64];
    
    
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