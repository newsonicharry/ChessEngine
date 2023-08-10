using ChessEngine.Board;

namespace ChessEngine.bitboard;

public abstract class MovementMasks
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
        
    public static readonly ulong[] RookMovementMasksNoEdges = new ulong[64];
    public static readonly ulong[] BishopMovementMasksNoEdges = new ulong[64];

    public static readonly ulong[][] RookMovesLookUp = new ulong[64][];
    public static ulong[][] BishopMovesLookUp = new ulong[64][];



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

                RookMovementMasksNoEdges[index] = CreateRookMovementMaskNoEdges(x, y);
                BishopMovementMasksNoEdges[index] = CreateBishopMovementMaskNoEdges(x,y);

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

                if (!(x == xRook && y == yRook))
                {
                    if ((x == xRook) || (y == yRook))
                    {

                        movementMask |= 1ul << index;
                        
                    }
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

    private static ulong CreateBishopMovementMaskNoEdges(int xBishop, int yBishop)
    { 
        ulong movementMask = 0ul;
        int bishopIndex = yBishop * 8 + xBishop;
        
        for (int y = 1; y < 7; y += 1) {
            for (int x = 1; x < 7; x += 1) {
                
                int index = y * 8 + x;
                
                int yDistance = Math.Abs(yBishop - y);

                if (bishopIndex == index)
                {
                    continue;
                }
                
                if (index == bishopIndex+7*yDistance && yBishop != BoardUtils.IndexToFile(bishopIndex+7)){
                    movementMask |= 1ul << Math.Abs(index);
                }
                
                if (index == bishopIndex-7*yDistance && yBishop != BoardUtils.IndexToFile(bishopIndex-7)){
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
                
                if (index == bishopIndex-7*yDistance && yBishop != BoardUtils.IndexToFile(bishopIndex-7)){
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
                    if (yPawn == 6)
                    {
                        movementMask |= 1ul << i-8;

                    }

                }
            }
        }
        
        
        return movementMask;
    }
    
    private static ulong CreateRookMovementMaskNoEdges(int xRook, int yRook)
    {
        ulong movementMask = 0ul;

        for (int y = 0; y < 8; y += 1)
        {
            for (int x = 0; x < 8; x += 1)
            {   

                int index = y * 8 + x;

                if (!(x == xRook && y == yRook))
                {
                    if ((x == xRook))
                    {
                        if (y != 0 & y != 7)
                        {
                            movementMask |= 1ul << index;

                        }
                    }

                    if (y == yRook)
                    {
                        if (x != 0 & x != 7)
                        {
                            movementMask |= 1ul << index;
                        }
                    }
                }
                
            }
        }

        return movementMask;
    }

    
    public static void GenerateRookMovesLookup()
    {
        
        int rookIndex = 0;
        foreach (ulong rookMovementMask in RookMovementMasksNoEdges)
        {


            RookMovesLookUp[rookIndex] = new ulong[16386];
            ulong[] blockers = Blockers.GenerateBlockers(rookMovementMask);

            foreach (ulong blocker in blockers)
            {

                ulong key = (blocker * PrecomputedMagics.RookMagics[rookIndex]) >> PrecomputedMagics.RookShifts[rookIndex];
                ulong validMoves = Blockers.GetRookMovesFromBlockers(rookMovementMask,blocker, rookIndex);
                
                RookMovesLookUp[rookIndex][key] = validMoves;
                
                
                
            }

            rookIndex++;

        }

    }
    
    
    public static void GenerateBishopMovesLookup()
    {
        
        int bishopIndex = 0;
        foreach (ulong bishopMovementMask in BishopMovementMasksNoEdges)
        {
            BishopMovesLookUp[bishopIndex] = new ulong[10000];
            
            ulong[] blockers = Blockers.GenerateBlockers(bishopMovementMask);

            foreach (ulong blocker in blockers)
            {   
                ulong magic = PrecomputedMagics.BishopMagics[bishopIndex];
                int shift = PrecomputedMagics.BishopShifts[bishopIndex];

                ulong key = (blocker * magic) >> shift;
                
                ulong validMove = Blockers.GetBishopMovesFromBlockers(bishopMovementMask, blocker, bishopIndex);
                
                
                BishopMovesLookUp[bishopIndex][key] = validMove;
                
                
            }

            bishopIndex++;

        }
        

    }
}
    
    
