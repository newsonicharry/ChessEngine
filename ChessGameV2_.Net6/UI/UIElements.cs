using System.Numerics;
using ChessGame.board;
using ChessGame.bitboards;
using Raylib_cs;
using static learnraylib.UIConstants;

namespace learnraylib;

public class UIElements
{   
    

    private static readonly int[][] SquarePositions = FindSquarePositions();
    
    private static int _currentSquareSelected = -1;
    private static bool _pieceSelected = false;
    private static Texture2D _pieceSelectedTexture = new Texture2D();
    private static ulong _pieceBitboard = 0ul;
    private static List<int> _occupiedSquares = new List<int>();

    private static int[][] _validMoves = Board.FindValidMoves();
    
    
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

    public static void MovePiece()
    {

        // checks if the square should be highlighted
        if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_RIGHT_BUTTON))
        {
            _currentSquareSelected = -1;
        }
        
        
        if (!Raylib.IsMouseButtonDown(MouseButton.MOUSE_LEFT_BUTTON) & _pieceSelected)
        {

            int index = 0;
            foreach (int[] squarePosition in SquarePositions)
            {
                if (Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), new Rectangle(squarePosition[0], squarePosition[1], SideLength, SideLength)))
                {
                    
                    if (_currentSquareSelected != index)
                    {
                        foreach (int[] validMove in _validMoves)
                        {
                            if (validMove[0] == _currentSquareSelected & validMove[1] == index)
                            {   
                                Board.UpdateBitboards(_pieceBitboard, _currentSquareSelected, index);
                                Board.SwitchCurrentPlayerTurn();
                                _validMoves = Board.FindValidMoves();

                            }
                            
                        }
                        
                    }
                    

                }
            
                index++;

            }
            
            _currentSquareSelected = -1;
            _pieceBitboard = 0ul;
            _pieceSelectedTexture = new Texture2D();

        }
            

        if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_LEFT_BUTTON))
        {
            Vector2 mousePosition = Raylib.GetMousePosition();

            int i = 0;
            foreach (int[] posData in SquarePositions)
            {
                int xPos = posData[0];
                int yPos = posData[1];
                
                if (Raylib.CheckCollisionPointRec(mousePosition, new Rectangle(xPos, yPos, SideLength, SideLength)) & _occupiedSquares.Contains(i))
                {
                    _currentSquareSelected = i;
                }

                i++;
            }

        }
        _pieceSelected = Raylib.IsMouseButtonDown(MouseButton.MOUSE_LEFT_BUTTON);

        if (_pieceSelected)
        {
            Raylib.DrawTexture(_pieceSelectedTexture, Raylib.GetMouseX()-SideLength/2, Raylib.GetMouseY()-SideLength/2, Color.WHITE);
        }
        
    }
    public static Texture2D[] GetPieceTextures()
    {
        string[] imageFiles = Directory.GetFiles("../../../Pieces");

        List<Texture2D> allPieceTextures = new List<Texture2D>();
        
        
        foreach (string imagePath in imageFiles)
        {

            unsafe
            {   
                Image image = Raylib.LoadImage(imagePath);
                Raylib.ImageResize(&image, SideLength, SideLength);
                Texture2D texture = Raylib.LoadTextureFromImage(image);
                Raylib.UnloadImage(image);
            
                allPieceTextures.Add(texture);
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
                
                
                foreach (int[] validMove in _validMoves) {   
                    if (validMove[0] == _currentSquareSelected & validMove[1] == index) { customColor = true; }
                }
                if (index == _currentSquareSelected) { customColor = true;}
                
                
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


        Texture2D[] allBitboardPieceTextures =
        {
            allPieceTextures[9], allPieceTextures[7], allPieceTextures[5], allPieceTextures[6],
            allPieceTextures[3], allPieceTextures[10], allPieceTextures[4], allPieceTextures[11],
            allPieceTextures[1], allPieceTextures[8], allPieceTextures[2], allPieceTextures[0]
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
                            Raylib.DrawTexture(allBitboardPieceTextures[bitboardIndex], posX, posY, selectedColor);
                            
                            _pieceSelectedTexture = allBitboardPieceTextures[bitboardIndex];
                            _pieceBitboard = allBitboards[bitboardIndex];

                        }
                        
                        if (index != _currentSquareSelected)
                        {
                            Raylib.DrawTexture(allBitboardPieceTextures[bitboardIndex], posX, posY, Color.WHITE);

                        }
                        
                        
                    }

                    index--;

                }

            }
        }

    }
}
