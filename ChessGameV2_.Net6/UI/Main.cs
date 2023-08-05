using ChessGame;
using ChessGame.board;
using Raylib_cs;
using static learnraylib.UIConstants;


// if you cant tell, im not so great with ui code
// this thing is a sloppy mess

namespace learnraylib
{
    static class Program
    {
        
        public static void Main()
        {
            Raylib.InitWindow(Width, Height, "Chess Engine");
            Raylib.SetTargetFPS(144);

            Color backgroundColor = new Color((byte)31, (byte)31, (byte)31, (byte)255);

            Texture2D[] allPieceTextures = UIElements.GetPieceTextures();


            StartEngine.Start();

            while (!Raylib.WindowShouldClose())
            {

                Raylib.BeginDrawing();
                Raylib.ClearBackground(backgroundColor);

                if (!Board.GameOver)
                {
                    UIElements.DrawBoard();
                    UIElements.DrawPiecesFromBitboards(allPieceTextures);
                    UIElements.MovePiece();   
                }
                
                

                Raylib.EndDrawing();

            }

            Raylib.CloseWindow();
        }

    }
}
