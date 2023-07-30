using System.Dynamic;
using ChessGame.board;

namespace ChessGame.bitboards;

public class Blockers
{
    
    
    public static ulong[] GenerateBlockers(ulong movementMask)
    {   
        
        List<int> moveIndexes = new List<int>();
        for (int i = 0; i < 64; i++)
        { if (((movementMask >> i) & 1) == 1)
            {
                moveIndexes.Add(i);
            }
        }

        int totalBlockerPatterns = 1 << moveIndexes.Count;
        ulong[] allBlockerPatterns = new ulong[totalBlockerPatterns];


        for (int patternIndex = 0; patternIndex < totalBlockerPatterns; patternIndex++)
        {
            for (int bitIndex = 0; bitIndex < moveIndexes.Count; bitIndex++)
            {   
                // these two lines of code are a mystery that i may never truly understand
                int bit = (patternIndex >> bitIndex) & 1;
                allBlockerPatterns[patternIndex] |= (ulong)bit << moveIndexes[bitIndex];
            }
        }        
        return allBlockerPatterns;
    }

    public static ulong GetBishopMovesFromBlockers(ulong bishopMovementMask, ulong blockersBitboard, int bishopIndex) 
    {
        ulong validBoard = bishopMovementMask & (~blockersBitboard);
        
        
        bool CheckValidBits(int i, bool canNegate)
        {
            if (canNegate)
            {
                ulong bitmask = (ulong)1 << i;
                bitmask = ~bitmask;
                validBoard &= bitmask;
            }
            else
            {
                ulong bitmask = (ulong)1 << i;
                bool isBitZero = (validBoard & bitmask) == 0;

                if (isBitZero)
                {
                    return true;
                }
            }
            
            return canNegate;
        }
        
        // right side
        bool canNegate = false;
        for (int i = bishopIndex+9; i < 64; i+=9) {
            
            if (BoardUtils.IndexToFile(i) - BoardUtils.IndexToFile(bishopIndex) == 1) {
                canNegate = CheckValidBits(i, canNegate);

            }
        }
        

        // left side
        canNegate = false;
        for (int i = bishopIndex+7; i < 64; i+=7) {
            
            if (BoardUtils.IndexToFile(i) - BoardUtils.IndexToFile(bishopIndex) == 1) {
                canNegate = CheckValidBits(i, canNegate);

            }
        }
        

        // bottom left side
        canNegate = false;
        for (int i = bishopIndex-9; i >= 0; i-=9) {
            
            if (BoardUtils.IndexToFile(bishopIndex) - BoardUtils.IndexToFile(i) == 1) {
                canNegate = CheckValidBits(i, canNegate);

            }
        }
        

        // bottom right side
        canNegate = false;
        for (int i = bishopIndex-7; i >= 0; i-=7) {
            
            if (BoardUtils.IndexToFile(bishopIndex) - BoardUtils.IndexToFile(i) == 1) {
                canNegate = CheckValidBits(i, canNegate);

            }
        }
        

        return validBoard;
    }
    
    public static ulong GetRookMovesFromBlockers(ulong rookMovementMask, ulong blockersBitboard, int rookIndex)
    {
        ulong validBoard = rookMovementMask & (~blockersBitboard);
        
        
        bool CheckValidBits(int i, bool canNegate)
        {
            if (canNegate)
            {
                validBoard = BitboardUtils.negateBit(validBoard, i);
            }
            else
            {
                if (BitboardUtils.isBitOff(validBoard, i))
                {
                    validBoard = BitboardUtils.enableBit(validBoard, i);
                    return true;
                }
            }
            
            return canNegate;
        }
        
        // right side
        bool canNegate = false;
        for (int i = rookIndex; i < rookIndex + rookIndex%8; i++) {
            
            if (BoardUtils.IndexToFile(i) == BoardUtils.IndexToFile(rookIndex) & i != rookIndex) {
                canNegate = CheckValidBits(i, canNegate);
            }                
        }
        
        // left side 
        canNegate = false;
        for (int i = rookIndex; i >= rookIndex - rookIndex%8; i--) {   

            if (BoardUtils.IndexToFile(i) == BoardUtils.IndexToFile(rookIndex) & i != rookIndex) {
                canNegate = CheckValidBits(i, canNegate);
            }
        }
        
        // up
        canNegate = false;
        for (int i = rookIndex+8; i < 64; i+=8) {   
            canNegate = CheckValidBits(i, canNegate);
            
        }
        
        // down
        canNegate = false;
        for (int i = rookIndex-8; i >= 0; i-=8) {   
            canNegate = CheckValidBits(i, canNegate);
            
        }
        

        return validBoard;
    }

    
}