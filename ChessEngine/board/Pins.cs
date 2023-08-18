using ChessEngine.bitboard;

namespace ChessEngine.Board;

public abstract class Pins
{
    
    // i wrote this code in a daze
    // dont worry about what it does
    // it probably works    (jk there is no way it just works)

    private static ulong GetRookMoves(int rookIndex, ulong blockers)
    {
        ulong enemyPieces;

        if (board.Board.IsWhite)
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

        ulong key = (blockers * PrecomputedMagics.BishopMagics[bishopIndex]) >> PrecomputedMagics.BishopShifts[bishopIndex];
        ulong validMoves = MovementMasks.BishopMovesLookUp[bishopIndex][key];
        
        return validMoves;
    }

    public static (int, int, ulong) FindBishopPinnedPieces(bool isWhite, int friendlyPieceIndex)
    {
        
        ulong enemyBishopBitboard = Bitboards.BlackBishopBitboard;
        ulong enemyQueenBitboard = Bitboards.BlackQueenBitboard;
        ulong friendlyKingBitboard = Bitboards.WhiteKingBitboard;
        ulong friendlyBitboard = BoardUtils.GetWhiteBitboard();


        if (!isWhite)
        {
            enemyBishopBitboard = Bitboards.WhiteBishopBitboard;
            enemyQueenBitboard = Bitboards.WhiteQueenBitboard;
            friendlyKingBitboard = Bitboards.BlackKingBitboard;
            friendlyBitboard = BoardUtils.GetBlackBitboard();

        }
        
        int[] enemyBishopIndexes = BitboardUtils.GetSetBitIndexes(enemyBishopBitboard).ToList().Concat(BitboardUtils.GetSetBitIndexes(enemyQueenBitboard)).ToArray();

        List<ulong> enemyBishopMoves = new List<ulong>();

        for (int index = 0; index < enemyBishopIndexes.Length; index++){
            int enemyBishopIndex = enemyBishopIndexes[index];
            enemyBishopMoves.Add(GetBishopMoves(enemyBishopIndex, 0ul));
        }
            
        
        // i dont like this much nesting but like if it saves a tiny bit of performance then why not
        // i need performance for this engine
        
        for (int i = 0; i < enemyBishopMoves.ToArray().Length; i++)
        {
            int enemyBishopIndex = enemyBishopIndexes[i];
            ulong enemyBishopMove = enemyBishopMoves[i];
            bool isAttackingKing = (enemyBishopMove | friendlyKingBitboard) == enemyBishopMove;

            ulong friendlyPieceIndexBitboard = 1ul << friendlyPieceIndex;
            
            // if the bishop can actually check the king, assuming it has no blockers
            if (isAttackingKing){
                
                // the enemy bishop moves, with the friendly piece as a blocker
                // checks if the enemy bishop can still check the king with the friendly piece as a blocker
                // if so then it is not pinned
                ulong bishopMovesWithPiece = GetBishopMoves(enemyBishopIndex, BitboardUtils.RemoveEdgeIndexes(friendlyPieceIndexBitboard & enemyBishopMove));
                bool attackingKingWithPieceInWay = (bishopMovesWithPiece | friendlyKingBitboard) == bishopMovesWithPiece;
                

                if (!attackingKingWithPieceInWay)
                {
                    
                    // checks if the king can still be checked with other friendly pieces on the board
                    // if it can be then there are no other pieces in the way, and so cant pin two pieces at once
                    ulong friendlyBitboardWithoutPiece = BitboardUtils.NegateBit(friendlyBitboard, friendlyPieceIndex);
                    
                    ulong bishopMovesWithOtherPieces = GetBishopMoves(enemyBishopIndex, BitboardUtils.RemoveEdgeIndexes(friendlyBitboardWithoutPiece & enemyBishopMove));
                    bool attackingKingWithOtherPieces =  (bishopMovesWithOtherPieces | friendlyKingBitboard) == bishopMovesWithOtherPieces;

                    if (friendlyPieceIndex == 13)
                    {
                        // BitboardUtils.PrintBitboards(friendlyBitboardWithoutPiece & enemyBishopMove);

                        // Console.WriteLine("attacking king with other pieces = " + attackingKingWithOtherPieces);

                    }

                    if (attackingKingWithOtherPieces)
                    {   
                        return (friendlyPieceIndex, enemyBishopIndex, enemyBishopMove);
                    }
                    

                }
            }
        }
        
        // default for no pins
        return (-1, -1, 0ul);
    }

    public static (int, int, ulong) FindRookPinnedPieces(bool isWhite, int friendlyPieceIndex)
    {
        
        ulong enemyRookBitboard = Bitboards.BlackRookBitboard;
        ulong enemyQueenBitboard = Bitboards.BlackQueenBitboard;
        ulong friendlyKingBitboard = Bitboards.WhiteKingBitboard;
        ulong friendlyBitboard = BoardUtils.GetWhiteBitboard();


        if (!isWhite)
        {
            enemyRookBitboard = Bitboards.WhiteRookBitboard;
            enemyQueenBitboard = Bitboards.WhiteQueenBitboard;
            friendlyKingBitboard = Bitboards.BlackKingBitboard;
            friendlyBitboard = BoardUtils.GetBlackBitboard();

        }
        
        int[] enemyRookIndexes = BitboardUtils.GetSetBitIndexes(enemyRookBitboard).ToList().Concat(BitboardUtils.GetSetBitIndexes(enemyQueenBitboard)).ToArray();

        List<ulong> enemyRookMoves = new List<ulong>();

        for (int index = 0; index < enemyRookIndexes.Length; index++){
            int enemyRookIndex = enemyRookIndexes[index];
            enemyRookMoves.Add(GetRookMoves(enemyRookIndex, 0ul));
        }
            
        
        // i dont like this much nesting but like if it saves a tiny bit of performance then why not
        // i need performance for this engine
        
        for (int i = 0; i < enemyRookMoves.Count; i++)
        {
            int enemyRookIndex = enemyRookIndexes[i];
            ulong enemyRookMove = enemyRookMoves[i];
            bool isAttackingKing = (enemyRookMove | friendlyKingBitboard) == enemyRookMove;

            ulong friendlyPieceIndexBitboard = 1ul << friendlyPieceIndex;
            
            // if the rook can actually check the king, assuming it has no blockers
            if (isAttackingKing){
                
                
                // the enemy rook moves, with the friendly piece as a blocker
                // checks if the enemy rook can still check the king with the friendly piece as a blocker
                // if so then it is not pinned
                ulong rookMovesWithPiece = GetRookMoves(enemyRookIndex, BitboardUtils.RemoveEdgeIndexes(friendlyPieceIndexBitboard & enemyRookMove));
                bool attackingKingWithPieceInWay = (rookMovesWithPiece | friendlyKingBitboard) == rookMovesWithPiece;
                

                if (!attackingKingWithPieceInWay)
                {
                    

                    // checks if the king can still be checked with other friendly pieces on the board
                    // if it can be then there are no other pieces in the way, and so cant pin two pieces at once
                    ulong friendlyBitboardWithoutPiece = BitboardUtils.NegateBit(friendlyBitboard, friendlyPieceIndex);
                    
                    ulong rookMovesWithOtherPieces = GetRookMoves(enemyRookIndex, (friendlyBitboardWithoutPiece & enemyRookMove));
                    bool attackingKingWithOtherPieces =  (rookMovesWithOtherPieces | friendlyKingBitboard) == rookMovesWithOtherPieces;


                    if (attackingKingWithOtherPieces)
                    {   
                        return (friendlyPieceIndex, enemyRookIndex, enemyRookMove);
                    }
                    

                }
            }
        }
        
        // default for no pins
        return (-1, -1, 0ul);
    }
    
    
    
}

