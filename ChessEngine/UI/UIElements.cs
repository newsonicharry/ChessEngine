using System.Diagnostics;
using System.Numerics;
using ChessEngine.bitboard;
using ChessEngine.Board;
using ChessEngine.Engine;
using Raylib_cs;
using static ChessEngine.UI.UIConstants;

namespace ChessEngine.UI;

public abstract class UiElements
{   
    

    public static readonly int[][] SquarePositions = FindSquarePositions();
    
    private static int _currentSquareSelected = -1;
    private static bool _pieceSelected;
    private static Texture2D _pieceSelectedTexture;
    private static readonly List<int> OccupiedSquares = new();

    public static ushort[] ValidMoves = Board.ValidMoves.FindValidMoves();



    private static int[][] FindDecodedValidMove()
    {
        List<int[]> validMovesIndexes = new List<int[]>();
        
        foreach (ushort validMove in ValidMoves)
        {   
            (int startingSquare, int endingSquare, int pieceIndex) = BoardUtils.DecodeMove(validMove);
            validMovesIndexes.Add(new []{startingSquare, endingSquare, pieceIndex});
        }

        return validMovesIndexes.ToArray();
    }
    
    
    private static int[][] FindSquarePositions()
    {
        int[][] squarePositions = new int[64][];
        
        for (int i = 0; i < 64; i++)
        {
            int xPos = BoardUtils.IndexToRank(i) * SideLength + InitialXOffset;
            int yPos = (7-BoardUtils.IndexToFile(i)) * SideLength + InitialYOffset;

            squarePositions[i] = new[] { xPos, yPos };

        }

        return squarePositions;
    }
    
    public static int CheckIfMouesOnPiece()
    {
        int i = 0;
        foreach (int[] posData in SquarePositions)
        {
            int xPos = posData[0];
            int yPos = posData[1];
                
            if (Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), new Rectangle(xPos, yPos, SideLength, SideLength)) & OccupiedSquares.Contains(i))
            {
                return i;
            }

            i++;
        }

        return -1;
    }
    
    public static void WriteVersion(int majorVersion, int minorVersion, int patchVersion, Font font)
    {
        string version = $"Ver {majorVersion}.{minorVersion}.{patchVersion}";

        Vector2 textPosition = new Vector2(0, 0);
        
        Raylib.DrawTextPro(font, version, textPosition, textPosition, 0f, 20, 4, Color.GRAY);
    }
    public static void MovePiece()
    {

        
        if (!Raylib.IsMouseButtonDown(MouseButton.MOUSE_LEFT_BUTTON) & _pieceSelected)
        {


            int newSquareIndex = 0;
            foreach (int[] squarePosition in SquarePositions)
            {
                if (Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), new Rectangle(squarePosition[0], squarePosition[1], SideLength, SideLength)))
                {   
                    if (_currentSquareSelected != newSquareIndex & _currentSquareSelected != -1)
                    {
                        int[][] validMovesDecoded = FindDecodedValidMove();
                        if (validMovesDecoded.Length != 0)
                        {
                            int i = 0;
                            foreach (int[] moveData in validMovesDecoded)
                            {
                                if (newSquareIndex == moveData[1] & _currentSquareSelected == moveData[0])
                                {
                                    
                                    board.Board.UpdateBoard(ValidMoves[i]);
                                    
                                    Stopwatch stopwatch = new Stopwatch();
                                    stopwatch.Start();
                                    
                                    Engine.Engine.FindBestMove();
                                    
                                    stopwatch.Stop();
                                    Console.WriteLine(stopwatch.ElapsedMilliseconds);
                                    
                                }

                                i++;
                            }
                            
                            
                            
                            ValidMoves = Board.ValidMoves.FindValidMoves();

                        }

                    }
                    
                }
            
                newSquareIndex++;

            }
            
            _currentSquareSelected = -1;
            _pieceSelectedTexture = new Texture2D();

        }
            

        if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_LEFT_BUTTON))
        {
            int pieceSelected = CheckIfMouesOnPiece();
            if (pieceSelected != -1)
            {
                _currentSquareSelected = pieceSelected;
                _pieceSelected = true;
            }
        }


        if (_pieceSelected)
        {
            Raylib.DrawTexture(_pieceSelectedTexture, Raylib.GetMouseX()-SideLength/2, Raylib.GetMouseY()-SideLength/2, Color.WHITE);
        }
        
    }
    
    public static Texture2D[] GetPieceTextures()
    {
        string[] imageFiles = Directory.GetFiles("./Pieces");


        Texture2D[] allPieceTextures = new Texture2D[12];
        
        
        foreach (string imagePath in imageFiles)
        {

            unsafe
            {   
                Image image = Raylib.LoadImage(imagePath);
                Raylib.ImageResize(&image, SideLength, SideLength);
                Texture2D texture = Raylib.LoadTextureFromImage(image);
                Raylib.UnloadImage(image);

                string filename = imagePath.Split("\\")[1];            
                
                if (filename == "whitepawn.png") { allPieceTextures[0] = texture; }
                if (filename == "whiteknight.png") { allPieceTextures[1] = texture; }
                if (filename == "whitebishop.png") { allPieceTextures[2] = texture; }
                if (filename == "whiterook.png") { allPieceTextures[3] = texture; }
                if (filename == "whitequeen.png") { allPieceTextures[4] = texture; }
                if (filename == "whiteking.png") { allPieceTextures[5] = texture; }
                if (filename == "blackpawn.png") { allPieceTextures[6] = texture; }
                if (filename == "blackknight.png") { allPieceTextures[7] = texture; }
                if (filename == "blackbishop.png") { allPieceTextures[8] = texture; }
                if (filename == "blackrook.png") { allPieceTextures[9] = texture; }
                if (filename == "blackqueen.png") { allPieceTextures[10] = texture; }
                if (filename == "blackking.png") { allPieceTextures[11] = texture; }

                
            }
            

        }

        return allPieceTextures.ToArray();
    }
    
    public static void DrawBoard()
    {

        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {   
                
                Color currentColor;
                bool customColor = false;
                int index = (7-y) * 8 + x;


                if (_currentSquareSelected != -1)
                {   
                    int[][] validDecodedMoves = FindDecodedValidMove();
                    foreach (int[] validMoveData in validDecodedMoves)
                    {       
                        if (index == validMoveData[1] & _currentSquareSelected == validMoveData[0])
                        {
                            customColor = true;
                        }
                    }
                    
                    if (index == _currentSquareSelected) { customColor = true;}
                }
                
                
                
                // lighter
                if (Math.Abs(y-x) % 2 == 0){
                    
                    currentColor = new Color((byte)235, (byte)210, (byte)184, (byte)255);
                    if (customColor) {   
                        currentColor = new Color((byte)37, (byte)150, (byte)190, (byte)255);
                    }

                }
                
                else
                {
                    currentColor = new Color((byte)161, (byte)111, (byte)90, (byte)255);
                    if (customColor) {   
                        currentColor = new Color((byte)14, (byte)120, (byte)172, (byte)255);

                    }

                }
                
                int xPos = x * SideLength + InitialXOffset;
                int yPos = y * SideLength + InitialYOffset;
                

                Raylib.DrawRectangle(xPos, yPos, SideLength, SideLength, currentColor);

                // squarePositions[index] = new[] { xPos, yPos };


            }
        }

    }

    public static void DrawPiecesFromBitboards(Texture2D[] allPieceTextures)
    {   
        
        ulong[] allBitboards = 
        {
            Bitboards.WhitePawnBitboard, Bitboards.WhiteKnightBitboard, Bitboards.WhiteBishopBitboard,
            Bitboards.WhiteRookBitboard, Bitboards.WhiteQueenBitboard, Bitboards.WhiteKingBitboard,
            Bitboards.BlackPawnBitboard, Bitboards.BlackKnightBitboard, Bitboards.BlackBishopBitboard,
            Bitboards.BlackRookBitboard, Bitboards.BlackQueenBitboard, Bitboards.BlackKingBitboard
        };
        

        OccupiedSquares.Clear();

        for (int bitboardIndex = 0; bitboardIndex < 12; bitboardIndex++)
        {
            string strBitboard = BitboardUtils.ConvertULongToBinaryString(allBitboards[bitboardIndex]);

            string[] bitboardRows = new string[8];

            for (int i = 0; i < 8; i++)
            {
                bitboardRows[i] = strBitboard[(i * 8)..(i * 8 + 8)];
            }


            int index = 63;
            foreach (string bitboardRow in bitboardRows)
            {

                foreach (char bit in bitboardRow)
                {  

                    if (bit == '1')
                    {

                        int posX = SquarePositions[index][0];
                        int posY = SquarePositions[index][1];
                        
                        OccupiedSquares.Add(index);

                        if (index == _currentSquareSelected & Raylib.IsMouseButtonDown(MouseButton.MOUSE_LEFT_BUTTON))
                        {
                            Color selectedColor = Color.WHITE;
                            selectedColor.a = 50;
                            Raylib.DrawTexture(allPieceTextures[bitboardIndex], posX, posY, selectedColor);
                            
                            _pieceSelectedTexture = allPieceTextures[bitboardIndex];
                        }
                        
                        if (index != _currentSquareSelected)
                        {
                            Raylib.DrawTexture(allPieceTextures[bitboardIndex], posX, posY, Color.WHITE);

                        }
                        
                        
                    }

                    index--;

                }

            }
        }

    }
}
