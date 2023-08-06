using ChessGame.board;

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
        string[] fenFields = fen.Split(" ");

        string activeColor = fenFields[1];
        if (activeColor == "b") { Board.SwitchCurrentPlayerTurn(); }

        string castlingRights = fenFields[2];
        foreach (char castlingSymbol in castlingRights){
            if (castlingSymbol == 'K') { Castling.FenWhiteCastleKingSide = true; }
            if (castlingSymbol == 'Q') { Castling.FenWhiteCastleQueenSide = true; }
            if (castlingSymbol == 'k') { Castling.FenBlackCastleKingSide = true; }
            if (castlingSymbol == 'q') { Castling.FenBlackCastleQueenSide = true; }

        }

        string enPassantTargets = fenFields[3];
        if (enPassantTargets != "-")
        {   
            int xCord = enPassantTargets[0] - 'a';
            int yCord = (enPassantTargets[1] - '1');
            int enPassantTargetIndex = yCord * 8 + xCord;
            
            Console.WriteLine(xCord);
            Console.WriteLine(yCord);

            
            if (Board.IsWhite)
            {
                Board.WhiteDoubleMovedPawnIndex = enPassantTargetIndex - 8;
            }
            else
            {
                Board.BlackDoubleMovedPawnIndex = enPassantTargetIndex + 8;
            }
        }

        string halfMoveClock = fenFields[4];
        string fullMoveClock = fenFields[5];

        Board.HalfMoveClock = Convert.ToInt32(halfMoveClock);
        Board.FullMoveClock = Convert.ToInt32(fullMoveClock);

        
        
        string[] fenSplits = fenFields[0].Split("/");
        
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