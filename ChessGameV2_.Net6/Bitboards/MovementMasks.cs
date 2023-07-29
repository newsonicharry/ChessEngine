using ChessGame.board;


namespace ChessGame.bitboards;

public class MovementMasks
{

    public static readonly ulong[] RookMovementMasks = new ulong[64];
    public static readonly ulong[] BishopMovementMasks = new ulong[64];
    public static readonly ulong[] QueenMovementMasks = new ulong[64];
    public static readonly ulong[] KnightMovementMasks = new ulong[64];
    public static readonly ulong[] KingMovementMasks = new ulong[64];
    
    public static readonly ulong[] PawnWhiteMoveMovementMasks = new ulong[64];
    public static readonly ulong[] PawnWhiteAttackMovementMasks = new ulong[64];
    
    public static readonly ulong[] PawnBlackMoveMovementMasks = new ulong[64];
    public static readonly ulong[] PawnBlackAttackMovementMasks = new ulong[64];



    public static void CreateMovementMasks()
    {
        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                int index = y * 8 + x;
                
                ulong rookMovementMask = CreateRookMovementMask(x, y);
                ulong bishopMovementMask = CreateBishopMovementMask(x, y);
                ulong queenMovementMask = rookMovementMask | bishopMovementMask;
                
                RookMovementMasks[index] = rookMovementMask;
                BishopMovementMasks[index] = bishopMovementMask;
                QueenMovementMasks[index] = queenMovementMask;
                
                
                KnightMovementMasks[index] = CreateKnightMovementMask(x, y);
                KingMovementMasks[index] = CreateKingMovementMask(x, y);
                
                PawnWhiteMoveMovementMasks[index] = CreatePawnMoveMovementMask(x, y, true);
                PawnWhiteAttackMovementMasks[index] = CreatePawnAttackMovementMask(x, y, true);
                
                PawnBlackMoveMovementMasks[index] = CreatePawnMoveMovementMask(x, y, false);
                PawnBlackAttackMovementMasks[index] = CreatePawnAttackMovementMask(x, y, false);
                
            }
        }
    }
    
    private static ulong CreateKnightMovementMask(int xKnight, int yKnight)
    {
        ulong movementMask = 0ul;
        int knightIndex = yKnight * 8 + xKnight;

        for (int i = 0; i < 64; i++)
        {
            if (i == knightIndex + 17 & (BoardUtils.IndexToFile(knightIndex + 17) - yKnight == 2)) { movementMask |= 1ul << i; }
            if (i == knightIndex - 17 & (yKnight - BoardUtils.IndexToFile(knightIndex - 17) == 2)) { movementMask |= 1ul << i; }
                
            if (i == knightIndex + 15 & (BoardUtils.IndexToFile(knightIndex + 15) - yKnight == 2)) { movementMask |= 1ul << i; }
            if (i == knightIndex - 15 & (yKnight - BoardUtils.IndexToFile(knightIndex - 15) == 2)) { movementMask |= 1ul << i; }
                
            if (i == knightIndex + 10 & (BoardUtils.IndexToFile(knightIndex + 10) - yKnight == 1)) { movementMask |= 1ul << i; }
            if (i == knightIndex - 10 & (yKnight - BoardUtils.IndexToFile(knightIndex - 10) == 1)) { movementMask |= 1ul << i; }
                
            if (i == knightIndex + 6 && (BoardUtils.IndexToFile(knightIndex +  6) - yKnight == 1)) { movementMask |= 1ul << i; }
            if (i == knightIndex - 6 && (yKnight - BoardUtils.IndexToFile(knightIndex -  6) == 1)) { movementMask |= 1ul << i; }    
        }
        

        return movementMask;
    }
    
    private static ulong CreateRookMovementMask(int xRook, int yRook)
    {
        ulong movementMask = 0ul;

        for (int y = 0; y < 8; y += 1)
        {
            for (int x = 0; x < 8; x += 1)
            {   

                int index = y * 8 + x;

                if ((x == xRook || y == yRook) && !(x == xRook && y == yRook))
                {
                    movementMask |= 1ul << index;
                }
            }
        }

        return movementMask;
    }
    
    private static ulong CreateKingMovementMask(int xKing, int yKing)
    {
        ulong movementMask = 0ul;
        int kingIndex = yKing * 8 + xKing;

        for (int i = 0; i < 64; i++)
        {
                   
            int[] verticalOffsets = {7, 8, 9, -7, -8, -9};

            foreach (int offset in verticalOffsets)
            {
                if (i == kingIndex + offset &
                    (Math.Abs(BoardUtils.IndexToFile(kingIndex + offset) - yKing) == 1))
                {
                    movementMask |= 1ul << i;
                }

            }
                
            if (i == kingIndex + 1 & BoardUtils.IndexToFile(kingIndex + 1) == yKing) { movementMask |= 1ul << i; }
            if (i == kingIndex - 1 & BoardUtils.IndexToFile(kingIndex - 1) == yKing) { movementMask |= 1ul << i; }
        }


        return movementMask;
    }

    private static ulong CreateBishopMovementMask(int xBishop, int yBishop)
    { 
        ulong movementMask = 0ul;
        int bishopIndex = yBishop * 8 + xBishop;
        
        for (int y = 0; y < 8; y += 1) {
            for (int x = 0; x < 8; x += 1) {
                
                int index = y * 8 + x;
                
                int yDistance = Math.Abs(yBishop - y);

                if (bishopIndex == index)
                {
                    continue;
                }
                
                if (index == bishopIndex+7*yDistance && yBishop != BoardUtils.IndexToFile(bishopIndex+7)){
                    movementMask |= 1ul << Math.Abs(index);
                }
                
                if (index == bishopIndex-7*yDistance && yBishop != BoardUtils.IndexToFile(bishopIndex+7)){
                    movementMask |= 1ul << Math.Abs(index);
                }
                
                if (index == bishopIndex+9*yDistance){
                    movementMask |= 1ul << Math.Abs(index);
                }
                
                if (index == bishopIndex-9*yDistance){
                    movementMask |= 1ul << Math.Abs(index);
                }

            }
        }
        return movementMask;

    }
    
    private static ulong CreatePawnAttackMovementMask(int xPawn, int yPawn, bool isWhite)
    {
        ulong movementMask = 0ul;
        int pawnIndex = yPawn * 8 + xPawn;

        for (int i = 0; i < 64; i++)
        {
            if (isWhite)
            {
                if (i == pawnIndex + 7 & (BoardUtils.IndexToFile(pawnIndex + 7) - yPawn == 1)) { movementMask |= 1ul << i; }
                if (i == pawnIndex + 9 & (BoardUtils.IndexToFile(pawnIndex + 9) - yPawn == 1)) { movementMask |= 1ul << i; }
 
            }
            else
            {
                if (i == pawnIndex - 7 & (yPawn - BoardUtils.IndexToFile(pawnIndex - 7) == 1)) { movementMask |= 1ul << i; }
                if (i == pawnIndex - 9 & (yPawn - BoardUtils.IndexToFile(pawnIndex - 9) == 1)) { movementMask |= 1ul << i; }
            }
        }

        return movementMask;
    }

    private static ulong CreatePawnMoveMovementMask(int xPawn, int yPawn, bool isWhite)
    {
        ulong movementMask = 0ul;
        int pawnIndex = yPawn * 8 + xPawn;

        for (int i = 0; i < 64; i++)
        {
            if (isWhite)
            {
                if (pawnIndex+8 == i)
                {
                    movementMask |= 1ul << i;
                    if (yPawn == 1)
                    {
                        movementMask |= 1ul << i+8;

                    }
                }

                    
            }
            else
            {
                if (pawnIndex-8 == i)
                {
                    movementMask |= 1ul << i;

                }
            }
        }
        
        
        return movementMask;
    }
    
    
    public static ulong[] GenerateRookMovesLookup()
    {
        ulong[] rookValidMoveLookup = new ulong[1048576];

        
        ulong[] rookMovementMasks = RookMovementMasks;

        int rookIndex = 0;
        foreach (ulong rookMovementMask in rookMovementMasks)
        {   
            ulong[] blockers = Blockers.GenerateBlockers(rookMovementMask);

            for (int i = 0; i < blockers.Length; i++)
            {
                ulong blocker = blockers[i];
                ulong key = (blocker * PrecomputedMagics.RookMagics[rookIndex]) >> PrecomputedMagics.RookShifts[rookIndex];
                
                ulong validMoves = Blockers.GetRookMovesFromBlockers(rookMovementMask, blocker, rookIndex);

                rookValidMoveLookup[key] = validMoves;

            }

            rookIndex++;

        }
        
        return rookValidMoveLookup;

    }
    
    
    public static ulong[] GenerateBishopMovesLookup()
    {
        ulong[] bishopValidMoveLookup = new ulong[70399];
        
        ulong[] bishopMovementMasks = MovementMasks.BishopMovementMasks;

        int bishopIndex = 0;
        foreach (ulong bishopMovementMask in bishopMovementMasks)
        {   
            ulong[] blockers = Blockers.GenerateBlockers(bishopMovementMask);

            for (int i = 0; i < blockers.Length; i++)
            {
                ulong blocker = blockers[i];
                ulong key = (blocker * PrecomputedMagics.BishopMagics[bishopIndex]) >> PrecomputedMagics.BishopShifts[bishopIndex];
                
                ulong validMove = Blockers.GetRookMovesFromBlockers(bishopMovementMask, blocker, bishopIndex);
                
                bishopValidMoveLookup[key] = validMove;

            }

            bishopIndex++;

        }
        
        
        return bishopValidMoveLookup;

    }
}
    
    
