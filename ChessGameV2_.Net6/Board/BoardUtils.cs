namespace ChessGame.board;

public class BoardUtils
{
    public static int IndexToFile(int index) 
    {
        return index / 8;
    }

    public static int IndexToRank(int index)
    {
        return index % 8;  // simpler equation then i would have thought
    }
    
    
}