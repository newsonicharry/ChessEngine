using ChessEngine.bitboard;
using ChessEngine.Board;
using static ChessEngine.Engine.PieceSquareTables;
using System.Threading;

namespace ChessEngine.Engine;

public abstract class Engine
{
    private const int PawnValue = 100;
    private const int KnightValue = 280;
    private const int BishopValue = 320;
    private const int RookValue = 479;
    private const int QueenValue = 929;
    private const int KingValue = 60000;
    
    private const int NegativeInf = -9999999;
    private const int PositiveInf = 9999999;


    private const int Depth = 5;
    
    public static void FindBestMove()
    {
        
        ushort[] validMoves = ValidMoves.FindValidMoves(); // Assuming FindValidMoves takes a Board parameter
        int bestScore = NegativeInf;
        ushort bestMove = 0; // Placeholder for the best move

        for (int i = 0; i < validMoves.Length; i++)
        {
            ushort move = validMoves[i];
            board.Board.UpdateBoard(move);

            
            int score = -Search(Depth - 1, NegativeInf, PositiveInf);
            board.Board.UndoMove();
            
            
            if (score > bestScore)
            {   
                bestScore = score;
                bestMove = move;
            }
        }
        
        board.Board.UpdateBoard(bestMove);

        board.Board.AllBitboardsMoves.Clear();
        
    }
    
    private static int Search(int depth, int alpha, int beta)
    {   
        
        if (depth == 0)
        {
            try
            {
                return Transpositions.TranspositionTable[Transpositions.ZobristHash];
            
            }
            catch (KeyNotFoundException)
            {
                int eval = EvaluatePosition();
                Transpositions.TranspositionTable.Add(new KeyValuePair<int, int>(Transpositions.ZobristHash, eval));
                
                return eval;
            
            }
        }

        ushort[] validMoves = ValidMoves.FindValidMoves(); 

        if (validMoves.Length == 0)
        {
            if (board.Board.WhiteInCheck & board.Board.IsWhite)
            {
                return NegativeInf;
            }
            if (board.Board.BlackInCheck & !board.Board.IsWhite)
            {
                return NegativeInf;
            }
        
            return 0;
        }
        

        for (int i = 0; i < validMoves.Length; i++)
        {
            ushort move = validMoves[i];
            
            board.Board.UpdateBoard(move);
            int score = -Search(depth - 1, -beta, -alpha);
            board.Board.UndoMove();

            if (score >= beta)
            {
                return beta;
            }

            alpha = Math.Max(alpha, score);

        }

        return alpha;
    }
    
    public static int EvaluatePosition()
    {
        if (board.Board.IsWhite)
        {   
            return GetPieceSquareTablesEvalWhite() - GetPieceSquareTablesEvalBlack();
        }
        return GetPieceSquareTablesEvalBlack()-GetPieceSquareTablesEvalWhite();
        
        
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
        
        
        
        if (bishopIndexes.Length > 1)
        {
            whitePositionEval += 50;
        }
        
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


        if (bishopIndexes.Length > 1)
        {
            blackPositionEval += 50;
        }
        
        for (int i = 0; i < pawnIndexes.Length; i++)
        {
            int pawnIndex = pawnIndexes[i];
            blackPositionEval += (PawnEvalBlack[pawnIndex] + PawnValue);
        }
        
        for (int i = 0; i < knightIndexes.Length; i++)
        {
            int knightIndex = knightIndexes[i];
            blackPositionEval += (KnightEval[knightIndex] + KnightValue);
        }
        
        for (int i = 0; i < bishopIndexes.Length; i++)
        {
            int bishopIndex = bishopIndexes[i];
            blackPositionEval += (BishopEvalBlack[bishopIndex] + BishopValue);
        }
        
        for (int i = 0; i < rookIndexes.Length; i++)
        {
            int rookIndex = rookIndexes[i];
            blackPositionEval += (RookEvalBlack[rookIndex] + RookValue);
        }
        
        for (int i = 0; i < queenIndexes.Length; i++)
        {
            int queenIndex = queenIndexes[i];
            blackPositionEval += (QueenEval[queenIndex] + QueenValue);
        }
        
        for (int i = 0; i < kingIndexes.Length; i++)
        {
            int kingIndex = kingIndexes[i];
            blackPositionEval += (KingEvalBlack[kingIndex] + KingValue);
        }

        return blackPositionEval;
    }
    
}


