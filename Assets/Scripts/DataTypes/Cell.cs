using System.Collections.Generic;
using DataTypes.CellAreas;
using Definitions;

namespace DataTypes
{
    public class Cell
    {
        #region Private Variables
    
        private int _cellValue = -1;
        private bool[] _possibleValues = new bool[9] { true, true, true, true, true, true, true, true, true };
        private CellState _state = CellState.Uncertain;
        private GroupState _groupState = GroupState.Ungrouped;
    
        private OneDimensionalCellArea _rowCellArea;
        private OneDimensionalCellArea _columnCellArea;
        private TwoDimensionalCellArea _twoDimensionalCellArea;
    
        #endregion
    
        #region Public Accessors

        public int CellValue => _cellValue;
        public bool[] PossibleValues => _possibleValues;
        public CellState State => _state;
        public GroupState GroupState => _groupState;

        public int RowIndex => RowCellArea.AreaIndex;
        public int ColumnIndex => ColumnCellArea.AreaIndex;
    
        public OneDimensionalCellArea RowCellArea => _rowCellArea;
        public OneDimensionalCellArea ColumnCellArea => _columnCellArea;
        public TwoDimensionalCellArea TwoDimensionalCellArea => _twoDimensionalCellArea;
    
        #endregion
        
        #region Public Functions

        public void SetAreas(OneDimensionalCellArea rowCellArea, OneDimensionalCellArea columnCellArea, TwoDimensionalCellArea twoDimensionalCellArea)
        {
            _rowCellArea = rowCellArea;
            _columnCellArea = columnCellArea;
            _twoDimensionalCellArea = twoDimensionalCellArea;
        }

        public bool IsSolved() => _state is CellState.Starter or CellState.Solved;

        public void CheckIfOnlyOnePossibility(ref List<SingleDeduction> deductions)
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
        
            SingleDeduction deduction = new SingleDeduction(this, (int)possibilityNumber, AreaType.Cell, Cause.Cell);
            deductions.Add(deduction);
        }

        public void SetValueImpossible(int obliteratedNumber)
        {
            _possibleValues[obliteratedNumber - 1] = false;
        }

        public void SetStarterValue(int starterNumber)
        {
            SetValue(starterNumber);
            _state = CellState.Starter;
            _groupState = GroupState.Ungrouped;
        }

        public void SetSolvedValue(int solvedNumber)
        {
            SetValue(solvedNumber);
            _state = CellState.Solved;
            _groupState = GroupState.Ungrouped;
        }

        public void ResetDerivedValue()
        {
            if (_state == CellState.Starter)
            {
                return;
            }
        
            _cellValue = -1;
            _possibleValues = new bool[9] { true, true, true, true, true, true, true, true, true }; 
            _state = CellState.Uncertain;
            _groupState = GroupState.Ungrouped;
        }
        
        #endregion
    
        #region Helper Functions

        private void SetValue(int number)
        {
            _cellValue = number;
        
            _possibleValues = new bool[9] { false, false, false, false, false, false, false, false, false };
            _possibleValues[number - 1] = true;
        
            _rowCellArea.Obliterate(number);
            _columnCellArea.Obliterate(number);
            _twoDimensionalCellArea.Obliterate(number);
        }
    
        #endregion
    
    }
}
