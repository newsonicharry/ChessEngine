namespace ChessGame.bitboards;

public class Bitboards
{
    public static ulong WhiteRookBitboard;
    public static ulong WhiteBishopBitboard;
    public static ulong WhitePawnBitboard;
    public static ulong WhiteKnightBitboard;
    public static ulong WhiteKingBitboard;
    public static ulong WhiteQueenBitboard;
    
    public static ulong BlackRookBitboard;
    public static ulong BlackBishopBitboard;
    public static ulong BlackPawnBitboard;
    public static ulong BlackKnightBitboard;
    public static ulong BlackKingBitboard;
    public static ulong BlackQueenBitboard;
    
    public static void LoadBitboardsFromFen(string fen) {

        // only the board position
        string rawFen = fen.Split(" ")[0];

        string[] fenSplits = rawFen.Split("/");
        
        int index = 0;
        foreach (string column in fenSplits)
        {
            foreach (char piece in column)
            {
                if (char.IsDigit(piece))
                {   
                    index += piece - '0';
                    continue;
                }
                
                if (piece == 'P') { WhitePawnBitboard |= 1ul << Math.Abs(63-BitboardUtils.ConvertToBitboardIndex(index)); }
                if (piece == 'B') { WhiteBishopBitboard |= 1ul << Math.Abs(63-BitboardUtils.ConvertToBitboardIndex(index)); }
                if (piece == 'N') { WhiteKnightBitboard |= 1ul << Math.Abs(63-BitboardUtils.ConvertToBitboardIndex(index)); }
                if (piece == 'R') { WhiteRookBitboard |= 1ul << Math.Abs(63-BitboardUtils.ConvertToBitboardIndex(index)); }
                if (piece == 'Q') { WhiteQueenBitboard |= 1ul << Math.Abs(63-BitboardUtils.ConvertToBitboardIndex(index)); }
                if (piece == 'K') { WhiteKingBitboard |= 1ul << Math.Abs(63-BitboardUtils.ConvertToBitboardIndex(index)); }

                if (piece == 'p') { BlackPawnBitboard |= 1ul << Math.Abs(63-BitboardUtils.ConvertToBitboardIndex(index)); }
                if (piece == 'b') { BlackBishopBitboard |= 1ul << Math.Abs(63-BitboardUtils.ConvertToBitboardIndex(index)); }
                if (piece == 'n') { BlackKnightBitboard |= 1ul << Math.Abs(63-BitboardUtils.ConvertToBitboardIndex(index)); }
                if (piece == 'r') { BlackRookBitboard |= 1ul << Math.Abs(63-BitboardUtils.ConvertToBitboardIndex(index)); }
                if (piece == 'q') { BlackQueenBitboard |= 1ul << Math.Abs(63-BitboardUtils.ConvertToBitboardIndex(index)); }
                if (piece == 'k') { BlackKingBitboard |= 1ul << Math.Abs(63-BitboardUtils.ConvertToBitboardIndex(index)); }

                index += 1;
            
            }
            
            
            
        }
    }
    
    
}