using Definitions;

namespace DataTypes
{
    public class Group
    {
        private Cell[] _cells;
        private bool[] _groupedValues;
        private AreaType _areaType;

        public Group(Cell[] cells, bool[] groupedValues, AreaType areaType)
        {
            if (cells.Length == 0 || groupedValues.Length == 0 || cells.Length != groupedValues.Length)
            {
                return;
            }

            _cells = cells;
            _groupedValues = groupedValues;
            _areaType = areaType;

            for (int cellIndex = 0; cellIndex < _cells.Length; ++cellIndex)
            {
                Cell cell = _cells[cellIndex];

                if (cell == null)
                {
                    continue;
                }

                //cell.OnCellSolved += OnCellSolved;
            }
        }

        ~Group()
        {
            for (int cellIndex = 0; cellIndex < _cells.Length; ++cellIndex)
            {
                Cell cell = _cells[cellIndex];

                if (cell == null)
                {
                    continue;
                }

                //cell.OnCellSolved -= OnCellSolved;
            }
        }

        private void OnCellSolved(Value value)
        {
            //TODO: If only two remaining cells in group, solve the other. If three remaining, remove the solved cell.  
        }
    }
}
