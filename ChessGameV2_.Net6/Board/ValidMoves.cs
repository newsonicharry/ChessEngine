using ChessGame.bitboards;


namespace ChessGame.board;

public class ValidMoves
{

    public static ulong GetEnemyAttackSquares()
    {
        ulong attackedSquares = 0ul;
        
        for (int pieceIndex = 0; pieceIndex < 64; pieceIndex++)
        {
            if (Board.IsWhite)
            {
                attackedSquares |= FindBlackValidMoves(pieceIndex, 0ul);
            }
            else
            {
                attackedSquares |= FindWhiteValidMoves(pieceIndex, 0ul);
            }
        }

        return attackedSquares;
    }

    private static ulong FindWhiteValidMoves(int pieceIndex, ulong enemyAttackedSquares)
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
            validMoves &= ~enemyAttackedSquares;
            
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

    private static ulong FindBlackValidMoves(int pieceIndex, ulong enemyAttackedSquares)
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
            validMoves &= ~enemyAttackedSquares;
            
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

        Board.EnemyAttackedSquares = GetEnemyAttackSquares();
        
        for (int index = 0; index < 64; index++)
        {
            if (Board.IsWhite)
            {
                allValidMoves[index] = FindWhiteValidMoves(index, Board.EnemyAttackedSquares);
            }
            else
            {
                allValidMoves[index] = FindBlackValidMoves(index, Board.EnemyAttackedSquares);
            }
        }

        if (Board.IsWhite)
        {
            if (Board.CanWhiteShortCastle(Board.EnemyAttackedSquares))
            {
                allValidMoves[4] |= 64ul;
            }
            
            if (Board.CanWhiteLongCastle(Board.EnemyAttackedSquares))
            {
                allValidMoves[4] |= 4ul;
            }
        }
        else
        {
            if (Board.CanBlackShortCastle(Board.EnemyAttackedSquares))
            {
                allValidMoves[60] |= 4611686018427387904ul;
            }

            if (Board.CanBlackLongCastle(Board.EnemyAttackedSquares))
            {
                allValidMoves[60] |= 288230376151711744;
            }
        }
        
        
        return allValidMoves;

    }
}