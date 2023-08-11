using ChessEngine.bitboard;
using ChessEngine.Board;
using static ChessEngine.Engine.PieceSquareTables;

namespace ChessEngine.Engine;

public abstract class Engine
{
    private const int PawnValue = 100;
    private const int KnightValue = 300;
    private const int BishopValue = 320;
    private const int RookValue = 500;
    private const int QueenValue = 900;
    private const int KingValue = 100000;
    
    
    
    public static ushort FindBestMove(int depth)
    {
        ushort[] validMoves = ValidMoves.FindValidMoves(); // Assuming FindValidMoves takes a Board parameter
        int bestScore = int.MinValue;
        ushort bestMove = 0; // Placeholder for the best move

        foreach (ushort move in validMoves)
        {
            board.Board.UpdateBoard(move);
            int score = -Search(depth - 1);
            board.Board.UndoMove();
            
            
            if (score > bestScore)
            {
                bestScore = score;
                bestMove = move;
            }
        }

        return bestMove;
    }

    public static int Search(int depth)
    {
        if (depth == 0)
        {
            return EvaluatePosition();
        }

        ushort[] validMoves = ValidMoves.FindValidMoves(); // Assuming FindValidMoves takes a Board parameter
        int bestScore = int.MinValue;

        foreach (ushort move in validMoves)
        {
            board.Board.UpdateBoard(move);
            int score = -Search(depth - 1);
            board.Board.UndoMove();

            if (score > bestScore)
            {
                bestScore = score;
            }
        }

        return bestScore;
    }
    
    public static int EvaluatePosition()
    {

        
        return GetPieceSquareTablesEvalBlack() - GetPieceSquareTablesEvalWhite(); // Invert the material calculation
    }


    private static int GetPieceSquareTablesEvalWhite()
    {
        int whitePositionEval = 0;

        int[] pawnIndexes = BitboardUtils.GetSetBitIndexes(Bitboards.WhitePawnBitboard);
        int[] knightIndexes = BitboardUtils.GetSetBitIndexes(Bitboards.WhiteKnightBitboard);
        int[] bishopIndexes = BitboardUtils.GetSetBitIndexes(Bitboards.WhiteBishopBitboard);
        int[] rookIndexes = BitboardUtils.GetSetBitIndexes(Bitboards.WhiteRookBitboard);
        int[] queenIndexes = BitboardUtils.GetSetBitIndexes(Bitboards.WhiteQueenBitboard);
        int[] kingIndexes = BitboardUtils.GetSetBitIndexes(Bitboards.WhiteKingBitboard);

        
        for (int i = 0; i < pawnIndexes.Length; i++)
        {
            int pawnIndex = pawnIndexes[i];
            whitePositionEval += (PawnEvalWhite[pawnIndex] + 100);
        }
        
        for (int i = 0; i < knightIndexes.Length; i++)
        {
            int knightIndex = knightIndexes[i];
            whitePositionEval += (KnightEval[knightIndex] + 300);
        }
        
        for (int i = 0; i < bishopIndexes.Length; i++)
        {
            int bishopIndex = bishopIndexes[i];
            whitePositionEval += (BishopEvalWhite[bishopIndex] + 320);
        }
        
        for (int i = 0; i < rookIndexes.Length; i++)
        {
            int rookIndex = rookIndexes[i];
            whitePositionEval += (RookEvalWhite[rookIndex] + 500);
        }
        
        for (int i = 0; i < queenIndexes.Length; i++)
        {
            int queenIndex = queenIndexes[i];
            whitePositionEval += (QueenEval[queenIndex] + 900);
        }
        
        for (int i = 0; i < kingIndexes.Length; i++)
        {
            int kingIndex = kingIndexes[i];
            whitePositionEval += (KingEvalWhite[kingIndex] + 900);
        }

        return whitePositionEval;
    }
    
    private static int GetPieceSquareTablesEvalBlack()
    {
        int blackPositionEval = 0;

        int[] pawnIndexes = BitboardUtils.GetSetBitIndexes(Bitboards.BlackPawnBitboard);
        int[] knightIndexes = BitboardUtils.GetSetBitIndexes(Bitboards.BlackKnightBitboard);
        int[] bishopIndexes = BitboardUtils.GetSetBitIndexes(Bitboards.BlackBishopBitboard);
        int[] rookIndexes = BitboardUtils.GetSetBitIndexes(Bitboards.BlackRookBitboard);
        int[] queenIndexes = BitboardUtils.GetSetBitIndexes(Bitboards.BlackQueenBitboard);
        int[] kingIndexes = BitboardUtils.GetSetBitIndexes(Bitboards.BlackKingBitboard);

        
        for (int i = 0; i < pawnIndexes.Length; i++)
        {
            int pawnIndex = pawnIndexes[i];
            blackPositionEval += (PawnEvalBlack[pawnIndex] + 100);
        }
        
        for (int i = 0; i < knightIndexes.Length; i++)
        {
            int knightIndex = knightIndexes[i];
            blackPositionEval += (KnightEval[knightIndex] + 300);
        }
        
        for (int i = 0; i < bishopIndexes.Length; i++)
        {
            int bishopIndex = bishopIndexes[i];
            blackPositionEval += (BishopEvalBlack[bishopIndex] + 320);
        }
        
        for (int i = 0; i < rookIndexes.Length; i++)
        {
            int rookIndex = rookIndexes[i];
            blackPositionEval += (RookEvalBlack[rookIndex] + 500);
        }
        
        for (int i = 0; i < queenIndexes.Length; i++)
        {
            int queenIndex = queenIndexes[i];
            blackPositionEval += (QueenEval[queenIndex] + 900);
        }
        
        for (int i = 0; i < kingIndexes.Length; i++)
        {
            int kingIndex = kingIndexes[i];
            blackPositionEval += (KingEvalBlack[kingIndex] + 900);
        }

        return blackPositionEval;
    }
    
}


