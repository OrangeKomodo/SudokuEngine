using System.Collections.Generic;
using DataTypes.Abstracts;
using Definitions;

namespace DataTypes.Deductions
{
    public class GroupDeduction : Deduction
    {
        public ACellArea CellArea;
        public List<Cell> Cells;
        public bool[] GroupedValues;
        public string GroupID;
        
        public GroupDeduction(ACellArea cellArea, List<Cell> cells, bool[] groupedValues, AreaType areaType, Cause cause)
        {
            CellArea = cellArea;
            Cells = cells;
            GroupedValues = groupedValues;
            AreaType = areaType;
            Cause = cause;

            // This just needs to be distinct
            GroupID = GenerateGroupID(cells);
        }

        private string GenerateGroupID(List<Cell> cells)
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
