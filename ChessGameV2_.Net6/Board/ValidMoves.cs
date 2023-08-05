using ChessGame.bitboards;


namespace ChessGame.board;

public class ValidMoves
{

    private static ulong GetRookValidMoves(int pieceIndex, bool isWhite, bool includeBlockers)
    {

        ulong friendlyBitboard;
        ulong enemyBitboard;


        if (isWhite)
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

        ulong blockers = 0ul;
        if (includeBlockers) { blockers = rookMovementMask & (enemyBitboard | friendlyBitboard); }
            
        ulong key = (blockers * PrecomputedMagics.RookMagics[pieceIndex]) >> PrecomputedMagics.RookShifts[pieceIndex];
        ulong validMoves = MovementMasks.RookMovesLookUp[pieceIndex][key];
        validMoves &= ~friendlyBitboard;


        return validMoves;
    }
    
    private static ulong GetBishopValidMoves(int pieceIndex, bool isWhite, bool includeBlockers)
    {

        ulong friendlyBitboard;
        ulong enemyBitboard;


        if (isWhite)
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

        ulong blockers = 0ul;
        if (includeBlockers) { blockers = bishopMovementMask & (enemyBitboard | friendlyBitboard); }
        
        ulong key = (blockers * PrecomputedMagics.BishopMagics[pieceIndex]) >> PrecomputedMagics.BishopShifts[pieceIndex];
        ulong validMoves = MovementMasks.BishopMovesLookUp[pieceIndex][key];

        validMoves &= ~friendlyBitboard;

        return validMoves;
    }

    private static ulong FindSlidingPieceMoves(bool isWhite, int pieceIndex, ulong enemyAttackedSquares)
    {
        ulong friendlyRookBitboard = Bitboards.WhiteRookBitboard;
        ulong enemyRookBitboard = Bitboards.BlackRookBitboard;
        ulong friendlyBishopBitboard = Bitboards.WhiteBishopBitboard;
        ulong enemyBishopBitboard = Bitboards.BlackBishopBitboard;
        ulong friendlyQueenBitboard = Bitboards.WhiteQueenBitboard;
        ulong enemyQueenBitboard = Bitboards.BlackQueenBitboard;
        ulong friendlyKingBitboard = Bitboards.WhiteKingBitboard;

        
        if (!isWhite)
        {
            friendlyRookBitboard = Bitboards.BlackRookBitboard;
            enemyRookBitboard = Bitboards.WhiteRookBitboard;
            friendlyBishopBitboard = Bitboards.BlackBishopBitboard;
            enemyBishopBitboard = Bitboards.WhiteBishopBitboard;
            friendlyQueenBitboard = Bitboards.BlackQueenBitboard;
            enemyQueenBitboard = Bitboards.WhiteQueenBitboard;
            friendlyKingBitboard = Bitboards.BlackKingBitboard;
        }
        
        
        // check for checks
        bool inCheck = Board.InCheck(isWhite, enemyAttackedSquares);
        
        ulong squaresNeedToBlock = 0ul;
        
        if (inCheck) {(squaresNeedToBlock, _) = GetSquaresNeedToBlockWhenChecked(enemyAttackedSquares); }

        
        // check for pins
        bool pinnedByBishop = false;
        bool pinnedByRook = false;
        
        (int rookPinnedPieceIndex, int enemyPinnedRookIndex, ulong enemyPinnedRookMovementMask) = Pins.FindRookPinnedPieces(pieceIndex, enemyRookBitboard, enemyQueenBitboard, friendlyKingBitboard);
        (int bishopPinnedPieceIndex, int enemyPinnedBishopsIndex, ulong enemyPinnedBishopMovementMask) = Pins.FindBishopPinnedPieces(pieceIndex, enemyBishopBitboard, enemyQueenBitboard, friendlyKingBitboard);
        
        if (rookPinnedPieceIndex != -1)   {pinnedByRook = true;}
        if (bishopPinnedPieceIndex != -1) {pinnedByBishop = true;}
        
        
        // the queen is considered a rook because its diagonal moves are automatically not valid if pinned by another rook
        if (BitboardUtils.isBitOn(friendlyRookBitboard, pieceIndex))
        {
            // if its pinned by a bishop, then the rook cannot move
            if (pinnedByBishop) { return 0ul; }
            
            // finds valid moves not accounting for being pinned by a rook
            ulong validMoves = GetRookValidMoves(pieceIndex, isWhite, true);
            
            // if it is pinned by a rook then it can only go to squares the enemy pinned rook can go to, and also capture it
            if (pinnedByRook)
            {
                validMoves &= enemyPinnedRookMovementMask;  // shared squares
                validMoves |= 1ul << enemyPinnedRookIndex;  // capture the enemy pinned piece
            }
            
            if (inCheck)
            {
                validMoves &= squaresNeedToBlock;
            }
            
            return validMoves;

            
        }
        
        // the queen is considered a bishop because its orthogonal moves are automatically not valid if pinned by another bishop
        if (BitboardUtils.isBitOn(friendlyBishopBitboard, pieceIndex))
        {
            
            // if its pinned by a rook, then the bishop cannot move
            if (pinnedByRook) {return 0ul; }
            
            // finds valid moves not accounting for being pinned by a bishop
            ulong validMoves = GetBishopValidMoves(pieceIndex, isWhite, true);
            
            if (pinnedByBishop)
            {
                validMoves &= enemyPinnedBishopMovementMask;
                validMoves |= 1ul << enemyPinnedBishopsIndex;
            }
            
            if (inCheck)
            {   
                validMoves &= squaresNeedToBlock;
            }
            
            
            return validMoves;
        }
        
        
        // the queen is considered a bishop because its orthogonal moves are automatically not valid if pinned by another bishop
        if (BitboardUtils.isBitOn(friendlyQueenBitboard, pieceIndex))
        {
            ulong validMoves = GetBishopValidMoves(pieceIndex, isWhite, true);
            validMoves |= GetRookValidMoves(pieceIndex, isWhite, true);
            
            if (pinnedByBishop)
            {
                validMoves &= enemyPinnedBishopMovementMask;
                validMoves |= 1ul << enemyPinnedBishopsIndex;
            }
            
            if (pinnedByRook)
            {
                validMoves &= enemyPinnedRookMovementMask; 
                validMoves |= 1ul << enemyPinnedRookIndex;  
            }
            
            if (inCheck)
            {   
                validMoves &= squaresNeedToBlock;
            }
            
            
            return validMoves;
        }
        

        return 0ul;
    }
    
    private static ulong FindNonSlidingValidMoves(bool isWhite, int pieceIndex, ulong enemyAttackedSquares)
    {
        
        ulong[] friendlyPawnMovementMasks = MovementMasks.PawnWhiteMoveMovementMasks;
        ulong[] friendlyPawnAttackMovementMasks = MovementMasks.PawnWhiteAttackMovementMasks;

        int enemyDoubleMovedPawnIndex = Board.BlackDoubleMovedPawnIndex;
    
        ulong friendlyPawnBitboard = Bitboards.WhitePawnBitboard;
        ulong friendlyKnightBitboard = Bitboards.WhiteKnightBitboard;
        ulong enemyRookBitboard = Bitboards.BlackRookBitboard;
        ulong enemyBishopBitboard = Bitboards.BlackBishopBitboard;
        ulong enemyQueenBitboard = Bitboards.BlackQueenBitboard;
        ulong friendlyKingBitboard = Bitboards.WhiteKingBitboard;
    
    
        ulong friendlyBitboard = BoardUtils.GetWhiteBitboard();
        ulong enemyBitboard = BoardUtils.GetBlackBitboard();

        if (!isWhite)
        {   
            friendlyPawnMovementMasks = MovementMasks.PawnBlackMoveMovementMasks;
            friendlyPawnAttackMovementMasks = MovementMasks.PawnBlackAttackMovementMasks;

            enemyDoubleMovedPawnIndex = Board.BlackDoubleMovedPawnIndex;
            
            friendlyPawnBitboard = Bitboards.BlackPawnBitboard;
            friendlyKnightBitboard = Bitboards.BlackKnightBitboard;
            enemyRookBitboard = Bitboards.WhiteRookBitboard;
            enemyBishopBitboard = Bitboards.WhiteBishopBitboard;
            enemyQueenBitboard = Bitboards.WhiteQueenBitboard;
            friendlyKingBitboard = Bitboards.BlackKingBitboard;
            
            friendlyBitboard = BoardUtils.GetBlackBitboard();
            enemyBitboard = BoardUtils.GetWhiteBitboard();
        }
        
        
        
        // handle checks
        bool inCheck = Board.InCheck(isWhite, enemyAttackedSquares);
        
        ulong squaresNeedToBlock = 0ul;
        ulong squaresNotAllowedByKing = 0ul;
        
        if (inCheck) {(squaresNeedToBlock, squaresNotAllowedByKing) = GetSquaresNeedToBlockWhenChecked(enemyAttackedSquares); }
        
        // handle pins 
        bool piecePinned = false;
        
        
        (int rookPinnedPieceIndex, _, _) = Pins.FindRookPinnedPieces(pieceIndex, enemyRookBitboard, enemyQueenBitboard, friendlyKingBitboard);
        (int bishopPinnedPieceIndex, _, _) = Pins.FindBishopPinnedPieces(pieceIndex, enemyBishopBitboard, enemyQueenBitboard, friendlyKingBitboard);
        
        if (rookPinnedPieceIndex != -1 | bishopPinnedPieceIndex != -1)   {piecePinned = true;}
        
        
        // returns a movement mask that does not include captures of friendly pieces
        ulong MovesNotObstructed(ulong movementMask)
        {
            return movementMask & ~friendlyBitboard;
        }
        

        if (BitboardUtils.isBitOn(friendlyKnightBitboard, pieceIndex))
        {
            if (piecePinned){
                return 0ul;
            }
            
            ulong validMoves = MovesNotObstructed(MovementMasks.KnightMovementMasks[pieceIndex]);

            if (inCheck)
            {
                validMoves &= squaresNeedToBlock;
            }
            
            return validMoves;
        }
            
            
        if (BitboardUtils.isBitOn(friendlyPawnBitboard, pieceIndex))
        {
            ulong validMoves;
            ulong validAttacks;
                
            if (piecePinned){
                return 0ul;
            }
            
            if (isWhite)
            {
            
                validMoves = friendlyPawnMovementMasks[pieceIndex] & ~(friendlyBitboard | enemyBitboard) << 8;
                validMoves |= 1ul << pieceIndex+8;
                validMoves &= ~(friendlyBitboard | enemyBitboard);

                validAttacks = friendlyPawnAttackMovementMasks[pieceIndex] & enemyBitboard;
            
                // en passant
                if (BoardUtils.IndexToFile(enemyDoubleMovedPawnIndex) == BoardUtils.IndexToFile(pieceIndex) & (Math.Abs(enemyDoubleMovedPawnIndex-pieceIndex) == 1))
                {
                    validAttacks |= 1ul << pieceIndex + 8 + enemyDoubleMovedPawnIndex-pieceIndex;
                }
            }
            else
            {
                validMoves = friendlyPawnMovementMasks[pieceIndex] & ~(friendlyBitboard | enemyBitboard) >> 8;
                validMoves |= 1ul << pieceIndex-8;
                validMoves &= ~(friendlyBitboard | enemyBitboard);

                validAttacks = friendlyPawnMovementMasks[pieceIndex] & enemyBitboard;
            
                if (BoardUtils.IndexToFile(enemyDoubleMovedPawnIndex) == BoardUtils.IndexToFile(pieceIndex) & (Math.Abs(enemyDoubleMovedPawnIndex-pieceIndex) == 1))
                {
                    validAttacks |= 1ul << pieceIndex - 8 + Board.WhiteDoubleMovedPawnIndex-pieceIndex;
                }
            }
          
            
            if (inCheck)
            {
                validMoves &= squaresNeedToBlock;
                validAttacks &= squaresNeedToBlock;
            }
            
            return (validMoves | validAttacks) ;
        }
            
        if (BitboardUtils.isBitOn(friendlyKingBitboard, pieceIndex))
        {
            ulong validMoves = MovesNotObstructed(MovementMasks.KingMovementMasks[pieceIndex]);

            if (inCheck)
            {
                validMoves &= ~ squaresNotAllowedByKing;
            }
            
            return validMoves;
        }

        return 0ul;
    }
    
    private static (ulong, ulong) GetSquaresNeedToBlockWhenChecked(ulong enemyAttackedSquares)
    {
        ulong friendlyKingBitboard = Bitboards.WhiteKingBitboard;
        ulong enemyRookBitboard = Bitboards.BlackRookBitboard;
        ulong enemyBishopBitboard = Bitboards.BlackBishopBitboard;
        
        if (!Board.IsWhite)
        {
            friendlyKingBitboard = Bitboards.WhiteKingBitboard;
            enemyRookBitboard = Bitboards.BlackRookBitboard;
            enemyBishopBitboard = Bitboards.BlackBishopBitboard;
        }


        int numPiecesCheckingKing = 0;
        ulong squaresNeedToBlock = 0ul;
        ulong squaresNotAllowedByKing = 0ul;
        
        for (int pieceIndex = 0; pieceIndex < 64; pieceIndex++)
        {

            ulong bitboardIndex = 1ul << pieceIndex;
            
            // checked by a rook
            if ((enemyRookBitboard | bitboardIndex) == enemyRookBitboard)
            {
                numPiecesCheckingKing++;
                
                
                int kingIndex = BitboardUtils.FindLsb(friendlyKingBitboard);
                ulong kingAsRook = GetRookValidMoves(kingIndex, Board.IsWhite, true);
                ulong validEnemyRookMoves = GetRookValidMoves(pieceIndex, Board.IsWhite, true);
                ulong enemyRookMovesNoBlockers = GetRookValidMoves(pieceIndex, Board.IsWhite, false);
                
                squaresNeedToBlock |= ((kingAsRook & validEnemyRookMoves) | bitboardIndex);
                squaresNotAllowedByKing |= enemyRookMovesNoBlockers;
            }
            
            // checked by a Bishop
            else if ((enemyBishopBitboard | bitboardIndex) == enemyBishopBitboard)
            {   
                
                numPiecesCheckingKing++;
                
                int kingIndex = BitboardUtils.FindLsb(friendlyKingBitboard);
                ulong kingAsRook = GetRookValidMoves(kingIndex, Board.IsWhite, true);
                ulong validEnemyRookMoves = GetRookValidMoves(pieceIndex, Board.IsWhite, true);
                ulong enemyBishopMovesNoBlockers = GetBishopValidMoves(pieceIndex, Board.IsWhite, false);

                
                squaresNeedToBlock |= ((kingAsRook & validEnemyRookMoves) | bitboardIndex);
                squaresNotAllowedByKing |= enemyBishopMovesNoBlockers;
            }
            
            // checked by anything else because in that case all we have to do is that that piece
            else if ((BoardUtils.GetBlackBitboard() | bitboardIndex) == BoardUtils.GetBlackBitboard())
            {   
                numPiecesCheckingKing++;
                squaresNeedToBlock |= bitboardIndex;
            }
            
        }

        if (numPiecesCheckingKing > 1)
        {
            return (0ul, squaresNotAllowedByKing);
        }
        
        return (squaresNeedToBlock, squaresNotAllowedByKing);


    }

    private static ulong GetEnemyAttackSquares()
    {
        ulong attackedSquares = 0ul;
        
        for (int pieceIndex = 0; pieceIndex < 64; pieceIndex++){
            
            attackedSquares |= FindNonSlidingValidMoves(!Board.IsWhite, pieceIndex, 0ul);
            attackedSquares |= FindSlidingPieceMoves(!Board.IsWhite, pieceIndex, 0ul);
            

        }

        return attackedSquares;
    }

    public static ulong[] FindValidMoves()
    {

        ulong[] allValidMoves = new ulong[64];

        Board.EnemyAttackedSquares = GetEnemyAttackSquares();
        
        for (int index = 0; index < 64; index++)
        {
            
            ulong indexValidMoves = FindSlidingPieceMoves(Board.IsWhite, index, Board.EnemyAttackedSquares);
            indexValidMoves |= FindNonSlidingValidMoves(Board.IsWhite, index, Board.EnemyAttackedSquares);

            allValidMoves[index] = indexValidMoves;
                
            if (Board.IsWhite) {Board.WhiteDoubleMovedPawnIndex = 0;}
            else { Board.BlackDoubleMovedPawnIndex = 0; }
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
    
    
}