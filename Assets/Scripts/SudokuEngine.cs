
using System;
using DataTypes;
using UnityEngine;

public class SudokuEngine : MonoBehaviour
{
    #region Public Variables
    
    public static Action<Puzzle> PuzzleComplete;
    
    #endregion
    
    #region Serialized Variables
    
    //[SerializeField]
    //private string PuzzleString;
    
    #endregion
    
    #region MonoBehaviour

    private void Start()
    {
        /*if (string.IsNullOrWhiteSpace(PuzzleString))
        {
            return;
        }
        
        SolvePuzzle(PuzzleString);*/
    }
    
    #endregion
    
    #region Public Functions

    public static void SolvePuzzle(string puzzleString)
    {
        Puzzle puzzle = new Puzzle(puzzleString);

        SolvePuzzle(puzzle);
    }

    public static void SolvePuzzle(Puzzle puzzle)
    {
        puzzle.SolvePuzzle();
        
        PuzzleComplete?.Invoke(puzzle);
        Debug.Log(puzzle.ToString(true));
    }
    
    #endregion
}


// Easy
// 4    86   541   2  9  7   8    4    3     5    1    6   2  5  3   814   17    9 

// Medium
// 8     4 1    9  2    35    536 1     2   7     3 845    96    7  8    9 4     3 
//   9   4   5 1   9 8   6 5 7  3  2      8      6  9  1 7 1   9 8   5 3   2   4   

// Hard
//   18  6  93  45  7        1    54   3  6  7   57    1        4  92  63  2  98   

// 17 Clues (Absolute minimum for a unique solution)
//       1 4         2           5 4 7  8   3    1 9    3  4  2   5 1        8 6   
//   7     1            43 2         6   5 9         418    81     2    5  4    3  
//              1  2  3    4         5  6 7     2743          36      8    1 2 5   