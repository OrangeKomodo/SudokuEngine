using System.Collections.Generic;

namespace DataTypes
{
    public class Group
    {
        private List<Cell> _cells;

        public Group(List<Cell> cells)
        {
            if (cells.Count == 0)
            {
                return;
            }

            _cells = cells;

            for (int cellIndex = 0; cellIndex < _cells.Count; ++cellIndex)
            {
                Cell cell = _cells[cellIndex];

                if (cell == null)
                {
                    continue;
                }

                cell.OnCellValueEliminated += OnCellUpdated;
                cell.OnCellSolved += OnCellSolved;
            }
        }

        ~Group()
        {
            for (int cellIndex = 0; cellIndex < _cells.Count; ++cellIndex)
            {
                Cell cell = _cells[cellIndex];

                if (cell == null)
                {
                    continue;
                }

                cell.OnCellValueEliminated -= OnCellUpdated;
                cell.OnCellSolved -= OnCellSolved;
            }
        }

        private void OnCellUpdated(Cell cell)
        {
            
        }

        private void OnCellSolved(Cell cell)
        {
            cell.OnCellValueEliminated -= OnCellUpdated;
            cell.OnCellSolved -= OnCellSolved;
            
            _cells.Remove(cell);

            if (_cells.Count > 1)
            {
                return;
            }
            
            // Resolve Group, and mark it for deletion
        }
    }
}
