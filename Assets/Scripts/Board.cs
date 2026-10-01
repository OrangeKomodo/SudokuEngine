using DataTypes;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] private CellComponent[] CellComponents; // <- It is imperative that these are in order in the Inspector
    
    private void Awake()
    {
        SudokuEngine.PuzzleComplete += OnPuzzleComplete;
    }

    private void OnDestroy()
    {
        SudokuEngine.PuzzleComplete -= OnPuzzleComplete;
    }

    private void OnPuzzleComplete(Puzzle puzzle)
    {
        for (int cellIndex = 0; cellIndex < puzzle.Cells.Length; ++cellIndex)
        {
            CellComponents[cellIndex].DisplayCell(puzzle.Cells[cellIndex]);
        }
    }
}
