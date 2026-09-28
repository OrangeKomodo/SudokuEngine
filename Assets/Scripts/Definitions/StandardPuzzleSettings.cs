
namespace Definitions
{
    public static class StandardPuzzleSettings
    {
        public static int GetRowIndex(int cellIndex)
        {
            return cellIndex / 9;
        }

        public static int GetColumnIndex(int cellIndex)
        {
            return cellIndex % 9;
        }

        public static int GetSquareIndex(int cellIndex)
        {
            int rowBlock = cellIndex / 9 / 3;
            int columnBlock = cellIndex % 9 / 3;
            return rowBlock * 3 + columnBlock;
        }
    }
}
