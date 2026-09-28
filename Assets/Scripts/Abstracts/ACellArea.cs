
using System.Collections.Generic;
using Definitions;

namespace Abstracts
{
    public abstract class ACellArea
    {

        #region Protected Variables

        private AreaType _areaType;
        private int _areaIndex;
        private Cell[] _cells = new Cell[9] { null, null, null, null, null, null, null, null, null };
    
        #endregion
    
        #region Public Accessors
    
        public AreaType AreaType => _areaType;
        public int AreaIndex => _areaIndex;
        public Cell[] Cells => _cells;
    
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
        }

        public void Obliterate(int solvedNumber)
        {
            for (int cellIndex = 0; cellIndex < _cells.Length; ++cellIndex)
            {
                if (_cells[cellIndex].IsSolved())
                {
                    continue;
                }
                
                _cells[cellIndex].SetValueImpossible(solvedNumber);
            }
        }

        public void FindSinglesInArea(ref List<SingleDeduction> deductions)
        {
            Index[] numberLocations = new Index[9] { Index.None, Index.None, Index.None, Index.None, Index.None, Index.None, Index.None, Index.None, Index.None };
            int uncertainCellCount = 0;

            for (int cellIndex = 0; cellIndex < _cells.Length; ++cellIndex)
            {
                Cell cell = _cells[cellIndex];
                    
                if (cell.IsSolved())
                {
                    continue;
                }

                ++uncertainCellCount;

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

                SingleDeduction deduction = new SingleDeduction(Cells[(int)positionOfSingle], numberIndex + 1, _areaType, Cause.Shape);
                deductions.Add(deduction);
            }
        }

        public void FindGroupsInArea(ref List<GroupDeduction> deductions)
        {
            
        }
        
        #endregion

    }
}
