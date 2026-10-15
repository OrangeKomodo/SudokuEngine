using System.Collections.Generic;
using DataTypes.Deductions;
using Definitions;

namespace DataTypes.Abstracts
{
    public abstract class ACellArea
    {
        #region Private Variables

        private AreaType _areaType;
        private int _areaIndex;
        private Cell[] _cells = new Cell[9];

        private int _uncertainCellCount;

        private bool[] _solvedValues = new bool[] { false, false, false, false, false, false, false, false, false };
    
        #endregion
    
        #region Public Accessors
    
        public AreaType AreaType => _areaType;
        public int AreaIndex => _areaIndex;
        public Cell[] Cells => _cells;

        public int UncertainCellCount => _uncertainCellCount;
    
        #endregion
        
        #region Public Functions

        public void PopulateData(AreaType areaType, int areaIndex)
        {
            _areaType = areaType;
            _areaIndex = areaIndex;
        }

        public void AddCell(Cell cell)
        {
            int cellIndex = 0;

            while (cellIndex < _cells.Length && _cells[cellIndex] != null)
            {
                ++cellIndex;
            }
            
            _cells[cellIndex] = cell;

            ++_uncertainCellCount;
        }

        public void Obliterate(Value solvedValue)
        {
            for (int cellIndex = 0; cellIndex < _cells.Length; ++cellIndex)
            {
                if (_cells[cellIndex].IsSolved())
                {
                    continue;
                }
                
                _cells[cellIndex].SetValueImpossible(solvedValue);
            }

            _solvedValues[(int)solvedValue - 1] = true;
            --_uncertainCellCount;
        }

        public void Obliterate(List<Cell> groupedCells, bool[] values)
        {
            for (int cellIndex = 0; cellIndex < _cells.Length; ++cellIndex)
            {
                Cell cell = _cells[cellIndex];
                
                if (cell.IsSolved())
                {
                    continue;
                }
                
                if (groupedCells.Contains(cell))
                {
                    cell.EliminatePossibilities(values);
                    continue;
                }
                
                cell.EliminatePossibilities(Util.BoolArrayNot(values));
            }
        }

        public void FindSinglesInArea(ref List<Deduction> deductions)
        {
            Index[] numberLocations = { Index.None, Index.None, Index.None, Index.None, Index.None, Index.None, Index.None, Index.None, Index.None };

            for (int cellIndex = 0; cellIndex < _cells.Length; ++cellIndex)
            {
                Cell cell = _cells[cellIndex];
                    
                if (cell.IsSolved())
                {
                    continue;
                }

                for (int numberIndex = 0; numberIndex < numberLocations.Length; ++numberIndex)
                {
                    if (cell.PossibleValues[numberIndex])
                    {
                        numberLocations[numberIndex] = numberLocations[numberIndex] == Index.None ? (Index)cellIndex : Index.Multiple;
                    }
                }
            }

            for (int numberIndex = 0; numberIndex < numberLocations.Length; ++numberIndex)
            {
                if (numberLocations[numberIndex] is Index.None or Index.Multiple)
                {
                    continue;
                }
                
                Index positionOfSingle = numberLocations[numberIndex];

                Deduction deduction = new SingleDeduction(Cells[(int)positionOfSingle], numberIndex + 1, _areaType, Cause.Area);
                deductions.Add(deduction);
            }
        }

        public void FindGroupsInArea(ref List<Deduction> deductions)
        {
            // This shouldn't ever be 1 when it hits here; that would mean that only one space is open in the Area, which would've been found already
            // It might be 0, though. And we might as well guard against the 1 case
            if (_uncertainCellCount <= 1)
            {
                return;
            }
            
            // If there are only two or three uncertain cells, automatically make a group out of them
            if (_uncertainCellCount <= 3)
            {
                CreateGroupFromRemainingCells(ref deductions);
                
                return;
            }

            FindNakedGroups(ref deductions);
            FindHiddenGroups(ref deductions);
        }
        
        #endregion

        #region Helper Functions

        private void CreateGroupFromRemainingCells(ref List<Deduction> deductions)
        {
            List<Cell> groupedCells = new List<Cell>();
            bool[] cellGroupValues = { false, false, false, false, false, false, false, false, false };

            for (int cellIndex = 0; cellIndex < _cells.Length; ++cellIndex)
            {
                Cell cell = _cells[cellIndex];
                    
                if (cell.IsSolved())
                {
                    continue;
                }

                groupedCells.Add(cell);
                cellGroupValues = Util.BoolArrayOr(cellGroupValues, cell.PossibleValues);
            }
                
            Deduction deduction = new GroupDeduction(this, groupedCells, cellGroupValues, _areaType, Cause.ConstrainedGroup);
            deductions.Add(deduction);
        }

        private void FindNakedGroups(ref List<Deduction> deductions)
        {
            for (int firstCellIndex = 0; firstCellIndex < _cells.Length - 1; ++firstCellIndex)
            {
                Cell firstCell = _cells[firstCellIndex];
                if (firstCell.IsSolved())
                {
                    continue;
                }
                
                bool[] firstCellPossibilities = firstCell.PossibleValues;
                int firstCount = Util.BoolArrayCount(firstCellPossibilities);

                // We're only looking for pairs or triplets at this time
                if (firstCount > 3)
                {
                    continue;
                }
                
                for (int secondCellIndex = firstCellIndex + 1; secondCellIndex < _cells.Length; ++secondCellIndex)
                {
                    Cell secondCell = _cells[secondCellIndex];
                    if (secondCell.IsSolved())
                    {
                        continue;
                    }
                    
                    bool[] secondCellPossibilities = secondCell.PossibleValues;
                    
                    bool[] cellPairValues = Util.BoolArrayOr(firstCellPossibilities, secondCellPossibilities);
                    int cellPairCount = Util.BoolArrayCount(cellPairValues);

                    // If there are more than 3, we don't have a group here
                    if (cellPairCount > 3)
                    {
                        continue;
                    }

                    // If there are 2 possibilities, the pair has been found
                    if (cellPairCount == 2)
                    {
                        List<Cell> groupedCells = new List<Cell>
                        {
                            firstCell,
                            secondCell
                        };
                        Deduction deduction = new GroupDeduction(this, groupedCells, cellPairValues, _areaType, Cause.NakedGroup);
                        deductions.Add(deduction);

                        break;
                    }
                    
                    // If there are no more cells, we shouldn't bother searching for a triplet
                    if (secondCellIndex >= _cells.Length - 1)
                    {
                        continue;
                    }

                    // Try to find a third cell for the possible group
                    for (int thirdCellIndex = secondCellIndex + 1; thirdCellIndex < _cells.Length; ++thirdCellIndex)
                    {
                        Cell thirdCell = _cells[thirdCellIndex];
                        if (thirdCell.IsSolved())
                        {
                            continue;
                        }
                    
                        bool[] thirdCellPossibilities = thirdCell.PossibleValues;
                    
                        bool[] cellTripletValues = Util.BoolArrayOr(cellPairValues, thirdCellPossibilities);
                        int cellTripletCount = Util.BoolArrayCount(cellTripletValues);
                        
                        // If there are only three possibilities between all cells, the triplet has been found
                        if (cellTripletCount == 3)
                        {
                            List<Cell> groupedCells = new List<Cell>()
                            {
                                firstCell, 
                                secondCell,
                                thirdCell
                            };
                            Deduction deduction = new GroupDeduction(this, groupedCells, cellTripletValues, _areaType, Cause.NakedGroup);
                            deductions.Add(deduction);

                            break;
                        }
                    }
                }
            }
        }

        private void FindHiddenGroups(ref List<Deduction> deductions)
        {
            for (Value firstValue = Value.One; firstValue <= Value.Nine - 1; ++firstValue)
            {
                // Check if Value has already been solved within the Area
                if (_solvedValues[(int)firstValue - 1])
                {
                    continue;
                }

                bool[] firstValueLocations = GetValueLocations(firstValue);
                int firstCount = Util.BoolArrayCount(firstValueLocations);

                // We're looking for Values with only up to 3 instances
                if (firstCount > 3)
                {
                    continue;
                }
                
                for (Value secondValue = firstValue + 1; secondValue <= Value.Nine; ++secondValue)
                {
                    // Check if Value has already been solved within the Area
                    if (_solvedValues[(int)secondValue - 1])
                    {
                        continue;
                    }
                    
                    bool[] secondValueLocations = GetValueLocations(secondValue);
                    
                    bool[] valuePairLocations = Util.BoolArrayOr(firstValueLocations, secondValueLocations);
                    int valuePairCount = Util.BoolArrayCount(valuePairLocations);

                    // If there are more than 3, we don't have a group here
                    if (valuePairCount > 3)
                    {
                        continue;
                    }

                    // If there are 2 possible locations for both values, the pair has been found
                    if (valuePairCount == 2)
                    {
                        List<Cell> groupedCells = GetCellsAtLocations(valuePairLocations);
                        bool[] groupedValues = GetValueList(firstValue, secondValue);
                        Deduction deduction = new GroupDeduction(this, groupedCells, groupedValues, _areaType, Cause.HiddenGroup);
                        deductions.Add(deduction);

                        break;
                    }
                    
                    // If there are no more values, we shouldn't bother searching for a triplet
                    if (secondValue >= Value.Nine)
                    {
                        continue;
                    }

                    // Try to find a third value for the possible group
                    for (Value thirdValue = secondValue + 1; thirdValue <= Value.Nine; ++thirdValue)
                    {
                        // Check if Value has already been solved within the Area
                        if (_solvedValues[(int)thirdValue - 1])
                        {
                            continue;
                        }
                    
                        bool[] thirdValueLocations = GetValueLocations(thirdValue);
                        
                        bool[] cellTripletLocations = Util.BoolArrayOr(valuePairLocations, thirdValueLocations);
                        int cellTripletCount = Util.BoolArrayCount(cellTripletLocations);
                        
                        // If there are only three possibilities between all cells, the triplet has been found
                        if (cellTripletCount == 3)
                        {
                            List<Cell> groupedCells = GetCellsAtLocations(cellTripletLocations);
                            bool[] groupedValues = GetValueList(firstValue, secondValue, thirdValue);
                            Deduction deduction = new GroupDeduction(this, groupedCells, groupedValues, _areaType, Cause.HiddenGroup);
                            deductions.Add(deduction);

                            break;
                        }
                    }
                }
            }
        }

        private bool[] GetValueLocations(Value value)
        {
            bool[] output = { false, false, false, false, false, false, false, false, false };
            int valueIndex = (int)value - 1;

            for (int cellIndex = 0; cellIndex < _cells.Length; ++cellIndex)
            {
                output[cellIndex] = _cells[cellIndex].PossibleValues[valueIndex];
            }
            
            return output;
        }

        private bool[] GetValueList(params Value[] values)
        {
            bool[] output = { false, false, false, false, false, false, false, false, false };
            
            for (int valueIndex = 0; valueIndex < values.Length; ++valueIndex)
            {
                Value value = values[valueIndex];
                output[(int)value - 1] = true;
            }
            
            return output;
        }

        private List<Cell> GetCellsAtLocations(bool[] locations)
        {
            List<Cell> output = new List<Cell>();

            for (int locationIndex = 0; locationIndex < _cells.Length; ++locationIndex)
            {
                if (!locations[locationIndex])
                {
                    continue;
                }
                
                Cell cell = _cells[locationIndex];
                    
                if (cell.IsSolved())
                {
                    continue;
                }

                output.Add(cell);
            }

            return output;
        }

        #endregion
    }
}
