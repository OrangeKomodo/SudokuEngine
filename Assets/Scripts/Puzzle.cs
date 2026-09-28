
using System.Collections.Generic;
using System.Text;
using CellAreas;
using Definitions;
using UnityEngine;

public class Puzzle
{
    #region Protected Variables

    // Compile-time variables
    private OneDimensionalCellArea[] _rows = new OneDimensionalCellArea[9];
    private OneDimensionalCellArea[] _columns = new OneDimensionalCellArea[9];
    private TwoDimensionalCellArea[] _twoDimensionalAreas = new TwoDimensionalCellArea[9];
    private Cell[] _cells = new Cell[81];
    
    // Runtime variables
    private List<SingleDeduction> _deductions = new List<SingleDeduction>();
    private int _stepCount = 0;
    
    #endregion
    
    #region Constructors

    public Puzzle()
    {
        // Populate the 9 row, column, and square areas
        for (int areaIndex = 0; areaIndex < 9; ++areaIndex)
        {
            _rows[areaIndex] = new OneDimensionalCellArea();
            _columns[areaIndex] = new OneDimensionalCellArea();
            _twoDimensionalAreas[areaIndex] = new TwoDimensionalCellArea();
            
            _rows[areaIndex].PopulateData(AreaType.Row, areaIndex);
            _columns[areaIndex].PopulateData(AreaType.Column, areaIndex);
            _twoDimensionalAreas[areaIndex].PopulateData(AreaType.Square, areaIndex);
        }

        // Populate the 81 cells, and connect them to the appropriate areas (and vice versa)
        for (int cellIndex = 0; cellIndex < 81; ++cellIndex)
        {
            _cells[cellIndex] = new Cell();
            
            int rowIndex = StandardPuzzleSettings.GetRowIndex(cellIndex);
            int columnIndex = StandardPuzzleSettings.GetColumnIndex(cellIndex);
            int squareIndex = StandardPuzzleSettings.GetSquareIndex(cellIndex);
            
            _cells[cellIndex].SetAreas(_rows[rowIndex], _columns[columnIndex], _twoDimensionalAreas[squareIndex]);
            
            _rows[rowIndex].AddCell(_cells[cellIndex]);
            _columns[columnIndex].AddCell(_cells[cellIndex]);
            _twoDimensionalAreas[squareIndex].AddCell(_cells[cellIndex]);
        }
    }
    
    // TODO: Create a Constructor that takes a custom rule set type and creates a puzzle with custom 2D areas.
        
    #endregion
    
    #region Public Functions

    public void SetStarterValue(int cellIndex, int starterValue)
    {
        _cells[cellIndex].SetStarterValue(starterValue);

        /*ResetAllDerivedValues();
        stepCount = 0;
        SolvePuzzle();*/
    }

    public bool SolvePuzzle()
    {
        do
        {
            Debug.Log("Try Solve");
            
            // Check 2D areas
            for (int tdaIndex = 0; tdaIndex < _twoDimensionalAreas.Length; ++tdaIndex)
            {
                _twoDimensionalAreas[tdaIndex].FindSinglesInArea(ref _deductions);
            }

            if (_deductions.Count > 0)
            {
                continue;
            }
            
            // Check rows
            for (int rowIndex = 0; rowIndex < _rows.Length; ++rowIndex)
            {
                _rows[rowIndex].FindSinglesInArea(ref _deductions);
            }

            if (_deductions.Count > 0)
            {
                continue;
            }
            
            // Check columns
            for (int columnIndex = 0; columnIndex < _columns.Length; ++columnIndex)
            {
                _columns[columnIndex].FindSinglesInArea(ref _deductions);
            }

            if (_deductions.Count > 0)
            {
                continue;
            }
            
            // Check all cells
            for (int cellIndex = 0; cellIndex < _cells.Length; ++cellIndex)
            {
                _cells[cellIndex].CheckIfOnlyOnePossibility(ref _deductions);
            }

            if (_deductions.Count > 0)
            {
                continue;
            }
            
            /*// Check 2D areas for groups
            for (int tdaIndex = 0; tdaIndex < twoDimensionalAreas.Length; ++tdaIndex)
            {
                
            }

            if (deductions.Count > 0)
            {
                continue;
            }
            
            // Check rows for groups
            for (int rowIndex = 0; rowIndex < rows.Length; ++rowIndex)
            {
                
            }

            if (deductions.Count > 0)
            {
                continue;
            }
            
            // Check columns for groups
            for (int columnIndex = 0; columnIndex < columns.Length; ++columnIndex)
            {
                
            }*/
            
        } while (ImplementExistingDeductions());
        
        return CheckSolved();
    }

    public override string ToString()
    {
        StringBuilder puzzleResult = new StringBuilder();

        for (int cellIndex = 0; cellIndex < _cells.Length; ++cellIndex)
        {
            Cell cell = _cells[cellIndex];
            char cellChar = cell.IsSolved() ? (char)(cell.CellValue + 48) : '?';
            puzzleResult.Append(cellChar.ToString());

            if (cellIndex % 9 == 8)
            {
                puzzleResult.Append("\n");
            }
        }
        
        return puzzleResult.ToString();
    }
    
    #endregion
    
    #region Helper Functions

    private bool ImplementExistingDeductions()
    {
        if (_deductions.Count == 0)
        {
            return false;
        }
        
        for (int deductionIndex = 0; deductionIndex < _deductions.Count; ++deductionIndex)
        {
            SingleDeduction singleDeduction = _deductions[deductionIndex];
            singleDeduction.Cell.SetSolvedValue(singleDeduction.SolvedNumber);
            LogDeduction(singleDeduction);
        }
        
        _deductions.Clear();
        return true;
    }

    private bool CheckSolved()
    {
        for (int cellIndex = 0; cellIndex < _cells.Length; ++cellIndex)
        {
            if (_cells[cellIndex].State != CellState.Starter && _cells[cellIndex].State != CellState.Solved)
            {
                return false;
            }
        }
        
        return true;
    }

    private void ResetAllDerivedValues()
    {
        for (int cellIndex = 0; cellIndex < _cells.Length; ++cellIndex)
        {
            _cells[cellIndex].ResetDerivedValue();
        }
    }

    private void LogDeduction(SingleDeduction singleDeduction)
    {
        string log = $"{singleDeduction.SolvedNumber} was set at location ({singleDeduction.Cell.RowIndex}, {singleDeduction.Cell.ColumnIndex})";
        string cause = null;
        
        switch (singleDeduction.Cause)
        {
            case Cause.Shape:
                cause = $"as the only possibility within the {singleDeduction.AreaType}";
                break;
            case Cause.Cell:
                cause = $"as the only possibility in the cell";
                break;
        }
        
        Debug.Log($"{++_stepCount}. {log} {cause}");
    }
    
    #endregion
}
