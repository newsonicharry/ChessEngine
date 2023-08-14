using System.Diagnostics;
using ChessEngine.Engine;
using Raylib_cs;
using static ChessEngine.UI.UIConstants;


// if you cant tell, im not so great with ui code
// this thing is a sloppy mess

namespace ChessEngine.UI
{
    static class Program
    {

        private static bool RuuningEngine = false;
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

                UiElements.DrawBoard();

                UiElements.DrawPiecesFromBitboards(allPieceTextures);
                UiElements.MovePiece();

                UiDebug.AddPieceToBoard();
                

                
                UiElements.WriteVersion(1, 2, 1, font);

                

                Raylib.EndDrawing();

            }

            Raylib.CloseWindow();
        }

    }
}
