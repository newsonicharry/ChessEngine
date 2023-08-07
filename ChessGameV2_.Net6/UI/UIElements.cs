using System.Numerics;
using ChessGame.board;
using ChessGame.bitboards;
using Raylib_cs;
using static learnraylib.UIConstants;
using ChessGame;

namespace learnraylib;

public class UIElements
{   
    

    private static readonly int[][] SquarePositions = FindSquarePositions();
    
    private static int _currentSquareSelected = -1;
    private static bool _pieceSelected = false;
    private static Texture2D _pieceSelectedTexture = new Texture2D();
    private static ulong _pieceBitboard = 0ul;
    private static List<int> _occupiedSquares = new List<int>();

    private static ulong[] _validMoves = ValidMoves.FindValidMoves();
    
    
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
    
    private static int CheckIfMouesOnPiece()
    {
        int i = 0;
        foreach (int[] posData in SquarePositions)
        {
            int xPos = posData[0];
            int yPos = posData[1];
                
            if (Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), new Rectangle(xPos, yPos, SideLength, SideLength)) & _occupiedSquares.Contains(i))
            {
                return i;
            }

            i++;
        }

        return -1;
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
                        if (BitboardUtils.isBitOn(_validMoves[_currentSquareSelected], newSquareIndex))
                        {
                            Board.UpdateBoard(_pieceBitboard, _currentSquareSelected, newSquareIndex);
                            Board.SwitchCurrentPlayerTurn();
                            
                            _validMoves = ValidMoves.FindValidMoves();

                        }

                    }
                    
                }
            
                newSquareIndex++;

            }
            
            _currentSquareSelected = -1;
            _pieceBitboard = 0ul;
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
        string[] imageFiles = Directory.GetFiles("../../../Pieces");

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
                    if (BitboardUtils.isBitOn(_validMoves[_currentSquareSelected], index)) { customColor = true;}
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
        

        _occupiedSquares.Clear();

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
                        
                        _occupiedSquares.Add(index);

                        if (index == _currentSquareSelected & Raylib.IsMouseButtonDown(MouseButton.MOUSE_LEFT_BUTTON))
                        {
                            Color selectedColor = Color.WHITE;
                            selectedColor.a = 50;
                            Raylib.DrawTexture(allPieceTextures[bitboardIndex], posX, posY, selectedColor);
                            
                            _pieceSelectedTexture = allPieceTextures[bitboardIndex];
                            _pieceBitboard = allBitboards[bitboardIndex];

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
