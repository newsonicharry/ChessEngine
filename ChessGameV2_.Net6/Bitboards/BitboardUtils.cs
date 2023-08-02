namespace ChessGame.bitboards;
using ChessGame.board;
public class BitboardUtils
{
    public static void PrintBitboards(ulong value)
    {
        for (int row = 7; row >= 0; row--)
        {
            for (int col = 0; col < 8; col++)
            {
                ulong mask = 1ul << (row * 8 + col);
                char bitChar = (value & mask) != 0 ? '1' : '0';
                Console.Write(bitChar + " ");
            }
            Console.Write("  " + row);
            Console.WriteLine();
        }
        Console.WriteLine();
        Console.WriteLine("0 1 2 3 4 5 6 7");
    }
    
    public static string ConvertULongToBinaryString(ulong value)
    {
        string binaryString = Convert.ToString((long)value, 2).PadLeft(64, '0');
        return binaryString;
    }
    
    public static ulong NegateBit(ulong bitboard, int index)
    {
        ulong bitmask = (ulong)1 << index;
        bitmask = ~bitmask;
        bitboard &= bitmask;

        return bitboard;
    }
    
    
    public static ulong EnableBit(ulong bitboard, int index)
    {
        ulong bitmask = (ulong)1 << index;
        bitboard |= bitmask;

        return bitboard;
    }

    public static bool isBitOn(ulong bitboard, int index)
    {
        ulong bitmask = (ulong)1 << index;
        bool isBitOn = (bitboard & bitmask) != 0;

        return isBitOn;
    }
    
    public static bool IsBitOff(ulong bitboard, int index)
    {
        ulong bitmask = (ulong)1 << index;
        bool isBitZero = (bitboard & bitmask) == 0;

        return isBitZero;
    }
    
    public static int ConvertToBitboardIndex(int index)
    {
        return (7-BoardUtils.IndexToRank(index))+BoardUtils.IndexToFile(index)*8;
    }
    
    public static int FindLsb(ulong bits)
    {
        // Check if the input is zero, which means no bit is set
        if (bits == 0)
            return -1; // Return a value to indicate no bit is set

        // Use a loop to find the index of the least significant bit (LSB)
        int index = 0;
        while ((bits & 1) == 0)
        {
            bits >>= 1;
            index++;
        }

        return index;
    }


    public static int[] GetSetBitIndexes(ulong number)
    {
        List<int> setBitIndexes = new List<int>();
        int bitIndex = 0;

        while (number > 0)
        {
            if ((number & 1UL) == 1UL) // Check if the least significant bit is set (i.e., equal to 1)
            {
                setBitIndexes.Add(bitIndex);
            }

            bitIndex++;
            number >>= 1; // Right shift the number by 1 bit to check the next bit
        }

        return setBitIndexes.ToArray();
    }
    
    public static ulong ChangeBitPosition(ulong bitboard, int originalIndex, int newIndex)
    {
        ulong newBitboard = BitboardUtils.NegateBit(bitboard, originalIndex);
        newBitboard = BitboardUtils.EnableBit(newBitboard, newIndex);
            
        return newBitboard;
    }
    
}