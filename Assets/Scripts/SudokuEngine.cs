
using UnityEngine;

public class SudokuEngine : MonoBehaviour
{
    #region Serialized Variables
    
    [SerializeField]
    private string PuzzleString;
    
    #endregion
    
    #region MonoBehaviour

    private void Start()
    {
        FormatPuzzleString();
        
        Puzzle puzzle = new Puzzle();

        for (int cellIndex = 0; cellIndex < PuzzleString.Length; ++cellIndex)
        {
            if (cellIndex >= PuzzleString.Length)
            {
                break;
            }
            
            char cellChar = PuzzleString[cellIndex];

            if (cellChar == ' ' || cellChar == '0')
            {
                continue;
            }
            
            puzzle.SetStarterValue(cellIndex, cellChar - '0');
        }

        puzzle.SolvePuzzle();
        
        Debug.Log(puzzle.ToString());
    }
    
    #endregion
    
    #region Helper Functions

    private void FormatPuzzleString()
    {
        PuzzleString = PuzzleString.Replace("\n", "");
        PuzzleString = PuzzleString.Replace("\r", "");
        PuzzleString = PuzzleString.Replace("\t", "");
    }
    
    #endregion
}


//Easy
// 4    86   541   2  9  7   8    4    3     5    1    6   2  5  3   814   17    9 

//Medium
// 8     4 1    9  2    35    536 1     2   7     3 845    96    7  8    9 4     3 
//   9   4   5 1   9 8   6 5 7  3  2      8      6  9  1 7 1   9 8   5 3   2   4   

//Hard
//   18  6  93  45  7        1    54   3  6  7   57    1        4  92  63  2  98   