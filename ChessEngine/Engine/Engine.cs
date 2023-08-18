using ChessEngine.bitboard;
using ChessEngine.board;
using ChessEngine.Board;
using static ChessEngine.Engine.PieceSquareTables;

namespace ChessEngine.Engine;

public abstract class Engine
{

    public static int PositionsSearched = 0;
    
    private const int NegativeInf = -9999999;
    private const int PositiveInf = 9999999;

    private const int TotalExtensions = 4;
    
    public const int Depth = 5;

    
    public static void FindBestMove()
    {
        
        ushort[] validMoves = ValidMoves.FindValidMoves(); 
        // validMoves = OrderMoves(validMoves);
        
        int bestScore = NegativeInf;
        ushort bestMove = 0; // Placeholder for the best move
        
        
        for (int i = 0; i < validMoves.Length; i++)
        {
            
            ushort move = validMoves[i];
            
            
            board.Board.UpdateBoard(move);
            
            int score = -Search(Depth - 1, NegativeInf, PositiveInf, 0);
            
            board.Board.UndoMove();
            
            
            
            if (score > bestScore)
            {   
                bestScore = score;
                bestMove = move;
            }
        }
        
        
        Console.WriteLine("Eval: " + (double)bestScore / 100);
        Console.WriteLine("Positions Searched: " + PositionsSearched);

        board.Board.UpdateBoard(bestMove);


    }
    
    private static int Search(int depth, int alpha, int beta, int numExtensions)
    {   
        
        if (depth == 0)
        {
            int eval = EvaluatePosition();
            return eval;

        }
        
        ushort[] validMoves = ValidMoves.FindValidMoves(); 
        // validMoves = OrderMoves(validMoves);


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
            PositionsSearched++;

            int extension = 0;
            if (numExtensions < TotalExtensions)
            {
                extension = GetExtensions(move);
            }
            
            
            int score = -Search(depth - 1 + extension, -beta, -alpha, numExtensions + extension);
            board.Board.UndoMove();

            if (score >= beta)
            {
                return beta;
            }

            alpha = Math.Max(alpha, score);

        }

        return alpha;
    }


    private static ushort[] OrderMoves(ushort[] moves)
    {   
        
        List<(int moveScoreGuess, ushort move)> data = new();


        for (int i = 0; i < moves.Length; i++){
            ushort move = moves[i];
            int moveScoreGuess = 0;
            
            (int startingSquare, int endingSquare, int piece) = BoardUtils.DecodeMove(move);

            int capturedPiece = Piece.GetCapturedPiece(move);
            
            int pieceValue = Piece.PieceValues[piece];

            
            if (capturedPiece != Piece.Empty)
            {   
                int capturedPieceValue = Piece.PieceValues[capturedPiece];
                moveScoreGuess = 10 * (capturedPieceValue - pieceValue);
            }

            if (((piece == Piece.WhitePawn) & (BoardUtils.IndexToRank(endingSquare) == 7)) | ((piece == Piece.BlackPawn) & (BoardUtils.IndexToRank(endingSquare) == 0)) )
            {
                moveScoreGuess += Piece.QueenValue;
            }
            
            ulong endingSquareBitboard = 1ul << endingSquare;
            if (board.Board.IsWhite & ((endingSquareBitboard | ValidMoves.BlackPawnAttacks) == ValidMoves.BlackPawnAttacks)){
                moveScoreGuess -= pieceValue;
            }
            if (!board.Board.IsWhite & ((endingSquareBitboard | ValidMoves.WhitePawnAttacks) == ValidMoves.WhitePawnAttacks)){
                moveScoreGuess -= pieceValue;
            }

            
            data.Add((moveScoreGuess, move));
        }
        
        ushort[] sortedData = data.OrderByDescending(item => item.move).Select(item => item.move).ToArray();

        
        return sortedData;
    }
    
    
    private static int GetExtensions(ushort move)
    {
        int extension = 0;

        (int startingSquare, int endingSquare, int piece) = BoardUtils.DecodeMove(move);
        
        
        if (board.Board.IsWhite)
        {
            if (board.Board.WhiteInCheck){
                extension = 1;
            }

            if ((piece == Piece.WhitePawn) & (endingSquare == 6))
            {
                extension = 1;
            }
        }
        
        if (!board.Board.IsWhite)
        {
            if (board.Board.BlackInCheck)
            {
                extension = 1;
            }
            
            if ((piece == Piece.BlackPawn) & (endingSquare == 1))
            {
                extension = 1;
            }
        }

        return extension;
    }
    
    private static int EvaluatePosition()
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
            whitePositionEval += (PawnEvalWhite[pawnIndex] + Piece.PawnValue);
        }
        
        for (int i = 0; i < knightIndexes.Length; i++)
        {
            int knightIndex = knightIndexes[i];
            whitePositionEval += (KnightEval[knightIndex] + Piece.KnightValue);
        }
        
        for (int i = 0; i < bishopIndexes.Length; i++)
        {
            int bishopIndex = bishopIndexes[i];
            whitePositionEval += (BishopEvalWhite[bishopIndex] + Piece.BishopValue);
        }
        
        for (int i = 0; i < rookIndexes.Length; i++)
        {
            int rookIndex = rookIndexes[i];
            whitePositionEval += (RookEvalWhite[rookIndex] + Piece.RookValue);
        }
        
        for (int i = 0; i < queenIndexes.Length; i++)
        {
            int queenIndex = queenIndexes[i];
            whitePositionEval += (QueenEval[queenIndex] + Piece.QueenValue);
        }
        
        for (int i = 0; i < kingIndexes.Length; i++)
        {
            int kingIndex = kingIndexes[i];
            whitePositionEval += (KingEvalWhite[kingIndex] + Piece.KingValue);
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
            blackPositionEval += (PawnEvalBlack[pawnIndex] + Piece.PawnValue);
        }
        
        for (int i = 0; i < knightIndexes.Length; i++)
        {
            int knightIndex = knightIndexes[i];
            blackPositionEval += (KnightEval[knightIndex] + Piece.KnightValue);
        }
        
        for (int i = 0; i < bishopIndexes.Length; i++)
        {
            int bishopIndex = bishopIndexes[i];
            blackPositionEval += (BishopEvalBlack[bishopIndex] + Piece.BishopValue);
        }
        
        for (int i = 0; i < rookIndexes.Length; i++)
        {
            int rookIndex = rookIndexes[i];
            blackPositionEval += (RookEvalBlack[rookIndex] + Piece.RookValue);
        }
        
        for (int i = 0; i < queenIndexes.Length; i++)
        {
            int queenIndex = queenIndexes[i];
            blackPositionEval += (QueenEval[queenIndex] + Piece.QueenValue);
        }
        
        for (int i = 0; i < kingIndexes.Length; i++)
        {
            int kingIndex = kingIndexes[i];
            blackPositionEval += (KingEvalBlack[kingIndex] + Piece.KingValue);
        }
        
        
        return blackPositionEval;
    }
    
}


