using System.Collections.Generic;
using DataTypes.CellAreas;
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
    
        private OneDimensionalCellArea _rowCellArea;
        private OneDimensionalCellArea _columnCellArea;
        private TwoDimensionalCellArea _twoDimensionalCellArea;
    
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
    
        public OneDimensionalCellArea RowCellArea => _rowCellArea;
        public OneDimensionalCellArea ColumnCellArea => _columnCellArea;
        public TwoDimensionalCellArea TwoDimensionalCellArea => _twoDimensionalCellArea;

        //public Action<Value> OnCellSolved;
    
        #endregion
        
        #region Constructors

        public Cell(int cellIndex, OneDimensionalCellArea rowCellArea, OneDimensionalCellArea columnCellArea, TwoDimensionalCellArea twoDimensionalCellArea)
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

        public void SetValueImpossible(Value obliteratedNumber)
        {
            _possibleValues[(int)obliteratedNumber - 1] = false;
        }

        public void SetStarterValue(int starterNumber)
        {
            SetValue((Value)starterNumber);
            _state = CellState.Starter;
            _groupState = GroupState.Ungrouped;
        }

        public void SetSolvedValue(Value solvedNumber)
        {
            SetValue(solvedNumber);
            _state = CellState.Solved;
            _groupState = GroupState.Ungrouped;

            //OnCellSolved.Invoke(solvedNumber);
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

        public void EliminatePossibilities(bool[] newValues)
        {
            _possibleValues = Util.BoolArrayAnd(_possibleValues, newValues);
        }
        
        #endregion
    
        #region Helper Functions

        private void SetValue(Value number)
        {
            _cellValue = number;

            _possibleValues = new bool[] { false, false, false, false, false, false, false, false, false };
            _possibleValues[(int)number - 1] = true;
        
            _rowCellArea.Obliterate(number);
            _columnCellArea.Obliterate(number);
            _twoDimensionalCellArea.Obliterate(number);
        }
    
        #endregion
    
    }
}
