using ChessGame.bitboards;


namespace ChessGame.board;

public class ValidMoves
{

    private static ulong GetRookValidMoves(int pieceIndex)
    {

        ulong friendlyBitboard;
        ulong enemyBitboard;


        if (Board.IsWhite)
        {
            friendlyBitboard = BoardUtils.GetWhiteBitboard();
            enemyBitboard = BoardUtils.GetBlackBitboard();
        }
        else
        {
            friendlyBitboard = BoardUtils.GetBlackBitboard();
            enemyBitboard = BoardUtils.GetWhiteBitboard();
        }
        
        
        ulong rookMovementMask = MovementMasks.RookMovementMasksNoEdges[pieceIndex];

        ulong blockers = rookMovementMask & (enemyBitboard | friendlyBitboard);
            
        ulong key = (blockers * PrecomputedMagics.RookMagics[pieceIndex]) >> PrecomputedMagics.RookShifts[pieceIndex];
        ulong validMoves = MovementMasks.RookMovesLookUp[pieceIndex][key];
        validMoves &= ~friendlyBitboard;


        return validMoves;
    }
    
    private static ulong GetBishopValidMoves(int pieceIndex)
    {

        ulong friendlyBitboard;
        ulong enemyBitboard;


        if (Board.IsWhite)
        {
            friendlyBitboard = BoardUtils.GetWhiteBitboard();
            enemyBitboard = BoardUtils.GetBlackBitboard();
        }
        else
        {
            friendlyBitboard = BoardUtils.GetBlackBitboard();
            enemyBitboard = BoardUtils.GetWhiteBitboard();
        }
        
        
        ulong bishopMovementMask = MovementMasks.BishopMovementMasksNoEdges[pieceIndex];
            
        ulong blockers = bishopMovementMask & (enemyBitboard | friendlyBitboard);
        ulong key = (blockers * PrecomputedMagics.BishopMagics[pieceIndex]) >> PrecomputedMagics.BishopShifts[pieceIndex];
        ulong validMoves = MovementMasks.BishopMovesLookUp[pieceIndex][key];

        validMoves &= ~friendlyBitboard;

        return validMoves;
    }
    
    public static ulong FindWhiteSlidingPieceMoves(int pieceIndex)
    {   
        
        // check for pins
        bool pinnedByBishop = false;
        bool pinnedByRook = false;
        
        (int rookPinnedPieceIndex, int enemyPinnedRookIndex, ulong enemyPinnedRookMovementMask) = Pins.FindRookPinnedPieces(pieceIndex, Bitboards.BlackRookBitboard, Bitboards.BlackQueenBitboard, Bitboards.WhiteKingBitboard);
        (int bishopPinnedPieceIndex, int enemyPinnedBishopsIndex, ulong enemyPinnedBishopMovementMask) = Pins.FindBishopPinnedPieces(pieceIndex, Bitboards.BlackBishopBitboard, Bitboards.BlackQueenBitboard, Bitboards.WhiteKingBitboard);
        
        if (rookPinnedPieceIndex != -1)   {pinnedByRook = true;}
        if (bishopPinnedPieceIndex != -1) {pinnedByBishop = true;}
        
        
        bool isQueen = BitboardUtils.isBitOn(Bitboards.WhiteQueenBitboard, pieceIndex);        
        
        // the queen is considered a rook because its diagonal moves are automatically not valid if pinned by another rook
        if (BitboardUtils.isBitOn(Bitboards.WhiteRookBitboard, pieceIndex) || (isQueen & pinnedByRook))
        {
            // if its pinned by a bishop, then the rook cannot move
            if (pinnedByBishop) { return 0ul; }
            
            // finds valid moves not accounting for being pinned by a rook
            ulong validMoves = GetRookValidMoves(pieceIndex);
            
            // if it is pinned by a rook then it can only go to squares the enemy pinned rook can go to, and also capture it
            if (pinnedByRook)
            {
                validMoves &= enemyPinnedRookMovementMask;  // shared squares
                validMoves |= 1ul << enemyPinnedRookIndex;  // capture the enemy pinned piece
            }
            
            
            return validMoves;
        }
        
        // the queen is considered a bishop because its orthogonal moves are automatically not valid if pinned by another bishop
        if (BitboardUtils.isBitOn(Bitboards.WhiteBishopBitboard, pieceIndex) || (isQueen & pinnedByBishop))
        {   
            
            // if its pinned by a rook, then the bishop cannot move
            if (pinnedByRook) { return 0ul; }
            
            // finds valid moves not accounting for being pinned by a bishop
            ulong validMoves = GetBishopValidMoves(pieceIndex);

            if (pinnedByBishop)
            {
                validMoves &= enemyPinnedBishopMovementMask;
                validMoves |= 1ul << enemyPinnedBishopsIndex;
            }
            
            return validMoves;
        }
        
        

        return 0ul;
    }
    
    private static ulong FindWhiteNonSlidingValidMoves(int pieceIndex, ulong enemyAttackedSquares)
    {   
        
        bool piecePinned = false;
        
        ulong friendlyKingBitboard = Bitboards.WhiteKingBitboard;
        ulong enemyQueenBitboard = Bitboards.BlackQueenBitboard;
        ulong enemyRookBitboard = Bitboards.BlackRookBitboard;
        ulong enemyBishopBitboard = Bitboards.BlackBishopBitboard;
        
        (int rookPinnedPieceIndex, int enemyPinnedRookIndex, ulong enemyPinnedRookMovementMask) = Pins.FindRookPinnedPieces(pieceIndex, enemyRookBitboard, enemyQueenBitboard, friendlyKingBitboard);
        (int bishopPinnedPieceIndex, int enemyPinnedBishopsIndex, ulong enemyPinnedBishopMovementMask) = Pins.FindBishopPinnedPieces(pieceIndex, enemyBishopBitboard, enemyQueenBitboard, friendlyKingBitboard);
        
        if (rookPinnedPieceIndex != -1 | bishopPinnedPieceIndex != -1)   {piecePinned = true;}
        
        
        ulong friendlyBitboard = BoardUtils.GetWhiteBitboard();
        ulong enemyBitboard = BoardUtils.GetBlackBitboard();
        

        ulong MovesNotObstructed(ulong movementMask)
        {
            return movementMask & ~friendlyBitboard;
        }
        

        if (BitboardUtils.isBitOn(Bitboards.WhiteKnightBitboard, pieceIndex))
        {
            if (piecePinned){
                return 0ul;
            }
            
            ulong validMoves = MovesNotObstructed(MovementMasks.KnightMovementMasks[pieceIndex]);
            return validMoves;
        }
            
            
        if (BitboardUtils.isBitOn(Bitboards.WhitePawnBitboard, pieceIndex))
        {   
            if (piecePinned){
                return 0ul;
            }
            
            ulong validMoves = MovementMasks.PawnWhiteMoveMovementMasks[pieceIndex] & ~(friendlyBitboard | enemyBitboard) << 8;
            validMoves |= 1ul << pieceIndex+8;
            validMoves &= ~(friendlyBitboard | enemyBitboard);

            ulong validAttacks = MovementMasks.PawnWhiteAttackMovementMasks[pieceIndex] & enemyBitboard;
            
            // en passant
            if (BoardUtils.IndexToFile(Board.BlackDoubleMovedPawnIndex) == BoardUtils.IndexToFile(pieceIndex) & (Math.Abs(Board.BlackDoubleMovedPawnIndex-pieceIndex) == 1))
            {
                validAttacks |= 1ul << pieceIndex + 8 + Board.BlackDoubleMovedPawnIndex-pieceIndex;
            }
            
            return validMoves | validAttacks;
        }
            
        if (BitboardUtils.isBitOn(Bitboards.WhiteKingBitboard, pieceIndex))
        {
            ulong validMoves = MovesNotObstructed(MovementMasks.KingMovementMasks[pieceIndex]);
            validMoves &= ~enemyAttackedSquares;
            
            return validMoves;
        }

        return 0ul;
    }
    
    private static ulong FindBlackSlidingPieceMoves(int pieceIndex)
    {   
        
        // check for pins
        bool pinnedByBishop = false;
        bool pinnedByRook = false;
        
        (int rookPinnedPieceIndex, int enemyPinnedRookIndex, ulong enemyPinnedRookMovementMask) = Pins.FindRookPinnedPieces(pieceIndex, Bitboards.WhiteRookBitboard, Bitboards.WhiteQueenBitboard, Bitboards.BlackKingBitboard);
        (int bishopPinnedPieceIndex, int enemyPinnedBishopsIndex, ulong enemyPinnedBishopMovementMask) = Pins.FindBishopPinnedPieces(pieceIndex, Bitboards.WhiteBishopBitboard, Bitboards.WhiteQueenBitboard, Bitboards.BlackKingBitboard);
        
        if (rookPinnedPieceIndex != -1)   {pinnedByRook = true;}
        if (bishopPinnedPieceIndex != -1) {pinnedByBishop = true;}
        
        
        bool isQueen = BitboardUtils.isBitOn(Bitboards.BlackQueenBitboard, pieceIndex);
        
        if (BitboardUtils.isBitOn(Bitboards.BlackRookBitboard, pieceIndex) || (isQueen & pinnedByRook))
        {   
            if (pinnedByBishop) { return 0ul; }

            ulong validMoves = GetRookValidMoves(pieceIndex);
            
            if (pinnedByRook)
            {
                validMoves &= enemyPinnedRookMovementMask;
                validMoves |= 1ul << enemyPinnedRookIndex;
            }
            
            return validMoves;
            
        }
        
        if (BitboardUtils.isBitOn(Bitboards.BlackBishopBitboard, pieceIndex) || (isQueen & pinnedByBishop))
        {   
            
            if (!isQueen & pinnedByRook)
            {
                return 0ul;
            }

            ulong validMoves = GetBishopValidMoves(pieceIndex);
            
            if (pinnedByBishop)
            {
                validMoves &= enemyPinnedBishopMovementMask;
                validMoves |= 1ul << enemyPinnedBishopsIndex;
                
                return validMoves;
            }
            return validMoves;

        }

        return 0ul;
    }
    
    private static ulong FindBlackNonSlidingValidMoves(int pieceIndex, ulong enemyAttackedSquares)
    {   
        
        // check for pins
        bool piecePinned = false;
        
        ulong friendlyKingBitboard = Bitboards.BlackKingBitboard;
        ulong enemyQueenBitboard = Bitboards.WhiteQueenBitboard;
        ulong enemyRookBitboard = Bitboards.WhiteRookBitboard;
        ulong enemyBishopBitboard = Bitboards.WhiteBishopBitboard;
        
        (int rookPinnedPieceIndex, int enemyPinnedRookIndex, ulong enemyPinnedRookMovementMask) = Pins.FindRookPinnedPieces(pieceIndex, enemyRookBitboard, enemyQueenBitboard, friendlyKingBitboard);
        (int bishopPinnedPieceIndex, int enemyPinnedBishopsIndex, ulong enemyPinnedBishopMovementMask) = Pins.FindBishopPinnedPieces(pieceIndex, enemyBishopBitboard, enemyQueenBitboard, friendlyKingBitboard);
        
        if (rookPinnedPieceIndex != -1 | bishopPinnedPieceIndex != -1)   {piecePinned = true;}
        
        
        ulong enemyBitboard = BoardUtils.GetWhiteBitboard();
        ulong friendlyBitboard = BoardUtils.GetBlackBitboard();
        
        
        ulong MovesNotObstructed(ulong movementMask)
        {
            return movementMask & ~friendlyBitboard;
        }


        if (BitboardUtils.isBitOn(Bitboards.BlackKnightBitboard, pieceIndex))
        {   
            if (piecePinned){
                return 0ul;
            }
            
            ulong validMoves = MovesNotObstructed(MovementMasks.KnightMovementMasks[pieceIndex]);

            return validMoves;
        }
    
    
        if (BitboardUtils.isBitOn(Bitboards.BlackPawnBitboard, pieceIndex))
        {
            if (piecePinned){
                return 0ul;
            }
            
            ulong validMoves = MovementMasks.PawnBlackMoveMovementMasks[pieceIndex] & ~(friendlyBitboard | enemyBitboard) >> 8;
            
            validMoves |= 1ul << pieceIndex-8;
            validMoves &= ~(friendlyBitboard | enemyBitboard);

            ulong validAttacks = MovementMasks.PawnBlackAttackMovementMasks[pieceIndex] & enemyBitboard;
            
            if (BoardUtils.IndexToFile(Board.WhiteDoubleMovedPawnIndex) == BoardUtils.IndexToFile(pieceIndex) & (Math.Abs(Board.WhiteDoubleMovedPawnIndex-pieceIndex) == 1))
            {
                validAttacks |= 1ul << pieceIndex - 8 + Board.WhiteDoubleMovedPawnIndex-pieceIndex;
            }
            
            
            return validMoves | validAttacks;
        }
        
        if (BitboardUtils.isBitOn(Bitboards.BlackKingBitboard, pieceIndex))
        {
            ulong validMoves = MovesNotObstructed(MovementMasks.KingMovementMasks[pieceIndex]);
            validMoves &= ~enemyAttackedSquares;
            

            
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
                allValidMoves[index] = FindWhiteNonSlidingValidMoves(index, Board.EnemyAttackedSquares) | FindWhiteSlidingPieceMoves(index);
                Board.WhiteDoubleMovedPawnIndex = 0;
            }
            else
            {   

                allValidMoves[index] = FindBlackNonSlidingValidMoves(index, Board.EnemyAttackedSquares) | FindBlackSlidingPieceMoves(index);
                Board.BlackDoubleMovedPawnIndex = 0;

            }
        }

        if (Board.IsWhite)
        {
            if (Castling.CanWhiteShortCastle(Board.EnemyAttackedSquares))
            {
                allValidMoves[4] |= 64ul;
            }
            
            if (Castling.CanWhiteLongCastle(Board.EnemyAttackedSquares))
            {
                allValidMoves[4] |= 4ul;
            }
        }
        else
        {
            if (Castling.CanBlackShortCastle(Board.EnemyAttackedSquares))
            {
                allValidMoves[60] |= 4611686018427387904ul;
            }

            if (Castling.CanBlackLongCastle(Board.EnemyAttackedSquares))
            {
                allValidMoves[60] |= 288230376151711744ul;
            }
        }
        
        
        return allValidMoves;

    }

    private static ulong GetEnemyAttackSquares()
    {
        ulong attackedSquares = 0ul;
        
        for (int pieceIndex = 0; pieceIndex < 64; pieceIndex++){
            
            if (Board.IsWhite)
            {
                attackedSquares |= FindBlackNonSlidingValidMoves(pieceIndex, 0ul);
                attackedSquares |= FindBlackSlidingPieceMoves(pieceIndex);
                
            }
            else
            {
                attackedSquares |= FindWhiteNonSlidingValidMoves(pieceIndex, 0ul);
                attackedSquares |= FindWhiteSlidingPieceMoves(pieceIndex);

            }
        }

        return attackedSquares;
    }

    
}