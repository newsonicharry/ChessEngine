using ChessEngine.board;
using ChessEngine.Board;

namespace ChessEngine.Engine;

public abstract class Transpositions
{

    public static IDictionary<int, int> TranspositionTable = new Dictionary<int, int>();
    
    private static int[,] ZobristKeys = new int[13, 64];
    private static int SideToMove = new int();
    private static readonly int[] CastlingRights = new int[16];
    private static readonly int[] EnPassantFile = new int[8];

    public static int ZobristHash;



    public static void UpdateZobristHash(int movingPiece, int originalSquare, int capturedPiece, int newSquare)
    {
        ZobristHash ^= ZobristKeys[movingPiece, originalSquare];  // XOR out the moving piece
        if (capturedPiece != PieceHelper.Empty){
            ZobristHash ^= ZobristKeys[capturedPiece, newSquare]; // XOR out the captured piece
        }
        
        ZobristHash ^= ZobristKeys[movingPiece, newSquare]; // XOR in the moved piece
    }
    
    public static void LoadZobrist()
    {
        int zobristHash = 0;

        for (int piece = 0; piece < 12; piece++)
        {
            for (int square = 0; square < 64; square++)
            {
                if (PieceHelper.PieceArray[square] != PieceHelper.Empty)
                {
                    zobristHash ^= ZobristKeys[piece, square];
                }
            }
        }

        if (!board.Board.IsWhite)
        {
            zobristHash ^= SideToMove;
        }

        int castlingRightsIndex = -1;
        
        if (Castling.WhiteCastleKingSide){castlingRightsIndex += 1; }
        if (Castling.WhiteCastleQueenSide){castlingRightsIndex += 3;}
        if (Castling.BlackCastleKingSide){castlingRightsIndex += 5;}
        if (Castling.BlackCastleQueenSide){castlingRightsIndex += 7;}

        if (castlingRightsIndex != -1)
        {
            zobristHash ^= CastlingRights[castlingRightsIndex];
        }

        if (board.Board.IsWhite & board.Board.BlackDoubleMovedPawnIndex != 0)
        {
            zobristHash ^= EnPassantFile[BoardUtils.IndexToFile(board.Board.BlackDoubleMovedPawnIndex)];
        }
        
        if (!board.Board.IsWhite & board.Board.WhiteDoubleMovedPawnIndex != 0)
        {
            zobristHash ^= EnPassantFile[BoardUtils.IndexToFile(board.Board.WhiteDoubleMovedPawnIndex)];
        }

        ZobristHash = zobristHash;
    }
    
    public static void InitializeZobristKeys()
    {
        int[,] zobristKeys = new int[13, 64];
        
        Random random = new Random(12345);

        for (int piece = 0; piece < 13; piece++)
        {
            for (int square = 0; square < 64; square++)
            {
                zobristKeys[piece, square] = random.Next();
            }
        }
        
        ZobristKeys = zobristKeys;

        SideToMove = random.Next();

        for (int i = 0; i < 16; i++){
            CastlingRights[i] = random.Next();
        }
        
        for (int i = 0; i < 16; i++){
            CastlingRights[i] = random.Next();
        }

        for (int i = 0; i < 8; i++)
        {
            EnPassantFile[i] = random.Next();
        }


    }
    
    
}




