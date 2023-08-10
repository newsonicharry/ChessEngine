using ChessEngine.Engine;
using Raylib_cs;
using static ChessEngine.UI.UIConstants;


// if you cant tell, im not so great with ui code
// this thing is a sloppy mess

namespace ChessEngine.UI
{
    static class Program
    {
        public static void Main()
        {   

            Raylib.InitWindow(Width, Height, "Chess Engine");
            Raylib.SetTargetFPS(144);
            
        
            Color backgroundColor = new Color((byte)31, (byte)31, (byte)31, (byte)255);

            Texture2D[] allPieceTextures = UiElements.GetPieceTextures();
            
            Font font = Raylib.LoadFont("./Roboto-Black.ttf");
            
            StartEngine.Start();
        
            while (!Raylib.WindowShouldClose())
            {

                Raylib.BeginDrawing();
                Raylib.ClearBackground(backgroundColor);

                UiDebug.AddPieceToBoard();
                
                UiElements.DrawBoard();
                UiElements.DrawPiecesFromBitboards(allPieceTextures);

                if (!board.Board.GameOver)
                {   
                    UiElements.MovePiece();
                    // Engine.Engine.MakeMove();
                    // Board.Board.SwitchCurrentPlayerTurn();
                }
                
                UiElements.WriteVersion(1, 0, 5, font);

                

                Raylib.EndDrawing();

            }

            Raylib.CloseWindow();
        }

    }
}
