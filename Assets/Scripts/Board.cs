using TMPro;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] private TMP_Text PuzzleText;
    
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
        PuzzleText.text = puzzle.ToString(true, true);
    }
}
