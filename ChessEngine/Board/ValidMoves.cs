using System.Reflection.Metadata.Ecma335;
using ChessEngine.Bitboards;

namespace ChessEngine.Board;

public class ValidMoves
{

    private static ulong GetRookValidMoves(int pieceIndex, bool isWhite, bool includeBlockers, bool getEnemyAttackedSquares)
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
        if (includeBlockers)
        {
            blockers = rookMovementMask & (enemyBitboard | friendlyBitboard);
        }

        ulong key = (blockers * PrecomputedMagics.RookMagics[pieceIndex]) >> PrecomputedMagics.RookShifts[pieceIndex];
        ulong validMoves = MovementMasks.RookMovesLookUp[pieceIndex][key];
        
        // so that checking for protected pieces works
        if (getEnemyAttackedSquares)
        {
            validMoves &= ~friendlyBitboard;
        }


        return validMoves;
    }

    private static ulong GetBishopValidMoves(int pieceIndex, bool isWhite, bool includeBlockers, bool getEnemyAttackedSquares)
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
        if (includeBlockers)
        {
            blockers = bishopMovementMask & (enemyBitboard | friendlyBitboard);
        }

        ulong key = (blockers * PrecomputedMagics.BishopMagics[pieceIndex]) >>
                    PrecomputedMagics.BishopShifts[pieceIndex];
        ulong validMoves = MovementMasks.BishopMovesLookUp[pieceIndex][key];
        
        
        // so that getting protected pieces works
        if (getEnemyAttackedSquares)
        {
            validMoves &= ~friendlyBitboard;
        }

        return validMoves;
    }

    private static ulong FindPawnMoves(bool isWhite, int pieceIndex, bool inCheck, ulong squaresNeedToBlock, bool getEnemyAttackedSquares)
    {   
        
        // declare basic bitboards
        ulong[] friendlyPawnMovementMasks = MovementMasks.PawnWhiteMoveMovementMasks;
        ulong[] friendlyPawnAttackMovementMasks = MovementMasks.PawnWhiteAttackMovementMasks;

        int enemyDoubleMovedPawnIndex = Board.BlackDoubleMovedPawnIndex;

        ulong friendlyBitboard = BoardUtils.GetWhiteBitboard();
        ulong enemyBitboard = BoardUtils.GetBlackBitboard();
        

        if (!isWhite)
        {
            friendlyPawnMovementMasks = MovementMasks.PawnBlackMoveMovementMasks;
            friendlyPawnAttackMovementMasks = MovementMasks.PawnBlackAttackMovementMasks;
            enemyDoubleMovedPawnIndex = Board.WhiteDoubleMovedPawnIndex;
            friendlyBitboard = BoardUtils.GetBlackBitboard();
            enemyBitboard = BoardUtils.GetWhiteBitboard();
        }

        // pinned
        (int rookPinnedPieceIndex, int enemyPinnedRookIndex, _) = Pins.FindRookPinnedPieces(Board.IsWhite, pieceIndex);
        (int bishopPinnedPieceIndex, int enemyPinnedBishopIndex, _) = Pins.FindBishopPinnedPieces(Board.IsWhite, pieceIndex);
        
        bool piecePinned = rookPinnedPieceIndex != -1 | bishopPinnedPieceIndex != -1;
        
        
        ulong validMoves = 0ul;
        ulong validAttacks = 0ul;
        
        // if pinned then there are no possible moves other than taking the enemy piece
        if (piecePinned)
        {   
            
            // checks if it can take the enemy piece
            ulong enemyPinnedRookBitboardIndex = 1ul << enemyPinnedRookIndex;
            ulong enemyPinnedBishopBitboardIndex = 1ul << enemyPinnedBishopIndex;

            ulong possiblePinnedPieceCapture = friendlyPawnAttackMovementMasks[pieceIndex] & (enemyPinnedRookBitboardIndex | enemyPinnedBishopBitboardIndex);
            
            if (possiblePinnedPieceCapture != 0)
            {
                validAttacks = possiblePinnedPieceCapture;
            }
        }

        
        if (isWhite)
        {
            // so that the pawn moving does not count as it attack that square
            if (!getEnemyAttackedSquares)
            {
                validMoves = friendlyPawnMovementMasks[pieceIndex] & ~(friendlyBitboard | enemyBitboard) << 8;
                validMoves |= 1ul << pieceIndex + 8;
                validMoves &= ~(friendlyBitboard | enemyBitboard);
            }

            

            // en passant
            if (BoardUtils.IndexToFile(enemyDoubleMovedPawnIndex) == BoardUtils.IndexToFile(pieceIndex) &
                (Math.Abs(enemyDoubleMovedPawnIndex - pieceIndex) == 1))
            {
                validAttacks |= 1ul << pieceIndex + 8 + enemyDoubleMovedPawnIndex - pieceIndex;
            }
        }
        else
        {
            if (!getEnemyAttackedSquares)
            {
                validMoves = friendlyPawnMovementMasks[pieceIndex] & ~(friendlyBitboard | enemyBitboard) >> 8;
                validMoves |= 1ul << pieceIndex - 8;
                validMoves &= ~(friendlyBitboard | enemyBitboard);
            }


            if (BoardUtils.IndexToFile(enemyDoubleMovedPawnIndex) == BoardUtils.IndexToFile(pieceIndex) &
                (Math.Abs(enemyDoubleMovedPawnIndex - pieceIndex) == 1))
            {
                validAttacks |= 1ul << pieceIndex - 8 + Board.WhiteDoubleMovedPawnIndex - pieceIndex;
            }
        }

        
        // so that a king can not move into the way of a enemy pawns attack
        if (getEnemyAttackedSquares)
        {
            validAttacks |= friendlyPawnAttackMovementMasks[pieceIndex];
        }
        else
        {
            validAttacks |= friendlyPawnAttackMovementMasks[pieceIndex] & enemyBitboard;
        }
        
        
        if (inCheck)
        {   
            validMoves &= squaresNeedToBlock;
            validAttacks &= squaresNeedToBlock;
        }

        return (validMoves | validAttacks);
    }

    private static ulong FindKnightMoves(bool isWhite, int pieceIndex, bool inCheck, ulong squaresNeedToBlock, bool getEnemyAttackedSquares)
    {
        ulong friendlyBitboard = BoardUtils.GetWhiteBitboard();
        if (!isWhite) { friendlyBitboard = BoardUtils.GetBlackBitboard(); }
        
        // pinned
        (int rookPinnedPieceIndex, _, _) = Pins.FindRookPinnedPieces(Board.IsWhite, pieceIndex);
        (int bishopPinnedPieceIndex, _, _) = Pins.FindBishopPinnedPieces(Board.IsWhite, pieceIndex);

        bool piecePinned = rookPinnedPieceIndex != -1 | bishopPinnedPieceIndex != -1;
        

        if (piecePinned)
        {
            return 0ul;
        }

        ulong validMoves = MovementMasks.KnightMovementMasks[pieceIndex];

        if (!getEnemyAttackedSquares)
        {
            validMoves &= ~friendlyBitboard;
        }

        if (inCheck)
        {
            validMoves &= squaresNeedToBlock;
        }

        return validMoves;
    }

    private static ulong FindBishopMoves(bool isWhite, int pieceIndex, bool inCheck, ulong squaresNeedToBlock, bool getEnemyAttackedSquares)
    {
        // pinned
        (int rookPinnedPieceIndex, _, _) = Pins.FindRookPinnedPieces(Board.IsWhite, pieceIndex);
        (int bishopPinnedPieceIndex, int enemyPinnedBishopsIndex, ulong enemyPinnedBishopMove) = Pins.FindBishopPinnedPieces(Board.IsWhite, pieceIndex);

        bool pinnedByRook = rookPinnedPieceIndex != -1;
        bool pinnedByBishop = bishopPinnedPieceIndex != -1;


        if (pinnedByRook)
        {
            return 0ul;
        }

        // finds valid moves not accounting for being pinned by a bishop
        ulong validMoves = GetBishopValidMoves(pieceIndex, isWhite, true, getEnemyAttackedSquares);

        if (pinnedByBishop)
        {
            validMoves &= enemyPinnedBishopMove;
            validMoves |= 1ul << enemyPinnedBishopsIndex;
        }

        if (inCheck)
        {
            validMoves &= squaresNeedToBlock;
        }


        return validMoves;
    }

    private static ulong FindRookMoves(bool isWhite, int pieceIndex, bool inCheck, ulong squaresNeedToBlock, bool getEnemyAttackedSquares)
    {
        // pinned
        (int rookPinnedPieceIndex, int enemyPinnedRookIndex, ulong enemyPinnedRookMoves) = Pins.FindRookPinnedPieces(Board.IsWhite, pieceIndex);
        (int bishopPinnedPieceIndex, _, _) = Pins.FindBishopPinnedPieces(Board.IsWhite, pieceIndex);

        bool pinnedByRook = rookPinnedPieceIndex != -1;
        bool pinnedByBishop = bishopPinnedPieceIndex != -1;
        

        // if its pinned by a bishop, then the rook cannot move
        if (pinnedByBishop)
        {
            return 0ul;
        }

        // finds valid moves not accounting for being pinned by a rook
        ulong validMoves = GetRookValidMoves(pieceIndex, isWhite, true, getEnemyAttackedSquares);

        // if it is pinned by a rook then it can only go to squares the enemy pinned rook can go to, and also capture it
        if (pinnedByRook)
        {
            validMoves &= enemyPinnedRookMoves; // shared squares
            validMoves |= 1ul << enemyPinnedRookIndex; // capture the enemy pinned piece
        }

        if (inCheck)
        {
            validMoves &= squaresNeedToBlock;
        }

        return validMoves;
    }

    private static ulong FindQueenMoves(bool isWhite, int pieceIndex, bool inCheck, ulong squaresNeedToBlock, bool getEnemyAttackedSquares)
    {
        // pinned
        (int rookPinnedPieceIndex, int enemyPinnedRookIndex, ulong enemyPinnedRookMoves) = Pins.FindRookPinnedPieces(Board.IsWhite, pieceIndex);
        (int bishopPinnedPieceIndex, int enemyPinnedBishopIndex, ulong enemyPinnedBishopMoves) = Pins.FindBishopPinnedPieces(Board.IsWhite, pieceIndex);

        bool pinnedByRook = rookPinnedPieceIndex != -1;
        bool pinnedByBishop = bishopPinnedPieceIndex != -1;
        

        ulong validMoves = GetBishopValidMoves(pieceIndex, isWhite, true, getEnemyAttackedSquares);
        validMoves |= GetRookValidMoves(pieceIndex, isWhite, true, getEnemyAttackedSquares);

        if (pinnedByBishop)
        {
            validMoves &= enemyPinnedBishopMoves;
            validMoves |= 1ul << enemyPinnedBishopIndex;
        }

        if (pinnedByRook)
        {
            validMoves &= enemyPinnedRookMoves;
            validMoves |= 1ul << enemyPinnedRookIndex;
        }

        if (inCheck)
        {
            validMoves &= squaresNeedToBlock;
        }


        return validMoves;
    }

    private static ulong FindKingMoves(bool isWhite, int pieceIndex, bool inCheck, ulong squaresNotAllowedByKing, ulong enemyAttackedSquares, bool getEnemyAttackedSquares)
    {
        ulong friendlyBitboard = BoardUtils.GetWhiteBitboard();
        ulong enemyBitboard = BoardUtils.GetBlackBitboard();

        if (!isWhite)
        {
            friendlyBitboard = BoardUtils.GetBlackBitboard();
            enemyBitboard = BoardUtils.GetWhiteBitboard();
        }
        
        
        ulong validMoves = MovementMasks.KingMovementMasks[pieceIndex];

        if (!getEnemyAttackedSquares)
        {
            validMoves &= ~friendlyBitboard;

        }

        validMoves &= ~ enemyAttackedSquares;
            
            
        ulong enemyPiecesProtected = enemyBitboard & enemyAttackedSquares;
        
        validMoves &= ~enemyPiecesProtected;
        
        
        if (inCheck)
        {
            validMoves &= ~ squaresNotAllowedByKing;
        }

        return validMoves;
    }


    private static (ulong, ulong) GetBlockableSquaresWhenChecked(bool isWhite,
        (int, ulong)[] piecesAttackingKingValidMoves)
    {
        // returns a ulong of indexes of squares that can be blocked by a friendly piece
        // and a ulong of indexes that the piece wont allow the king to go to because it will still be in check by that attacking piece


        ulong friendlyKingBitboard = Bitboards.Bitboards.WhiteKingBitboard;

        if (!isWhite)
        {
            friendlyKingBitboard = Bitboards.Bitboards.BlackKingBitboard;
        }


        ulong squaresNeedToBlock = 0ul;
        ulong squaresNotAllowedByKing = 0ul;

        bool multiplePiecesChecking = piecesAttackingKingValidMoves.Length > 1;


        foreach ((int index, ulong validMoves) data in piecesAttackingKingValidMoves)
        {
            int pieceIndex = data.index;
            ulong validMoves = data.validMoves;
            ulong bitboardIndex = 1ul << pieceIndex;

            int pieceType = BoardUtils.IndexToPieceType(pieceIndex);
        
            // if being attacked by a queen
            if (pieceType == 5 | pieceType == 11)
            {
                // check if its acting as a rook would, so checking on its orthogonal
                bool attackedOnOrthogonal = (GetRookValidMoves(pieceIndex, isWhite, true, false) & friendlyKingBitboard) != 0;
                // if so then call it a white rook
                // not then we know its attacked on its diagonals so its a bishop
                pieceType = attackedOnOrthogonal ? 4 :3;
            }
            
            
            // piece equal to white or black rook or black or white queen
            if (pieceType == 4 | pieceType == 10)
            {       
                // if multiple pieces checking then the only option is moving the king not blocking the check
                if (multiplePiecesChecking)
                {
                    ulong enemyRookMovesNoBlockers = GetRookValidMoves(pieceIndex, isWhite, false, false);
                    squaresNotAllowedByKing |= enemyRookMovesNoBlockers;
                }
                else
                {   
                    // simulates the king as a rook to figure out the overlapping squares of the king as rook and the enemy rook
                    // and using an & shows all squares that can be used to block with a friendly piece from the check
                    int kingIndex = BitboardUtils.FindLsb(friendlyKingBitboard);
                    ulong kingAsRook = GetRookValidMoves(kingIndex, isWhite, true, false);
                    
                    // the rook shown without any blockers as to know that when the king moves, it may not be still checked
                    // by the enemy rook
                    ulong enemyRookMovesNoBlockers = GetRookValidMoves(pieceIndex, isWhite, false, false);

                    squaresNeedToBlock |= (kingAsRook & validMoves) | bitboardIndex;
                    squaresNotAllowedByKing |= enemyRookMovesNoBlockers;
                }

            }

            // piece is white or black bishop
            if (pieceType == 3 | pieceType == 9)
            {   

                if (multiplePiecesChecking)
                {
                    ulong enemyBishopMovesNoBlockers = GetBishopValidMoves(pieceIndex, isWhite, false, false);
                    squaresNotAllowedByKing |= enemyBishopMovesNoBlockers;

                }
                else
                {
                    int kingIndex = BitboardUtils.FindLsb(friendlyKingBitboard);
                    ulong kingAsBishop = GetBishopValidMoves(kingIndex, isWhite, true, false);
                    ulong validEnemyBishopMoves = GetBishopValidMoves(pieceIndex, isWhite, true, false);
                    ulong enemyBishopMovesNoBlockers = GetBishopValidMoves(pieceIndex, isWhite, false, false);
                    
                    
                    squaresNeedToBlock |= ((kingAsBishop & validEnemyBishopMoves) | bitboardIndex);
                    squaresNotAllowedByKing |= enemyBishopMovesNoBlockers;
                }
            }

            // piece is anything else
            if (pieceType != 3 & pieceType != 4 & pieceType != 5 & pieceType != 9 & pieceType != 10 & pieceType != 11)
            {   
                // dont need to worry about squares not allowed, because we know the king will not be checked by the same 
                // piece when it moves
                squaresNeedToBlock |= bitboardIndex;
            }

            
        }
        
        return (squaresNeedToBlock, squaresNotAllowedByKing);

    }

    private static (ulong, (int, ulong)[]) GetEnemyAttacksAndKingAttacksSquares()
    {
        ulong enemyKingBitboard = Bitboards.Bitboards.WhiteKingBitboard;
        if (!Board.IsWhite)
        {
            enemyKingBitboard = Bitboards.Bitboards.BlackKingBitboard;
        }


        ulong attackedSquares = 0ul;
        List<(int, ulong)> piecesAttackingKingValidMoves = new List<(int, ulong)>();

        bool inCheck = Board.InCheck(Board.IsWhite, Board.EnemyAttackedSquares);

        for (int pieceIndex = 0; pieceIndex < 64; pieceIndex++)
        {

            ulong bitboardIndex = 1ul << pieceIndex;
            ulong currentValidMoves = 0ul;

            if (!Board.IsWhite)
            {
                if ((Bitboards.Bitboards.WhitePawnBitboard | bitboardIndex) == Bitboards.Bitboards.WhitePawnBitboard){
                    currentValidMoves = FindPawnMoves(true, pieceIndex, inCheck, 0ul, true);
                }

                if ((Bitboards.Bitboards.WhiteKnightBitboard | bitboardIndex) == Bitboards.Bitboards.WhiteKnightBitboard){
                    currentValidMoves = FindKnightMoves(true, pieceIndex, inCheck, 0ul, true);
                }

                if ((Bitboards.Bitboards.WhiteBishopBitboard | bitboardIndex) == Bitboards.Bitboards.WhiteBishopBitboard){
                    currentValidMoves = FindBishopMoves(true, pieceIndex, inCheck, 0ul, true);
                }

                if ((Bitboards.Bitboards.WhiteRookBitboard | bitboardIndex) == Bitboards.Bitboards.WhiteRookBitboard){
                    currentValidMoves = FindRookMoves(true, pieceIndex, inCheck, 0ul, true);
                }

                if ((Bitboards.Bitboards.WhiteQueenBitboard | bitboardIndex) == Bitboards.Bitboards.WhiteQueenBitboard){
                    currentValidMoves = FindQueenMoves(true, pieceIndex, inCheck, 0ul, true);
                }

                if ((Bitboards.Bitboards.WhiteKingBitboard | bitboardIndex) == Bitboards.Bitboards.WhiteKingBitboard){
                    currentValidMoves = FindKingMoves(true, pieceIndex, inCheck, 0ul, 0ul, true);
                }
            }
            else
            {
                if ((Bitboards.Bitboards.BlackPawnBitboard | bitboardIndex) == Bitboards.Bitboards.BlackPawnBitboard){
                    currentValidMoves = FindPawnMoves(false, pieceIndex, inCheck, 0ul, true);
                }

                if ((Bitboards.Bitboards.BlackKnightBitboard | bitboardIndex) == Bitboards.Bitboards.BlackKnightBitboard){
                    currentValidMoves = FindKnightMoves(false, pieceIndex, inCheck, 0ul, true);
                }

                if ((Bitboards.Bitboards.BlackBishopBitboard | bitboardIndex) == Bitboards.Bitboards.BlackBishopBitboard){
                    currentValidMoves = FindBishopMoves(false, pieceIndex, inCheck, 0ul, true);
                }

                if ((Bitboards.Bitboards.BlackRookBitboard | bitboardIndex) == Bitboards.Bitboards.BlackRookBitboard){
                    currentValidMoves = FindRookMoves(false, pieceIndex, inCheck, 0ul, true);
                }

                if ((Bitboards.Bitboards.BlackQueenBitboard | bitboardIndex) == Bitboards.Bitboards.BlackQueenBitboard){
                    currentValidMoves = FindQueenMoves(false, pieceIndex, inCheck, 0ul, true);
                }

                if ((Bitboards.Bitboards.BlackKingBitboard | bitboardIndex) == Bitboards.Bitboards.BlackKingBitboard){
                    currentValidMoves = FindKingMoves(false, pieceIndex, inCheck, 0ul, 0ul, true);
                }
            }


            if ((currentValidMoves | enemyKingBitboard) == currentValidMoves)
            {
                piecesAttackingKingValidMoves.Add((pieceIndex, currentValidMoves));
            }

            attackedSquares |= currentValidMoves;

        }

        return (attackedSquares, piecesAttackingKingValidMoves.ToArray());
    }

    public static ulong[] FindValidMoves()
    {
        

        
        (ulong enemyAttackedSquares, (int, ulong)[] piecesAttackingKingValidMoves) =
            GetEnemyAttacksAndKingAttacksSquares();

        (ulong squaresBlockedWhenChecked, ulong squaresNotAllowedByKing) =
            GetBlockableSquaresWhenChecked(Board.IsWhite, piecesAttackingKingValidMoves);
        
        // BitboardUtils.PrintBitboards(squaresNotAllowedByKing);
        
        ulong[] allValidMoves = new ulong[64];

        bool inCheck = Board.InCheck(Board.IsWhite, enemyAttackedSquares);
        
        // Console.WriteLine(Board.EnemyAttackedSquares);
        
        for (int index = 0; index < 64; index++)
        {
            ulong bitboardIndex = 1ul << index;


            if (Board.IsWhite)
            {
                if ((Bitboards.Bitboards.WhitePawnBitboard | bitboardIndex) == Bitboards.Bitboards.WhitePawnBitboard){
                    allValidMoves[index] = FindPawnMoves(true, index, inCheck, squaresBlockedWhenChecked, false);
                }

                if ((Bitboards.Bitboards.WhiteKnightBitboard | bitboardIndex) == Bitboards.Bitboards.WhiteKnightBitboard){
                    allValidMoves[index] = FindKnightMoves(true, index, inCheck, squaresBlockedWhenChecked, false);
                }

                if ((Bitboards.Bitboards.WhiteBishopBitboard | bitboardIndex) == Bitboards.Bitboards.WhiteBishopBitboard){
                    allValidMoves[index] = FindBishopMoves(true, index, inCheck, squaresBlockedWhenChecked, false);
                }

                if ((Bitboards.Bitboards.WhiteRookBitboard | bitboardIndex) == Bitboards.Bitboards.WhiteRookBitboard){
                    allValidMoves[index] = FindRookMoves(true, index, inCheck, squaresBlockedWhenChecked, false);
                }

                if ((Bitboards.Bitboards.WhiteQueenBitboard | bitboardIndex) == Bitboards.Bitboards.WhiteQueenBitboard){
                    allValidMoves[index] = FindQueenMoves(true, index, inCheck, squaresBlockedWhenChecked, false);
                }

                if ((Bitboards.Bitboards.WhiteKingBitboard | bitboardIndex) == Bitboards.Bitboards.WhiteKingBitboard){
                    allValidMoves[index] = FindKingMoves(true, index, inCheck, squaresNotAllowedByKing, enemyAttackedSquares, false);
                }

                Board.WhiteDoubleMovedPawnIndex = 0;
            }
            else
            {
                if ((Bitboards.Bitboards.BlackPawnBitboard | bitboardIndex) == Bitboards.Bitboards.BlackPawnBitboard){
                    allValidMoves[index] = FindPawnMoves(false, index, inCheck, squaresBlockedWhenChecked, false);
                }

                if ((Bitboards.Bitboards.BlackKnightBitboard | bitboardIndex) == Bitboards.Bitboards.BlackKnightBitboard){
                    allValidMoves[index] = FindKnightMoves(false, index, inCheck, squaresBlockedWhenChecked, false);
                }

                if ((Bitboards.Bitboards.BlackBishopBitboard | bitboardIndex) == Bitboards.Bitboards.BlackBishopBitboard){
                    allValidMoves[index] = FindBishopMoves(false, index, inCheck, squaresBlockedWhenChecked, false);
                }

                if ((Bitboards.Bitboards.BlackRookBitboard | bitboardIndex) == Bitboards.Bitboards.BlackRookBitboard){
                    allValidMoves[index] = FindRookMoves(false, index, inCheck, squaresBlockedWhenChecked, false);
                }

                if ((Bitboards.Bitboards.BlackQueenBitboard | bitboardIndex) == Bitboards.Bitboards.BlackQueenBitboard){
                    allValidMoves[index] = FindQueenMoves(false, index, inCheck, squaresBlockedWhenChecked, false);
                }

                if ((Bitboards.Bitboards.BlackKingBitboard | bitboardIndex) == Bitboards.Bitboards.BlackKingBitboard){
                    allValidMoves[index] = FindKingMoves(false, index, inCheck, squaresNotAllowedByKing, enemyAttackedSquares, false);
                }

                Board.BlackDoubleMovedPawnIndex = 0;
            }



            if (Board.IsWhite)
            {
                Board.WhiteDoubleMovedPawnIndex = 0;
            }
            else
            {
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

}