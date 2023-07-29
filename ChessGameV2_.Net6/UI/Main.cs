using System.Numerics;
using ChessGame;
using Raylib_cs;
using ChessGame.bitboards;
using ChessGame.board;


// if you cant tell, im not so great with ui code
// this thing is a sloppy mess

namespace learnraylib
{
    static class Program
    {

        private const int Width = 1080;
        private const int Height = 720;
        private const int SideLength = 75;

        private const int InitialXOffset = Width/2-(SideLength*8/2);
        private const int InitialYOffset = Height/2-(SideLength*8/2);
        
        
        private static int[][] squarePositions = new int[64][];

        private static int CurrentSquareSelected = -1;
        private static bool PieceSelected = false;
        private static Texture2D PieceSelectedTexture = new Texture2D();
        private static ulong PieceBitboard = 0ul;
        private static List<int> OccupiedSquares = new List<int>();
        
        
        public static void Main()
        {
            Raylib.InitWindow(Width, Height, "Chess Engine");
            Raylib.SetTargetFPS(144);
            
            Color backgroundColor = new Color((byte)31, (byte)31, (byte)31, (byte)255);

            Texture2D[] allPieceTextures = GetPieceTextures();
            
            
            StartEngine.Start();
            
            FindSquarePositions();

            Board.FindValidMoves(true);
            
            while (!Raylib.WindowShouldClose())
            {
                
                Raylib.BeginDrawing();
                Raylib.ClearBackground(backgroundColor);
                
                DrawBoard();
                DrawPiecesFromBitboards(allPieceTextures);
                MovePiece();

                
                
                
                Raylib.EndDrawing();

            }

            Raylib.CloseWindow();
        }


        private static void FindSquarePositions()
        {
            for (int i = 0; i < 64; i++)
            {
                int xPos = BoardUtils.IndexToRank(i) * SideLength + InitialXOffset;
                int yPos = (7-BoardUtils.IndexToFile(i)) * SideLength + InitialYOffset;

                squarePositions[i] = new[] { xPos, yPos };

            }
        }

        private static void MovePiece()
        {

            // checks if the square should be highlighted
            if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_RIGHT_BUTTON))
            {
                CurrentSquareSelected = -1;
            }
            
            
            if (!Raylib.IsMouseButtonDown(MouseButton.MOUSE_LEFT_BUTTON) & PieceSelected)
            {

                int index = 0;
                foreach (int[] squarePosition in squarePositions)
                {
                    if (Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), new Rectangle(squarePosition[0], squarePosition[1], SideLength, SideLength)))
                    {   
                        Console.WriteLine(CurrentSquareSelected);
                        Console.WriteLine(index);
                        Console.WriteLine();

                        
                        if (CurrentSquareSelected != index)
                        {   

                            // Console.WriteLine("true");
                            int[][] validMoves = Board.FindValidMoves(true);
                            
                            foreach (int[] validMove in validMoves)
                            {   
                                
                                // Console.WriteLine(validMove[0] + ", " + validMove[1]);
                                if (validMove[0] == CurrentSquareSelected & validMove[1] == index)
                                {
                                    Board.UpdateBitboards(PieceBitboard, CurrentSquareSelected, index);

                                }
                                
                            }
                            
                        }
                        

                    }
                
                    index++;

                }
                
                CurrentSquareSelected = -1;
                PieceBitboard = 0ul;
                
            }
            

            if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_LEFT_BUTTON))
            {
                Vector2 mousePosition = Raylib.GetMousePosition();

                int i = 0;
                foreach (int[] posData in squarePositions)
                {
                    int xPos = posData[0];
                    int yPos = posData[1];
                    
                    if (Raylib.CheckCollisionPointRec(mousePosition, new Rectangle(xPos, yPos, SideLength, SideLength)) & OccupiedSquares.Contains(i))
                    {
                        CurrentSquareSelected = i;
                    }

                    i++;
                }

            }
            PieceSelected = Raylib.IsMouseButtonDown(MouseButton.MOUSE_LEFT_BUTTON);

            if (PieceSelected)
            {
                Raylib.DrawTexture(PieceSelectedTexture, Raylib.GetMouseX()-SideLength/2, Raylib.GetMouseY()-SideLength/2, Color.WHITE);
            }
            
        }
        private static Texture2D[] GetPieceTextures()
        {
            string[] imageFiles = Directory.GetFiles("/home/harry/Desktop/RiderProjects/ChessGameV2_.Net6/ChessGameV2_.Net6/Pieces");
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
        private static void DrawBoard()
        {

            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {   
                    
                    Color currentColor;
                    int index = (7-y) * 8 + x;

                    if (Math.Abs(y-x) % 2 == 0)
                        currentColor = new Color((byte)235, (byte)210, (byte)184, (byte)255);
                    else
                    {
                        currentColor = new Color((byte)161, (byte)111, (byte)90, (byte)255);
                    }

                    if (index == CurrentSquareSelected) {   
                        currentColor = new Color((byte)37, (byte)150, (byte)190, (byte)255);
                        // currentColor = new Color((byte)248, (byte)236, (byte)90, (byte)255);
                    }
                    
                    
                    int xPos = x * SideLength + InitialXOffset;
                    int yPos = y * SideLength + InitialYOffset;
                    

                    Raylib.DrawRectangle(xPos, yPos, SideLength, SideLength, currentColor);

                    // squarePositions[index] = new[] { xPos, yPos };


                }
            }

        }

        private static void DrawPiecesFromBitboards(Texture2D[] allPieceTextures)
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

                            int posX = squarePositions[index][0];
                            int posY = squarePositions[index][1];
                            
                            // OccupiedSquares.Add((7-BoardUtils.IndexToRank(index)) * BoardUtils.IndexToFile(index));
                            OccupiedSquares.Add(index);

                            if (index == CurrentSquareSelected & Raylib.IsMouseButtonDown(MouseButton.MOUSE_LEFT_BUTTON))
                            {
                                Color selectedColor = Color.WHITE;
                                selectedColor.a = 50;
                                Raylib.DrawTexture(allBitboardPieceTextures[bitboardIndex], posX, posY, selectedColor);
                                
                                PieceSelectedTexture = allBitboardPieceTextures[bitboardIndex];
                                PieceBitboard = allBitboards[bitboardIndex];

                            }
                            
                            if (index != CurrentSquareSelected)
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
}
