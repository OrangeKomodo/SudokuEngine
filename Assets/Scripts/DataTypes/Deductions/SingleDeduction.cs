using Definitions;

namespace DataTypes.Deductions
{
    public class SingleDeduction : Deduction
    {
        public Cell Cell;
        public int SolvedValue;

        public SingleDeduction(Cell cell, int solvedValue, AreaType areaType, Cause cause)
        {
            Cell = cell;
            SolvedValue = solvedValue;
            AreaType = areaType;
            Cause = cause;
        }
    }
}
