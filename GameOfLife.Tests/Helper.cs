namespace GameOfLife.Tests;

public static class Helper
{
    
    public static bool CompareBoards(bool[][] board1, bool[][] board2)
    {
        if (board1.Length != board2.Length)
            return false;
        for (int i = 0; i < board1.Length; i++)
        {
            if (board1[i].Length != board2[i].Length)
                return false;
            for (int j = 0; j < board1[i].Length; j++)
            {
                if (board1[i][j] != board2[i][j])
                    return false;
            }
        }
        return true;
    }
    
}