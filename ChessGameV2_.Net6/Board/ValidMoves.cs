using ChessGame.bitboards;


namespace ChessGame.board;

public class ValidMoves
{
    public static ulong FindWhiteValidMoves(int pieceIndex)
    {

        ulong friendlyBitboard = BoardUtils.GetWhiteBitboard();
        ulong enemyBitboard = BoardUtils.GetBlackBitboard();
        

        ulong MovesNotObstructed(ulong movementMask)
        {
            return movementMask & ~friendlyBitboard;
        }
        

        if (BitboardUtils.isBitOn(Bitboards.WhiteKnightBitboard, pieceIndex))
        {   
            ulong validMoves = MovesNotObstructed(MovementMasks.KnightMovementMasks[pieceIndex]);
            
            return validMoves;
        }
            
            
        if (BitboardUtils.isBitOn(Bitboards.WhitePawnBitboard, pieceIndex))
        {
            ulong validMoves = MovementMasks.PawnWhiteMoveMovementMasks[pieceIndex] & ~(friendlyBitboard | enemyBitboard) << 8;
            validMoves |= 1ul << pieceIndex+8;
            validMoves &= ~(friendlyBitboard | enemyBitboard);

            ulong validAttacks = MovementMasks.PawnWhiteAttackMovementMasks[pieceIndex] & enemyBitboard;

            return validMoves | validAttacks;
        }
            
        if (BitboardUtils.isBitOn(Bitboards.WhiteKingBitboard, pieceIndex))
        {
            ulong validMoves = MovesNotObstructed(MovementMasks.KingMovementMasks[pieceIndex]);

            return validMoves;
        }
        

        ulong queenValidMoves = 0ul;
        bool isQueen = BitboardUtils.isBitOn(Bitboards.WhiteQueenBitboard, pieceIndex);        
        
        if (BitboardUtils.isBitOn(Bitboards.WhiteRookBitboard, pieceIndex) || isQueen)
        {
            ulong rookMovementMask = MovementMasks.RookMovementMasksNoEdges[pieceIndex];

            ulong blockers = rookMovementMask & (enemyBitboard | friendlyBitboard);
            ulong key = (blockers * PrecomputedMagics.RookMagics[pieceIndex]) >> PrecomputedMagics.RookShifts[pieceIndex];
            ulong validMoves = MovementMasks.RookMovesLookUp[pieceIndex][key];
            validMoves = MovesNotObstructed(validMoves);

            if (isQueen)
            {
                queenValidMoves |= validMoves;
            }
            else
            {
                return validMoves;
            }
        }
        
        if (BitboardUtils.isBitOn(Bitboards.WhiteBishopBitboard, pieceIndex) || isQueen)
        {
            ulong bishopMovementMask = MovementMasks.BishopMovementMasksNoEdges[pieceIndex];
            
            ulong blockers = bishopMovementMask & (enemyBitboard | friendlyBitboard);
            ulong key = (blockers * PrecomputedMagics.BishopMagics[pieceIndex]) >> PrecomputedMagics.BishopShifts[pieceIndex];
            ulong validMoves = MovementMasks.BishopMovesLookUp[pieceIndex][key];

            validMoves = MovesNotObstructed(validMoves);

            if (isQueen)
            {
                return queenValidMoves | validMoves;
            }

            return validMoves;

        }

        return 0ul;
    }


    public static ulong FindBlackValidMoves(int pieceIndex)
    {
        ulong enemyBitboard = BoardUtils.GetWhiteBitboard();
        ulong friendlyBitboard = BoardUtils.GetBlackBitboard();
        
        
        ulong MovesNotObstructed(ulong movementMask)
        {
            return movementMask & ~friendlyBitboard;
        }


        if (BitboardUtils.isBitOn(Bitboards.BlackKnightBitboard, pieceIndex))
        {   
            ulong validMoves = MovesNotObstructed(MovementMasks.KnightMovementMasks[pieceIndex]);

            return validMoves;
        }
    
    
        if (BitboardUtils.isBitOn(Bitboards.BlackPawnBitboard, pieceIndex))
        {
            ulong validMoves = MovementMasks.PawnBlackMoveMovementMasks[pieceIndex] & ~(friendlyBitboard | enemyBitboard) >> 8;
            
            validMoves |= 1ul << pieceIndex-8;
            validMoves &= ~(friendlyBitboard | enemyBitboard);

            ulong validAttacks = MovementMasks.PawnBlackAttackMovementMasks[pieceIndex] & enemyBitboard;

            return validMoves | validAttacks;
        }
        
        if (BitboardUtils.isBitOn(Bitboards.BlackKingBitboard, pieceIndex))
        {
            ulong validMoves = MovesNotObstructed(MovementMasks.KingMovementMasks[pieceIndex]);
            
            return validMoves;
        }

        ulong queenValidMoves = 0ul;
        bool isQueen = BitboardUtils.isBitOn(Bitboards.BlackQueenBitboard, pieceIndex);
        
        if (BitboardUtils.isBitOn(Bitboards.BlackRookBitboard, pieceIndex) || isQueen)
        {
            ulong rookMovementMask = MovementMasks.RookMovementMasksNoEdges[pieceIndex];

            ulong blockers = rookMovementMask & (enemyBitboard | friendlyBitboard);;
            ulong key = (blockers * PrecomputedMagics.RookMagics[pieceIndex]) >> PrecomputedMagics.RookShifts[pieceIndex];
            ulong validMoves = MovementMasks.RookMovesLookUp[pieceIndex][key];
            validMoves = MovesNotObstructed(validMoves);

            if (isQueen)
            {
                queenValidMoves |= validMoves;
            }
            else
            {
                return validMoves;
            }
            
            
        }
        
        if (BitboardUtils.isBitOn(Bitboards.BlackBishopBitboard, pieceIndex) || isQueen)
        {
            ulong bishopMovementMask = MovementMasks.BishopMovementMasksNoEdges[pieceIndex];
            
            ulong blockers = bishopMovementMask & (enemyBitboard | friendlyBitboard);;
            ulong key = (blockers * PrecomputedMagics.BishopMagics[pieceIndex]) >> PrecomputedMagics.BishopShifts[pieceIndex];
            ulong validMoves = MovementMasks.BishopMovesLookUp[pieceIndex][key];

            validMoves = MovesNotObstructed(validMoves);
            
            if (isQueen)
            {
                return queenValidMoves | validMoves;
            }

            return validMoves;

        }
        

        return 0ul;
    }

    public static ulong[] FindValidMoves()
    {

        ulong[] allValidMoves = new ulong[64];
        
        for (int index = 0; index < 64; index++)
        {
            if (Board.IsWhite)
            {
                allValidMoves[index] = FindWhiteValidMoves(index);
            }
            else
            {
                allValidMoves[index] = FindBlackValidMoves(index);
            }
        }

        return allValidMoves;

    }
}