
namespace Definitions
{
    public enum AreaType
    {
        Row,
        Column,
        Square,
        Cell,
        Custom
    }

    public enum CellState
    {
        Uncertain,
        Starter,
        Grouped,
        Solved
    }

    public enum GroupState
    {
        Ungrouped,
        SingleGrouped,
        DoubleGrouped,
        TripleGrouped,
        Custom
    }

    public enum Cause
    {
        Shape,
        Cell,
        BondedGroup,
        HiddenGroup
    }

    public enum Index
    {
        Zero = 0,
        One = 1,
        Two = 2,
        Three = 3,
        Four = 4,
        Five = 5,
        Six = 6,
        Seven = 7,
        Eight = 8,
        None = 20,
        Multiple = 21
    }

    public enum Value
    {
        One = 1,
        Two = 2,
        Three = 3,
        Four = 4,
        Five = 5,
        Six = 6,
        Seven = 7,
        Eight = 8,
        Nine = 9,
        None = 20,
        Multiple = 21
    }
}