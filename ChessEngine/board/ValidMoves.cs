using ChessEngine.bitboard;

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
        if (!getEnemyAttackedSquares)
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

        ulong key = (blockers * PrecomputedMagics.BishopMagics[pieceIndex]) >> PrecomputedMagics.BishopShifts[pieceIndex];
        ulong validMoves = MovementMasks.BishopMovesLookUp[pieceIndex][key];
        
        
        // so that getting protected pieces works
        if (!getEnemyAttackedSquares)
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

        int enemyDoubleMovedPawnIndex = board.Board.BlackDoubleMovedPawnIndex;

        ulong friendlyBitboard = BoardUtils.GetWhiteBitboard();
        ulong enemyBitboard = BoardUtils.GetBlackBitboard();
        

        if (!isWhite)
        {
            friendlyPawnMovementMasks = MovementMasks.PawnBlackMoveMovementMasks;
            friendlyPawnAttackMovementMasks = MovementMasks.PawnBlackAttackMovementMasks;
            enemyDoubleMovedPawnIndex = board.Board.WhiteDoubleMovedPawnIndex;
            friendlyBitboard = BoardUtils.GetBlackBitboard();
            enemyBitboard = BoardUtils.GetWhiteBitboard();
        }

        // pinned
        (int rookPinnedPieceIndex, int enemyPinnedRookIndex, _) = Pins.FindRookPinnedPieces(isWhite, pieceIndex);
        (int bishopPinnedPieceIndex, int enemyPinnedBishopIndex, _) = Pins.FindBishopPinnedPieces(isWhite, pieceIndex);
        
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
                return possiblePinnedPieceCapture;
            }

            return 0ul;
            
            
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
                validAttacks |= 1ul << pieceIndex - 8 + board.Board.WhiteDoubleMovedPawnIndex - pieceIndex;
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
        (int rookPinnedPieceIndex, _, _) = Pins.FindRookPinnedPieces(board.Board.IsWhite, pieceIndex);
        (int bishopPinnedPieceIndex, _, _) = Pins.FindBishopPinnedPieces(board.Board.IsWhite, pieceIndex);

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
        (int rookPinnedPieceIndex, _, _) = Pins.FindRookPinnedPieces(board.Board.IsWhite, pieceIndex);
        (int bishopPinnedPieceIndex, int enemyPinnedBishopsIndex, ulong enemyPinnedBishopMove) = Pins.FindBishopPinnedPieces(board.Board.IsWhite, pieceIndex);

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
        (int rookPinnedPieceIndex, int enemyPinnedRookIndex, ulong enemyPinnedRookMoves) = Pins.FindRookPinnedPieces(board.Board.IsWhite, pieceIndex);
        (int bishopPinnedPieceIndex, _, _) = Pins.FindBishopPinnedPieces(board.Board.IsWhite, pieceIndex);

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
        (int rookPinnedPieceIndex, int enemyPinnedRookIndex, ulong enemyPinnedRookMoves) = Pins.FindRookPinnedPieces(board.Board.IsWhite, pieceIndex);
        (int bishopPinnedPieceIndex, int enemyPinnedBishopIndex, ulong enemyPinnedBishopMoves) = Pins.FindBishopPinnedPieces(board.Board.IsWhite, pieceIndex);

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
            // Console.WriteLine("king in check");
            // BitboardUtils.PrintBitboards(squaresNotAllowedByKing);
            validMoves &= ~ squaresNotAllowedByKing;
        }

        return validMoves;
    }


    private static (ulong, ulong) GetBlockableSquaresWhenChecked(bool isWhite,
        (int, ulong)[] piecesAttackingKingValidMoves)
    {
        // returns a ulong of indexes of squares that can be blocked by a friendly piece
        // and a ulong of indexes that the piece wont allow the king to go to because it will still be in check by that attacking piece


        ulong friendlyKingBitboard = Bitboards.WhiteKingBitboard;

        if (!isWhite)
        {
            friendlyKingBitboard = Bitboards.BlackKingBitboard;
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
                bool attackedOnOrthogonal = (GetRookValidMoves(pieceIndex, !isWhite, true, false) & friendlyKingBitboard) != 0;
                
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
                    ulong enemyRookMovesNoBlockers = GetRookValidMoves(pieceIndex, !isWhite, false, false);
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
                    ulong enemyRookMovesNoBlockers = GetRookValidMoves(pieceIndex, !isWhite, false, false);

                    squaresNeedToBlock |= (kingAsRook & validMoves) | bitboardIndex;
                    squaresNotAllowedByKing |= enemyRookMovesNoBlockers;
                }

            }

            // piece is white or black bishop
            if (pieceType == 3 | pieceType == 9)
            {   

                if (multiplePiecesChecking)
                {
                    ulong enemyBishopMovesNoBlockers = GetBishopValidMoves(pieceIndex, !isWhite, false, false);
                    squaresNotAllowedByKing |= enemyBishopMovesNoBlockers;

                }
                else
                {
                    int kingIndex = BitboardUtils.FindLsb(friendlyKingBitboard);
                    ulong kingAsBishop = GetBishopValidMoves(kingIndex, isWhite, true, false);
                    ulong validEnemyBishopMoves = GetBishopValidMoves(pieceIndex, !isWhite, true, false);
                    ulong enemyBishopMovesNoBlockers = GetBishopValidMoves(pieceIndex, !isWhite, false, false);
                    
                    
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
        ulong enemyKingBitboard = Bitboards.WhiteKingBitboard;
        if (!board.Board.IsWhite)
        {
            enemyKingBitboard = Bitboards.BlackKingBitboard;
        }


        ulong attackedSquares = 0ul;
        List<(int, ulong)> piecesAttackingKingValidMoves = new List<(int, ulong)>();

        bool inCheck = board.Board.InCheck(board.Board.IsWhite, board.Board.EnemyAttackedSquares);

        for (int pieceIndex = 0; pieceIndex < 64; pieceIndex++)
        {

            ulong bitboardIndex = 1ul << pieceIndex;
            ulong currentValidMoves = 0ul;

            if (!board.Board.IsWhite)
            {
                if ((Bitboards.WhitePawnBitboard | bitboardIndex) == Bitboards.WhitePawnBitboard){
                    currentValidMoves = FindPawnMoves(true, pieceIndex, board.Board.WhiteInCheck, 0ul, true);
                }

                if ((Bitboards.WhiteKnightBitboard | bitboardIndex) == Bitboards.WhiteKnightBitboard){
                    currentValidMoves = FindKnightMoves(true, pieceIndex, board.Board.WhiteInCheck, 0ul, true);
                }

                if ((Bitboards.WhiteBishopBitboard | bitboardIndex) == Bitboards.WhiteBishopBitboard){
                    currentValidMoves = FindBishopMoves(true, pieceIndex, board.Board.WhiteInCheck, 0ul, true);
                }

                if ((Bitboards.WhiteRookBitboard | bitboardIndex) == Bitboards.WhiteRookBitboard){
                    currentValidMoves = FindRookMoves(true, pieceIndex, board.Board.WhiteInCheck, 0ul, true);
                }

                if ((Bitboards.WhiteQueenBitboard | bitboardIndex) == Bitboards.WhiteQueenBitboard){
                    currentValidMoves = FindQueenMoves(true, pieceIndex, board.Board.WhiteInCheck, 0ul, true);
                }

                if ((Bitboards.WhiteKingBitboard | bitboardIndex) == Bitboards.WhiteKingBitboard){
                    currentValidMoves = FindKingMoves(true, pieceIndex, board.Board.WhiteInCheck, 0ul, 0ul, true);
                }
            }
            else
            {
                if ((Bitboards.BlackPawnBitboard | bitboardIndex) == Bitboards.BlackPawnBitboard){
                    currentValidMoves = FindPawnMoves(false, pieceIndex, board.Board.BlackInCheck, 0ul, true);
                }

                if ((Bitboards.BlackKnightBitboard | bitboardIndex) == Bitboards.BlackKnightBitboard){
                    currentValidMoves = FindKnightMoves(false, pieceIndex, board.Board.BlackInCheck, 0ul, true);
                }

                if ((Bitboards.BlackBishopBitboard | bitboardIndex) == Bitboards.BlackBishopBitboard){
                    currentValidMoves = FindBishopMoves(false, pieceIndex, board.Board.BlackInCheck, 0ul, true);
                }

                if ((Bitboards.BlackRookBitboard | bitboardIndex) == Bitboards.BlackRookBitboard){
                    currentValidMoves = FindRookMoves(false, pieceIndex, board.Board.BlackInCheck, 0ul, true);
                }

                if ((Bitboards.BlackQueenBitboard | bitboardIndex) == Bitboards.BlackQueenBitboard){
                    currentValidMoves = FindQueenMoves(false, pieceIndex, board.Board.BlackInCheck, 0ul, true);
                }

                if ((Bitboards.BlackKingBitboard | bitboardIndex) == Bitboards.BlackKingBitboard){
                    currentValidMoves = FindKingMoves(false, pieceIndex, board.Board.BlackInCheck, 0ul, 0ul, true);
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

    public static ushort[] FindValidMoves()
    {
        List<ushort> encodedValidMoves = new List<ushort>();

        void EncodeUlongMoves(ulong moves, int startingSquare, int pieceType)
        {
            int[] moveIndexes = BitboardUtils.GetSetBitIndexes(moves);
            for (int i = 0; i < moveIndexes.Length; i++)
            {
                int moveIndex = moveIndexes[i];
                encodedValidMoves.Add(BoardUtils.EncodeMove(startingSquare, moveIndex, pieceType));
            }
        }

        
        (ulong enemyAttackedSquares, (int, ulong)[] piecesAttackingKingValidMoves) =
            GetEnemyAttacksAndKingAttacksSquares();

        (ulong squaresBlockedWhenChecked, ulong squaresNotAllowedByKing) =
            GetBlockableSquaresWhenChecked(board.Board.IsWhite, piecesAttackingKingValidMoves);
        
        // BitboardUtils.PrintBitboards(squaresNotAllowedByKing);
        
        bool inCheck = board.Board.InCheck(board.Board.IsWhite, enemyAttackedSquares);

        if (board.Board.IsWhite)
        {
            board.Board.WhiteInCheck = inCheck;
        }
        else
        {
            board.Board.BlackInCheck = inCheck;
        }
        
        
        // Console.WriteLine(Board.EnemyAttackedSquares);
        
        for (int index = 0; index < 64; index++)
        {
            ulong bitboardIndex = 1ul << index;


            if (board.Board.IsWhite)
            {
                if ((Bitboards.WhitePawnBitboard | bitboardIndex) == Bitboards.WhitePawnBitboard){
                    ulong validPawnMoves = FindPawnMoves(true, index, board.Board.WhiteInCheck, squaresBlockedWhenChecked, false);
                    EncodeUlongMoves(validPawnMoves, index, 1);
                }

                if ((Bitboards.WhiteKnightBitboard | bitboardIndex) == Bitboards.WhiteKnightBitboard){
                    ulong validKnightMoves = FindKnightMoves(true, index, board.Board.WhiteInCheck, squaresBlockedWhenChecked, false);
                    EncodeUlongMoves(validKnightMoves, index, 2);
                }

                if ((Bitboards.WhiteBishopBitboard | bitboardIndex) == Bitboards.WhiteBishopBitboard){
                    ulong validBishopMoves = FindBishopMoves(true, index, board.Board.WhiteInCheck, squaresBlockedWhenChecked, false);
                    EncodeUlongMoves(validBishopMoves, index, 3);
                }

                if ((Bitboards.WhiteRookBitboard | bitboardIndex) == Bitboards.WhiteRookBitboard){
                    ulong validRookMoves = FindRookMoves(true, index, board.Board.WhiteInCheck, squaresBlockedWhenChecked, false);
                    EncodeUlongMoves(validRookMoves, index, 4);
                }

                if ((Bitboards.WhiteQueenBitboard | bitboardIndex) == Bitboards.WhiteQueenBitboard){
                    ulong validQueenMoves = FindQueenMoves(true, index, board.Board.WhiteInCheck, squaresBlockedWhenChecked, false);
                    EncodeUlongMoves(validQueenMoves, index, 5);
                }

                if ((Bitboards.WhiteKingBitboard | bitboardIndex) == Bitboards.WhiteKingBitboard){
                    ulong validKingMoves = FindKingMoves(true, index, board.Board.WhiteInCheck, squaresNotAllowedByKing, enemyAttackedSquares, false);
                    EncodeUlongMoves(validKingMoves, index, 6);
                }

                board.Board.WhiteDoubleMovedPawnIndex = 0;
            }
            else
            {
                if ((Bitboards.BlackPawnBitboard | bitboardIndex) == Bitboards.BlackPawnBitboard){
                    ulong validPawnMoves = FindPawnMoves(false, index, board.Board.BlackInCheck, squaresBlockedWhenChecked, false);
                    EncodeUlongMoves(validPawnMoves, index, 7);
                }

                if ((Bitboards.BlackKnightBitboard | bitboardIndex) == Bitboards.BlackKnightBitboard){
                    ulong validKnightMoves = FindKnightMoves(false, index, board.Board.BlackInCheck, squaresBlockedWhenChecked, false);
                    EncodeUlongMoves(validKnightMoves, index, 8);
                }

                if ((Bitboards.BlackBishopBitboard | bitboardIndex) == Bitboards.BlackBishopBitboard){
                    ulong validBishopMoves = FindBishopMoves(false, index, board.Board.BlackInCheck, squaresBlockedWhenChecked, false);
                    EncodeUlongMoves(validBishopMoves, index, 9);
                }

                if ((Bitboards.BlackRookBitboard | bitboardIndex) == Bitboards.BlackRookBitboard){
                    ulong validRookMoves = FindRookMoves(false, index, board.Board.BlackInCheck, squaresBlockedWhenChecked, false);
                    EncodeUlongMoves(validRookMoves, index, 10);
                }

                if ((Bitboards.BlackQueenBitboard | bitboardIndex) == Bitboards.BlackQueenBitboard){
                    ulong validQueenMoves = FindQueenMoves(false, index, board.Board.BlackInCheck, squaresBlockedWhenChecked, false);
                    EncodeUlongMoves(validQueenMoves, index, 11);
                }

                if ((Bitboards.BlackKingBitboard | bitboardIndex) == Bitboards.BlackKingBitboard){
                    ulong validKingMoves = FindKingMoves(false, index, board.Board.BlackInCheck, squaresNotAllowedByKing, enemyAttackedSquares, false);
                    EncodeUlongMoves(validKingMoves, index, 12);
                }

                board.Board.BlackDoubleMovedPawnIndex = 0;
            }



            if (board.Board.IsWhite)
            {
                board.Board.WhiteDoubleMovedPawnIndex = 0;
            }
            else
            {
                board.Board.BlackDoubleMovedPawnIndex = 0;
            }
        }



        if (board.Board.IsWhite)
        {
            
            if (Castling.CanWhiteShortCastle(enemyAttackedSquares, board.Board.WhiteInCheck))
            {   
                EncodeUlongMoves(64ul, 4, 6);
            }
            
            if (Castling.CanWhiteLongCastle(enemyAttackedSquares, board.Board.WhiteInCheck))
            {   
                EncodeUlongMoves(4ul, 4, 6);
            }
        }
        else
        {
            if (Castling.CanBlackShortCastle(enemyAttackedSquares, board.Board.BlackInCheck))
            {   
                EncodeUlongMoves(4611686018427387904ul, 60, 12);
            }
        
            if (Castling.CanBlackLongCastle(enemyAttackedSquares, board.Board.BlackInCheck))
            {   
                EncodeUlongMoves(288230376151711744ul, 60, 12);
            }
        }


        
        return encodedValidMoves.ToArray();


    }

}