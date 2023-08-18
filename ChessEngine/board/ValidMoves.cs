using ChessEngine.bitboard;

namespace ChessEngine.Board;

public abstract class ValidMoves
{
    public static ulong WhitePawnAttacks;
    public static ulong WhiteKnightAttacks;
    public static ulong WhiteBishopAttacks;
    public static ulong WhiteRookAttacks;
    public static ulong WhiteQueenAttacks;
    public static ulong WhiteKingAttacks;
    
    public static ulong BlackPawnAttacks;
    public static ulong BlackKnightAttacks;
    public static ulong BlackBishopAttacks;
    public static ulong BlackRookAttacks;
    public static ulong BlackQueenAttacks;
    public static ulong BlackKingAttacks;

    public static List<(int, ulong)> WhitePiecesAttackingKing = new List<(int, ulong)>();
    public static List<(int, ulong)> BlackPiecesAttackingKing = new List<(int, ulong)>();

    
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
        
        // if (pieceIndex == 52)
        // {
        //     BitboardUtils.PrintBitboards(validMoves);
        //     Console.WriteLine(pinnedByRook);
        // }

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
    
    private static (ulong, ulong) GetBlockableSquaresWhenChecked(bool isWhite, (int, ulong)[] piecesAttackingKingValidMoves)
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


        ulong squaresBlockedWhenChecked;
        ulong squaresNotAllowedByKing;
        
        ulong enemyAttackedSquares;
        
        if (board.Board.IsWhite){
            enemyAttackedSquares = BlackPawnAttacks | BlackKnightAttacks | BlackBishopAttacks | BlackRookAttacks | BlackQueenAttacks | BlackKingAttacks;
            (squaresBlockedWhenChecked, squaresNotAllowedByKing) = GetBlockableSquaresWhenChecked(board.Board.IsWhite, BlackPiecesAttackingKing.ToArray());
            
            WhitePiecesAttackingKing.Clear();
            
            WhitePawnAttacks = 0ul;
            WhiteKnightAttacks = 0ul;
            WhiteBishopAttacks = 0ul;
            WhiteRookAttacks = 0ul;
            WhiteQueenAttacks = 0ul;
            WhiteKingAttacks = 0ul;
        }else{
            enemyAttackedSquares = WhitePawnAttacks | WhiteKnightAttacks | WhiteBishopAttacks | WhiteRookAttacks | WhiteQueenAttacks | WhiteKingAttacks;
            (squaresBlockedWhenChecked, squaresNotAllowedByKing) = GetBlockableSquaresWhenChecked(board.Board.IsWhite, WhitePiecesAttackingKing.ToArray());
            
            BlackPiecesAttackingKing.Clear();
            
            BlackPawnAttacks = 0ul;
            BlackKnightAttacks = 0ul;
            BlackBishopAttacks = 0ul;
            BlackRookAttacks = 0ul;
            BlackQueenAttacks = 0ul;
            BlackKingAttacks = 0ul;
        }
        
        
        
        bool inCheck = board.Board.InCheck(board.Board.IsWhite, enemyAttackedSquares);

        if (board.Board.IsWhite){
            board.Board.WhiteInCheck = inCheck;
        }else{
            board.Board.BlackInCheck = inCheck;
        }


                
        for (int index = 0; index < 64; index++)
        {
            ulong bitboardIndex = 1ul << index;

            ulong validMoves = 0ul;

            if (board.Board.IsWhite)
            {
                if ((Bitboards.WhitePawnBitboard | bitboardIndex) == Bitboards.WhitePawnBitboard){
                    validMoves = FindPawnMoves(true, index, board.Board.WhiteInCheck, squaresBlockedWhenChecked, false);
                    WhitePawnAttacks |= validMoves;
                    EncodeUlongMoves(validMoves, index, 1);
                }

                if ((Bitboards.WhiteKnightBitboard | bitboardIndex) == Bitboards.WhiteKnightBitboard){
                    validMoves = FindKnightMoves(true, index, board.Board.WhiteInCheck, squaresBlockedWhenChecked, false);
                    WhiteKnightAttacks |= validMoves;
                    EncodeUlongMoves(validMoves, index, 2);
                }

                if ((Bitboards.WhiteBishopBitboard | bitboardIndex) == Bitboards.WhiteBishopBitboard){
                    validMoves = FindBishopMoves(true, index, board.Board.WhiteInCheck, squaresBlockedWhenChecked, false);
                    WhiteBishopAttacks |= validMoves;
                    EncodeUlongMoves(validMoves, index, 3);
                }

                if ((Bitboards.WhiteRookBitboard | bitboardIndex) == Bitboards.WhiteRookBitboard){
                    validMoves = FindRookMoves(true, index, board.Board.WhiteInCheck, squaresBlockedWhenChecked, false);
                    WhiteRookAttacks |= validMoves;
                    EncodeUlongMoves(validMoves, index, 4);
                }

                if ((Bitboards.WhiteQueenBitboard | bitboardIndex) == Bitboards.WhiteQueenBitboard){
                    validMoves = FindQueenMoves(true, index, board.Board.WhiteInCheck, squaresBlockedWhenChecked, false);
                    WhiteQueenAttacks |= validMoves;
                    EncodeUlongMoves(validMoves, index, 5);
                }

                if ((Bitboards.WhiteKingBitboard | bitboardIndex) == Bitboards.WhiteKingBitboard){
                    validMoves = FindKingMoves(true, index, board.Board.WhiteInCheck, squaresNotAllowedByKing, enemyAttackedSquares, false);
                    WhiteKingAttacks |= validMoves;
                    EncodeUlongMoves(validMoves, index, 6);
                }
                
                if ((validMoves | Bitboards.BlackKingBitboard) == validMoves){
                    WhitePiecesAttackingKing.Add((index, validMoves));
                }
                
            }
            else
            {
                if ((Bitboards.BlackPawnBitboard | bitboardIndex) == Bitboards.BlackPawnBitboard){
                    
                    validMoves = FindPawnMoves(false, index, board.Board.BlackInCheck, squaresBlockedWhenChecked, false);
                    
                    BlackPawnAttacks |= validMoves;
                    EncodeUlongMoves(validMoves, index, 7);
                }

                if ((Bitboards.BlackKnightBitboard | bitboardIndex) == Bitboards.BlackKnightBitboard){
                    validMoves = FindKnightMoves(false, index, board.Board.BlackInCheck, squaresBlockedWhenChecked, false);
                    BlackKnightAttacks |= validMoves;
                    EncodeUlongMoves(validMoves, index, 8);
                }

                if ((Bitboards.BlackBishopBitboard | bitboardIndex) == Bitboards.BlackBishopBitboard){
                    validMoves = FindBishopMoves(false, index, board.Board.BlackInCheck, squaresBlockedWhenChecked, false);
                    BlackBishopAttacks |= validMoves;
                    EncodeUlongMoves(validMoves, index, 9);
                }

                if ((Bitboards.BlackRookBitboard | bitboardIndex) == Bitboards.BlackRookBitboard){
                    validMoves = FindRookMoves(false, index, board.Board.BlackInCheck, squaresBlockedWhenChecked, false);
                    BlackRookAttacks |= validMoves;
                    EncodeUlongMoves(validMoves, index, 10);
                }

                if ((Bitboards.BlackQueenBitboard | bitboardIndex) == Bitboards.BlackQueenBitboard){
                    validMoves = FindQueenMoves(false, index, board.Board.BlackInCheck, squaresBlockedWhenChecked, false);
                    BlackQueenAttacks |= validMoves;
                    EncodeUlongMoves(validMoves, index, 11);
                }

                if ((Bitboards.BlackKingBitboard | bitboardIndex) == Bitboards.BlackKingBitboard){
                    validMoves = FindKingMoves(false, index, board.Board.BlackInCheck, squaresNotAllowedByKing, enemyAttackedSquares, false);
                    BlackKingAttacks |= validMoves;
                    EncodeUlongMoves(validMoves, index, 12);
                }
                
                if ((validMoves | Bitboards.WhiteKingBitboard) == validMoves){
                    BlackPiecesAttackingKing.Add((index, validMoves));
                }
                
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
            Castling.WhiteCastleKingSide = false;
            Castling.WhiteCastleQueenSide = false;
            
            if (Castling.CanWhiteShortCastle(enemyAttackedSquares, board.Board.WhiteInCheck))
            {   
                EncodeUlongMoves(64ul, 4, 6);
                Castling.WhiteCastleKingSide = true;
            }
            
            if (Castling.CanWhiteLongCastle(enemyAttackedSquares, board.Board.WhiteInCheck))
            {   
                EncodeUlongMoves(4ul, 4, 6);
                Castling.WhiteCastleQueenSide = true;

            }
        }
        else
        {   
            
            Castling.BlackCastleKingSide = false;
            Castling.BlackCastleQueenSide = false;
            
            if (Castling.CanBlackShortCastle(enemyAttackedSquares, board.Board.BlackInCheck))
            {   
                EncodeUlongMoves(4611686018427387904ul, 60, 12);
                Castling.BlackCastleKingSide = true;
            }
        
            if (Castling.CanBlackLongCastle(enemyAttackedSquares, board.Board.BlackInCheck))
            {   
                EncodeUlongMoves(288230376151711744ul, 60, 12);
                Castling.BlackCastleQueenSide = true;

            }
        }



        
        
        return encodedValidMoves.ToArray();


    }

}