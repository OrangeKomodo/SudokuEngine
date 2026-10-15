using Definitions;

namespace DataTypes.Deductions
{
    public class SingleDeduction : Deduction
    {
        public Cell Cell;
        public Value SolvedValue;

        public SingleDeduction(Cell cell, Value solvedValue, AreaType areaType, Cause cause)
        {
            Cell = cell;
            SolvedValue = solvedValue;
            AreaType = areaType;
            Cause = cause;
        }
    }
}
