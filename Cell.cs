using System.Drawing;

namespace OPR1_1;

public class Cell : ICloneable, IComparable<Cell>
{
    public Point Position { get; private set; }

    public double Value { get; set; }

    public Cell(Point position, double value)
    {
        Position = position;
        Value = value;
    }

    public object Clone() => new Cell(Position, Value);

    int IComparable<Cell>.CompareTo(Cell? other) => other is null ? 0 : Value.CompareTo(other.Value);
}