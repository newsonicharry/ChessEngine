using ChessGame.bitboards;

namespace ChessGame.board;

public class Pins
{
    private static ulong GetRookMoves(int rookIndex, ulong blockers)
    {
        ulong enemyPieces;

        if (Board.IsWhite)
        {
            enemyPieces = BoardUtils.GetBlackBitboard() & ~ Bitboards.BlackRookBitboard;
        }
        else
        {
            enemyPieces = BoardUtils.GetWhiteBitboard() & ~ Bitboards.WhiteRookBitboard;

        }


        ulong key = (blockers * PrecomputedMagics.RookMagics[rookIndex]) >> PrecomputedMagics.RookShifts[rookIndex];
        ulong rookMoves = MovementMasks.RookMovesLookUp[rookIndex][key];
        rookMoves &= ~enemyPieces;

        return rookMoves;
    }

    private static ulong GetBishopMoves(int bishopIndex, ulong blockers)
    {
        ulong enemyPieces;

        if (Board.IsWhite)
        {
            enemyPieces = BoardUtils.GetBlackBitboard() & ~ Bitboards.BlackBishopBitboard;
        }
        else
        {
            enemyPieces = BoardUtils.GetWhiteBitboard() & ~ Bitboards.WhiteBishopBitboard;

        }

        ulong key = (blockers * PrecomputedMagics.BishopMagics[bishopIndex]) >>
                    PrecomputedMagics.BishopShifts[bishopIndex];
        ulong validMoves = MovementMasks.BishopMovesLookUp[bishopIndex][key];
        validMoves &= ~enemyPieces;


        return validMoves;
    }

    public static (int, int, ulong) FindBishopPinnedPieces(int friendlyPieceIndex, ulong enemyBishopBitboard,
        ulong enemyQueenBitboard, ulong friendlyKingBitboard)
    {
        int[] enemyBishopIndexes = BitboardUtils.GetSetBitIndexes(enemyBishopBitboard).ToList()
            .Concat(BitboardUtils.GetSetBitIndexes(enemyQueenBitboard)).ToArray();

        List<ulong> enemyBishopMoves = new List<ulong>();

        for (int i = 0; i < enemyBishopIndexes.Length; i++)
        {
            enemyBishopMoves.Add(GetBishopMoves(enemyBishopIndexes[i], 0ul));
        }

        ulong enemyPinnedBishopMove = 0ul;
        int enemyPinnedBishopsIndex = -1;
        int pinnedPiecesIndex = -1;

        for (int i = 0; i < enemyBishopMoves.Count; i++)
        {

            ulong enemyBishopMove = enemyBishopMoves[i];
            bool isAttackingKing = (enemyBishopMove | friendlyKingBitboard) == enemyBishopMove;

            List<int> possiblePinnedPieces = new List<int>();
            if (isAttackingKing)
            {
                // checks if the piece is actually in the way of the rook
                if ((enemyBishopMove | (1ul << friendlyPieceIndex)) == enemyBishopMove)
                {
                    // adds that piece as a blocker
                    ulong newBishopMoves = GetBishopMoves(i, 1ul << friendlyPieceIndex);

                    // checks if with the piece in the way it can not attack the king
                    if ((newBishopMoves | friendlyKingBitboard) != newBishopMoves)
                    {
                        // if so then it is probably pinned (does not account for multiple pieces in the way)
                        possiblePinnedPieces.Add(friendlyPieceIndex);
                    }


                    if (possiblePinnedPieces.Count == 1)
                    {
                        pinnedPiecesIndex = possiblePinnedPieces[0];
                        enemyPinnedBishopMove = enemyBishopMove;
                        enemyPinnedBishopsIndex = enemyBishopIndexes[i];
                    }
                }

            }

        }
        return (pinnedPiecesIndex, enemyPinnedBishopsIndex, enemyPinnedBishopMove);
    }

    public static (int, int, ulong) FindRookPinnedPieces(int friendlyPieceIndex, ulong enemyRookBitboard,
        ulong enemyQueenBitboard, ulong friendlyKingBitboard)
    {

        int[] enemyRookIndexes = BitboardUtils.GetSetBitIndexes(enemyRookBitboard).ToList()
            .Concat(BitboardUtils.GetSetBitIndexes(enemyQueenBitboard)).ToArray();

        List<ulong> enemyRookMoves = new List<ulong>();

        for (int i = 0; i < enemyRookIndexes.Length; i++)
        {
            enemyRookMoves.Add(GetRookMoves(enemyRookIndexes[i], 0ul));
        }



        int pinnedPieceIndex = -1;
        int enemyPinnedRookIndex = -1;
        ulong enemyPinnedRookMovementMask = 0ul;


        for (int i = 0; i < enemyRookMoves.Count; i++)
        {

            ulong enemyRookMove = enemyRookMoves[i];
            bool isAttackingKing = (enemyRookMove | friendlyKingBitboard) == enemyRookMove;

            List<int> possiblePinnedPieces = new List<int>();
            if (isAttackingKing)
            {
                // checks if the piece is actually in the way of the rook
                if ((enemyRookMove | (1ul << friendlyPieceIndex)) == enemyRookMove)
                {
                    // adds that piece as a blocker
                    ulong newRookMoves = GetRookMoves(i, 1ul << friendlyPieceIndex);

                    // checks if with the piece in the way it can not attack the king
                    if ((newRookMoves | friendlyKingBitboard) != newRookMoves)
                    {
                        // if so then it is probably pinned (does not account for multiple pieces in the way)
                        possiblePinnedPieces.Add(friendlyPieceIndex);

                    }
                }


                if (possiblePinnedPieces.Count == 1)
                {
                    pinnedPieceIndex = possiblePinnedPieces[0];
                    enemyPinnedRookMovementMask = enemyRookMove;
                    enemyPinnedRookIndex = enemyRookIndexes[i];

                }
            }
        }

        return (pinnedPieceIndex, enemyPinnedRookIndex, enemyPinnedRookMovementMask);
    }
    
    
}

