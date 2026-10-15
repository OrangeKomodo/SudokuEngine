using System.Collections.Generic;
using DataTypes.Abstracts;
using Definitions;

namespace DataTypes.Deductions
{
    public class PointerDeduction : Deduction
    {
        public List<Cell> Cells;
        public Value PointerValue;
        public AreaType IntersectorAreaType;
        public string PointerID;

        public PointerDeduction(List<Cell> cells, Value pointerValue, AreaType areaType, AreaType intersectorAreaType, Cause cause)
        {
            Cells = cells;
            PointerValue = pointerValue;
            AreaType = areaType;
            IntersectorAreaType = intersectorAreaType;
            Cause = cause;

            PointerID = GenerateGroupID(cells, pointerValue);
        }

        // This just needs to be distinct and reproducible, not fancy
        public static string GenerateGroupID(List<Cell> cells, Value pointerValue)
        {
            string output = "";
            
            for (int groupedCellIndex = 0; groupedCellIndex < cells.Count; ++groupedCellIndex)
            {
                output += $"{cells[groupedCellIndex].CellIndex:D2}{(int)pointerValue}";
            }

            return output;
        }
    }
}
