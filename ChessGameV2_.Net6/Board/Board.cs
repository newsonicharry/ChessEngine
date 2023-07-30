using ChessGame.bitboards;

namespace ChessGame.board;


public class Board
{

    public static int[][] FindValidMoves(bool isWhite)
    {   
        
        ulong whiteBitboards = Bitboards.WhitePawnBitboard | Bitboards.WhiteKnightBitboard | Bitboards.WhiteBishopBitboard | Bitboards.WhiteRookBitboard | Bitboards.WhiteQueenBitboard | Bitboards.WhiteKingBitboard;
        ulong blackBitboards = Bitboards.BlackPawnBitboard | Bitboards.BlackKnightBitboard | Bitboards.BlackBishopBitboard | Bitboards.BlackRookBitboard | Bitboards.BlackQueenBitboard | Bitboards.BlackKingBitboard;

        ulong friendlyBitboard;
        ulong enemyBitboard;
        
        if (isWhite)
        {
            friendlyBitboard = whiteBitboards;
            enemyBitboard = blackBitboards;
        }
        else
        {
            friendlyBitboard = blackBitboards;
            enemyBitboard = whiteBitboards;
        }


        ulong GetBlockers(ulong movementMask)
        {   
            // BitboardUtils.PrintBitboards(movementMask & (enemyBitboard | friendlyBitboard));

            return movementMask & (enemyBitboard | friendlyBitboard);
        }
        
        bool PieceOnIndex(ulong bitboard, int i)
        {
            return BitboardUtils.isBitOn(bitboard, (i));
        }

        ulong MovesNotObstructed(ulong movementMask)
        {
            return movementMask & ~friendlyBitboard;
        }

        List<int[]> allValidMoves = new List<int[]>();

        for (int i = 0; i < 64; i++)
        {
            if (PieceOnIndex(Bitboards.WhiteKnightBitboard, i))
            {
                ulong validMoves = MovesNotObstructed(MovementMasks.KnightMovementMasks[i]);
                foreach (int validIndex in BitboardUtils.GetSetBitIndexes(validMoves))
                {
                    allValidMoves.Add(new [] {i, validIndex});
                }
            }
            
            if (PieceOnIndex(Bitboards.WhitePawnBitboard, i))
            {
                ulong validMoves = MovesNotObstructed(MovementMasks.PawnWhiteMoveMovementMasks[i]) & ~enemyBitboard;
                
                ulong validAttacks = MovementMasks.PawnWhiteAttackMovementMasks[i] & enemyBitboard;
                
                foreach (int validIndex in BitboardUtils.GetSetBitIndexes(validMoves) )
                {
                    allValidMoves.Add(new [] {i, validIndex});
                }
                
                foreach (int validIndex in BitboardUtils.GetSetBitIndexes(validAttacks) )
                {
                    allValidMoves.Add(new [] {i, validIndex});
                }
                
            }
            
            if (PieceOnIndex(Bitboards.WhiteKingBitboard, i))
            {
                ulong validMoves = MovesNotObstructed(MovementMasks.KingMovementMasks[i]);
                foreach (int validIndex in BitboardUtils.GetSetBitIndexes(validMoves))
                {
                    allValidMoves.Add(new [] {i, validIndex});
                }
            }

            if (PieceOnIndex(Bitboards.WhiteRookBitboard, i) || PieceOnIndex(Bitboards.WhiteQueenBitboard, i))
            {
                ulong rookMovementMask = MovementMasks.RookMovementMasksNoEdges[i];

                ulong blockers = GetBlockers(rookMovementMask);
                ulong key = (blockers * PrecomputedMagics.RookMagics[i]) >> PrecomputedMagics.RookShifts[i];
                ulong validMoves = MovementMasks.RookMovesLookUp[i][key];
                validMoves = MovesNotObstructed(validMoves);
                
                foreach (int validIndex in BitboardUtils.GetSetBitIndexes(validMoves))
                { 
                    allValidMoves.Add(new []{i, validIndex});
                }
                
            }
            
            if (PieceOnIndex(Bitboards.WhiteBishopBitboard, i) || PieceOnIndex(Bitboards.WhiteQueenBitboard, i))
            {
                ulong bishopMovementMask = MovementMasks.BishopMovementMasksNoEdges[i];
                
                ulong blockers = GetBlockers(bishopMovementMask);
                ulong key = (blockers * PrecomputedMagics.BishopMagics[i]) >> PrecomputedMagics.BishopShifts[i];
                ulong validMoves = MovementMasks.BishopMovesLookUp[i][key];

                validMoves = MovesNotObstructed(validMoves);
                
                foreach (int validIndex in BitboardUtils.GetSetBitIndexes(validMoves))
                { 
                    allValidMoves.Add(new []{i, validIndex});
                }
                
            }
            
            
            
        }

        return allValidMoves.ToArray();

    }
    
    public static void UpdateBitboards(ulong pieceBitboard, int originalIndex, int newIndex)
    {   


        bool NeedsToDeleteBit(ulong bitboard) 
        {
            return BitboardUtils.isBitOn(bitboard, (newIndex));
        }
        
        ulong DeleteBit(ulong bitboard)
        {
            return BitboardUtils.negateBit(bitboard, (newIndex));
        }
        
        
        if (NeedsToDeleteBit(Bitboards.WhitePawnBitboard)) { Bitboards.WhitePawnBitboard = DeleteBit(Bitboards.WhitePawnBitboard); }
        if (NeedsToDeleteBit(Bitboards.WhiteKnightBitboard)) { Bitboards.WhiteKnightBitboard = DeleteBit(Bitboards.WhiteKnightBitboard); }
        if (NeedsToDeleteBit(Bitboards.WhiteBishopBitboard)) { Bitboards.WhiteBishopBitboard = DeleteBit(Bitboards.WhiteBishopBitboard); }
        if (NeedsToDeleteBit(Bitboards.WhiteRookBitboard)) { Bitboards.WhiteRookBitboard = DeleteBit(Bitboards.WhiteRookBitboard); }
        if (NeedsToDeleteBit(Bitboards.WhiteQueenBitboard)) { Bitboards.WhiteQueenBitboard = DeleteBit(Bitboards.WhiteQueenBitboard); }
        if (NeedsToDeleteBit(Bitboards.WhiteKingBitboard)) { Bitboards.WhiteKingBitboard = DeleteBit(Bitboards.WhiteKingBitboard); }
        
        if (NeedsToDeleteBit(Bitboards.BlackPawnBitboard)) { Bitboards.BlackPawnBitboard = DeleteBit(Bitboards.BlackPawnBitboard); }
        if (NeedsToDeleteBit(Bitboards.BlackKnightBitboard)) { Bitboards.BlackKnightBitboard = DeleteBit(Bitboards.BlackKnightBitboard); }
        if (NeedsToDeleteBit(Bitboards.BlackBishopBitboard)) { Bitboards.BlackBishopBitboard = DeleteBit(Bitboards.BlackBishopBitboard); }
        if (NeedsToDeleteBit(Bitboards.BlackRookBitboard)) { Bitboards.BlackRookBitboard = DeleteBit(Bitboards.BlackRookBitboard); }
        if (NeedsToDeleteBit(Bitboards.BlackQueenBitboard)) { Bitboards.BlackQueenBitboard = DeleteBit(Bitboards.BlackQueenBitboard); }
        if (NeedsToDeleteBit(Bitboards.BlackKingBitboard)) { Bitboards.BlackKingBitboard = DeleteBit(Bitboards.BlackKingBitboard); }
        
        
        
        
        ulong ChangeBitPosition(ulong bitboard)
        {
            ulong newBitboard = BitboardUtils.negateBit(bitboard, (originalIndex));
            newBitboard = BitboardUtils.enableBit(newBitboard, (newIndex));
            
            return newBitboard;
        }
        
        if (pieceBitboard == Bitboards.WhitePawnBitboard) { Bitboards.WhitePawnBitboard = ChangeBitPosition(Bitboards.WhitePawnBitboard); }
        if (pieceBitboard == Bitboards.WhiteKnightBitboard) { Bitboards.WhiteKnightBitboard = ChangeBitPosition(Bitboards.WhiteKnightBitboard); }
        if (pieceBitboard == Bitboards.WhiteBishopBitboard) { Bitboards.WhiteBishopBitboard = ChangeBitPosition(Bitboards.WhiteBishopBitboard); }
        if (pieceBitboard == Bitboards.WhiteRookBitboard) { Bitboards.WhiteRookBitboard = ChangeBitPosition(Bitboards.WhiteRookBitboard); }
        if (pieceBitboard == Bitboards.WhiteQueenBitboard) { Bitboards.WhiteQueenBitboard = ChangeBitPosition(Bitboards.WhiteQueenBitboard); }
        if (pieceBitboard == Bitboards.WhiteKingBitboard) { Bitboards.WhiteKingBitboard = ChangeBitPosition(Bitboards.WhiteKingBitboard); }

        if (pieceBitboard == Bitboards.BlackPawnBitboard) { Bitboards.BlackPawnBitboard = ChangeBitPosition(Bitboards.BlackPawnBitboard); }
        if (pieceBitboard == Bitboards.BlackKnightBitboard) { Bitboards.BlackKnightBitboard = ChangeBitPosition(Bitboards.BlackKnightBitboard); }
        if (pieceBitboard == Bitboards.BlackBishopBitboard) { Bitboards.BlackBishopBitboard = ChangeBitPosition(Bitboards.BlackBishopBitboard); }
        if (pieceBitboard == Bitboards.BlackRookBitboard) { Bitboards.BlackRookBitboard = ChangeBitPosition(Bitboards.BlackRookBitboard); }
        if (pieceBitboard == Bitboards.BlackQueenBitboard) { Bitboards.BlackQueenBitboard = ChangeBitPosition(Bitboards.BlackQueenBitboard); }
        if (pieceBitboard == Bitboards.BlackKingBitboard) { Bitboards.BlackKingBitboard = ChangeBitPosition(Bitboards.BlackKingBitboard); }

        
    }
}