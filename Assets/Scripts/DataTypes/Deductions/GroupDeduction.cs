using System.Collections.Generic;
using Definitions;

namespace DataTypes.Deductions
{
    public class GroupDeduction : Deduction
    {
        public CellArea CellArea;
        public List<Cell> Cells;
        public bool[] GroupedValues;
        public string GroupID;
        
        public GroupDeduction(CellArea cellArea, List<Cell> cells, bool[] groupedValues, AreaType areaType, Cause cause)
        {
            CellArea = cellArea;
            Cells = cells;
            GroupedValues = groupedValues;
            AreaType = areaType;
            Cause = cause;

            GroupID = GenerateGroupID(cells);
        }

        // This just needs to be distinct and reproducible, not fancy
        private static string GenerateGroupID(List<Cell> cells)
        {
            string output = "";
            
            for (int groupedCellIndex = 0; groupedCellIndex < cells.Count; ++groupedCellIndex)
            {
                output += $"{cells[groupedCellIndex].CellIndex:D2}";
            }

            return output;
        }
    }
}
