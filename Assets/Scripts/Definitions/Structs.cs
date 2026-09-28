namespace Definitions
{
    public struct SingleDeduction
    {
        public Cell Cell;
        public int SolvedNumber;
        public AreaType AreaType;
        public Cause Cause;

        public SingleDeduction(Cell cell, int solvedNumber, AreaType areaType, Cause cause)
        {
            Cell = cell;
            SolvedNumber = solvedNumber;
            AreaType = areaType;
            Cause = cause;
        }
    }

    public struct GroupDeduction
    {
        public Cell[] Cells;
        public int[] SolvedNumbers;
        public AreaType AreaType;
        public Cause Cause;
        
        public GroupDeduction(Cell[] cells, int[] groupedNumbers, AreaType areaType, Cause cause)
        {
            Cells = cells;
            SolvedNumbers = groupedNumbers;
            AreaType = areaType;
            Cause = cause;
        }
    }
}
