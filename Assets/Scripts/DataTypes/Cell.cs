using System;
using System.Collections.Generic;
using DataTypes.Deductions;
using Definitions;

namespace DataTypes
{
    public class Cell
    {
        #region Private Variables

        private int _cellIndex = -1;
        private Value _cellValue = Value.None;
        private bool[] _possibleValues = { true, true, true, true, true, true, true, true, true };
        private CellState _state = CellState.Uncertain;
        private GroupState _groupState = GroupState.Ungrouped;
    
        private CellArea _rowCellArea;
        private CellArea _columnCellArea;
        private CellArea _twoDimensionalCellArea;
    
        #endregion
    
        #region Public Accessors

        public int CellIndex => _cellIndex;
        public Value CellValue => _cellValue;
        public bool[] PossibleValues => _possibleValues;
        public CellState State => _state;
        public GroupState GroupState => _groupState;

        public int RowIndex => _rowCellArea.AreaIndex;
        public int ColumnIndex => _columnCellArea.AreaIndex;
        public int TwoDAreaIndex => _twoDimensionalCellArea.AreaIndex;
    
        public CellArea RowCellArea => _rowCellArea;
        public CellArea ColumnCellArea => _columnCellArea;
        public CellArea TwoDimensionalCellArea => _twoDimensionalCellArea;

        public Action<Cell> OnStarterValueSet;
        public Action<Cell> OnCellValueEliminated;
        public Action<Cell> OnCellSolved;
    
        #endregion
        
        #region Constructors

        public Cell(int cellIndex, CellArea rowCellArea, CellArea columnCellArea, CellArea twoDimensionalCellArea)
        {
            _cellIndex = cellIndex;
            _rowCellArea = rowCellArea;
            _columnCellArea = columnCellArea;
            _twoDimensionalCellArea = twoDimensionalCellArea;
        }
        
        #endregion
        
        #region Public Functions

        public bool IsSolved() => _state == CellState.Starter || _state == CellState.Solved;

        public void CheckIfOnlyOnePossibility(ref List<Deduction> deductions)
        {
            if (IsSolved())
            {
                return;
            }
        
            Value possibilityNumber = Value.None;
        
            for (int numberIndex = 0; numberIndex < _possibleValues.Length; ++numberIndex)
            {
                if (_possibleValues[numberIndex])
                {
                    possibilityNumber = possibilityNumber == Value.None ? (Value)(numberIndex + 1) : Value.Multiple;
                }
            }

            if (possibilityNumber is Value.None or Value.Multiple)
            {
                return;
            }
        
            Deduction deduction = new SingleDeduction(this, possibilityNumber, AreaType.Cell, Cause.Cell);
            deductions.Add(deduction);
        }

        public bool ObliterateValue(Value obliteratedNumber)
        {
            int numberIndex = (int)obliteratedNumber - 1;

            // If no change occurred, make that known
            if (!_possibleValues[numberIndex])
            {
                return false;
            }
            
            _possibleValues[numberIndex] = false;
            
            OnCellValueEliminated?.Invoke(this);
            return true;
        }

        public bool SetAsOnlyPossibilities(bool[] newValues)
        {
            bool changeOccurred = false;
            
            for (int valueIndex = 0; valueIndex < _possibleValues.Length; ++valueIndex)
            {
                // If old value is already not possible, we don't care about it
                if (!_possibleValues[valueIndex])
                {
                    continue;
                }

                // If old value is possible AND new value is still possible, we're not changing anything
                if (newValues[valueIndex])
                {
                    continue;
                }

                // If old value is possible, and new value isn't, change the old value and mark it
                changeOccurred = true;
                _possibleValues[valueIndex] = false;
            }

            // If no change occurred, make that known
            if (!changeOccurred)
            {
                return false;
            }
            
            OnCellValueEliminated?.Invoke(this);
            return true;
        }

        public void SetStarterValue(Value starterNumber)
        {
            SetValue(starterNumber);
            _state = CellState.Starter;
            _groupState = GroupState.Ungrouped;
            
            OnStarterValueSet?.Invoke(this);
        }

        public void SetSolvedValue(Value solvedNumber)
        {
            SetValue(solvedNumber);
            _state = CellState.Solved;
            _groupState = GroupState.Ungrouped;

            OnCellSolved?.Invoke(this);
        }

        public void ResetSolvedValue()
        {
            if (_state == CellState.Starter)
            {
                return;
            }
        
            _cellValue = Value.None;
            _possibleValues = new bool[] { true, true, true, true, true, true, true, true, true };
            _state = CellState.Uncertain;
            _groupState = GroupState.Ungrouped;
        }
        
        #endregion
    
        #region Helper Functions

        private void SetValue(Value number)
        {
            _cellValue = number;

            _possibleValues = new bool[] { false, false, false, false, false, false, false, false, false };
            _possibleValues[(int)number - 1] = true;
        }
    
        #endregion
    
    }
}
