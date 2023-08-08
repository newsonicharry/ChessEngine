using ChessEngine.Engine;
using Raylib_cs;
using static ChessEngine.UI.UIConstants;
using ChessEngine.Board;


// if you cant tell, im not so great with ui code
// this thing is a sloppy mess

namespace ChessEngine.UI
{
    static class Program
    {
        public static void Main()
        {   
            
            
            Raylib.InitWindow(Width, Height, "Chess Engine");
            // Raylib.SetTargetFPS(144);

            Color backgroundColor = new Color((byte)31, (byte)31, (byte)31, (byte)255);

            Texture2D[] allPieceTextures = UiElements.GetPieceTextures();


            StartEngine.Start();
        
            while (!Raylib.WindowShouldClose())
            {

                Raylib.BeginDrawing();
                Raylib.ClearBackground(backgroundColor);
                
                UiElements.DrawBoard();
                UiElements.DrawPiecesFromBitboards(allPieceTextures);

                if (!Board.Board.GameOver)
                {   
                    UiElements.MovePiece();
                    // Engine.Engine.MakeMove();
                    // Board.Board.SwitchCurrentPlayerTurn();
                }
                
                

                Raylib.EndDrawing();

            }

            Raylib.CloseWindow();
        }

    }
}
