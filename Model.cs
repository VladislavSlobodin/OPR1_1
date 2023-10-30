using System.Drawing;
using System.Text;

namespace OPR1_1;

public class Model : IComparable<Model>, ICloneable
{
    private const string INFINITY = "inf";
    private List<Cell> _cells = new();
    private List<Point> _path = new();
    private int _initialSize;

    public int Size { get; private set; }

    private Model(List<Cell> cells, List<Point> path, int size)
    {
        _cells = cells;
        _path = path;
        Size = _initialSize = size;
    }

    private Model(List<Cell> cells, List<Point> path, int size, int initialSize)
    {
        _cells = cells;
        _path = path;
        Size = size;
        _initialSize = initialSize;
    }

    public double CalculatePath(List<Point> path) => path.Sum(p => _cells.First(c => c.Position == p).Value);

    public Model(string path)
    {
        var lines = File.ReadAllLines(path);
        _initialSize = Size = lines.Length;
        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i].Split('\t');
            for (int j = 0; j < line.Length; j++)
            {
                _cells.Add(new(new(i, j), line[j].Equals(INFINITY) ? double.PositiveInfinity : double.Parse(line[j])));
            }
        }
    }

    int IComparable<Model>.CompareTo(Model? other)
        => other is not null
        ? Math.Sign(_cells.Count - other._cells.Count)
        : 1;

    public object Clone() => new Model(_cells.Clone(), _path.ToList(), Size, _initialSize);

    public Model TakeNode(Cell cell)
    {
        var result = (Model)Clone();
        result._path.Add(cell.Position);
        result._cells.RemoveRange(c => c.Position.X == cell.Position.X || c.Position.Y == cell.Position.Y);
        result.Size--;
        var inverseCell = result._cells.FirstOrDefault(c => c.Position == new Point(cell.Position.Y, cell.Position.X));
        if (inverseCell is not null)
        {
            inverseCell.Value = double.PositiveInfinity;
        }

        return result;
    }

    public Model RemoveNode(Cell cell)
    {
        var result = (Model)Clone();    
        result._cells.First(c => c.Position == cell.Position).Value = double.PositiveInfinity;
        return result;
    }

    public List<Point> CalculateLastStep()
    {
        var rowMins = FindRowMins();
        SubstractFromRows(rowMins);
        var colMins = FindColumnMins();
        SubstractFromColumns(colMins);
        Print();

        var predicate = _cells[0].Value == 0 && _cells[3].Value == 0;
        _path.Add(_cells[predicate ? 0 : 1].Position);
        _path.Add(_cells[predicate ? 3 : 2].Position);
        return _path;
    }

    public void Print()
    {
        StringBuilder stringBuilder = new();//TODO: debug it
        var columnIndexes = _cells.Select(c => c.Position.Y).Distinct();
        columnIndexes.ForEach(x =>  stringBuilder.Append($"\t{x + 1}"));
        stringBuilder.AppendLine();
        stringBuilder.Append('-', columnIndexes.Count() * 8 + 1);
        stringBuilder.AppendLine();
        foreach (var rowId in Enumerable.Range(0, _initialSize))
        {
            var row = TakeRow(rowId);
            if (!row.Any())
            {
                continue;
            }

            stringBuilder.Append($"{rowId + 1} |\t");
            AppendRow(row, stringBuilder);
        }

        Console.WriteLine(stringBuilder.ToString());
    }

    private void AppendRow(IEnumerable<Cell> row, StringBuilder stringBuilder)
    {
        row.ForEach(cell => stringBuilder.Append($"{(cell.Value == 0 ? $"0({CellScore(cell)})" : cell.Value)}\t"));
        stringBuilder.Append('\n');
    }

    public double? FindRowMin(int rowId) => _cells.Where(c => c.Position.X == rowId).MinOrDefault(c => c.Value);

    public double? FindColumnMin(int columnId) => _cells.Where(c => c.Position.Y == columnId).MinOrDefault(c => c.Value);

    public double[] FindRowMins() => Enumerable.Range(0, _initialSize).Select(FindRowMin).Where(x => x is not null).Cast<double>().ToArray();

    public double[] FindColumnMins() => Enumerable.Range(0, _initialSize).Select(FindColumnMin).Where(x => x is not null).Cast<double>().ToArray();

    public double Path => FindRowMins().Sum() + FindColumnMins().Sum();

    public void SubsractFromRow(int rowId, double value) => TakeRow(rowId).ForEach(c => c.Value -= value);

    public void SubsractFromColumn(int columnId, double value) => TakeColumn(columnId).ForEach(c => c.Value -= value);

    public IEnumerable<Cell> TakeRow(int rowIndex) => _cells.Where(c => c.Position.X == rowIndex);

    public IEnumerable<Cell> TakeRow(int rowIndex, Func<Cell, bool> predicate) => _cells.Where(c => c.Position.X == rowIndex).Where(predicate);

    public IEnumerable<Cell> TakeColumn(int columnIndex) => _cells.Where(c => c.Position.Y == columnIndex);
    
    public IEnumerable<Cell> TakeColumn(int columnIndex, Func<Cell, bool> predicate) => _cells.Where(c => c.Position.Y == columnIndex).Where(predicate);

    public void SubstractFromRows(double[] values)
    {
        var rowIndexes = _cells.Select(c => c.Position.X).Distinct().ToList();
        for (int i = 0; i < Size; i++)
        {
            SubsractFromRow(rowIndexes[i], values[rowIndexes[i]]);
        }
    }

    public void SubstractFromColumns(double[] values)
    {
        var columnIndexes = _cells.Select(c => c.Position.Y).Distinct().ToList();
        for (int i = 0; i < Size; i++)
        {
            SubsractFromColumn(columnIndexes[i], values[i]);
        }
    }

    public List<Cell> FindZeroValueCells() => _cells.Where(c => c.Value == 0).ToList();

    public double CellScore(Cell cell)
        => (double)(TakeRow(cell.Position.X, c => c.Position != cell.Position).MinOrDefault(c => c.Value) +
                    TakeColumn(cell.Position.Y, c => c.Position != cell.Position).MinOrDefault(c => c.Value))!;
    public Cell FindCellWithMaxScore(List<Cell> cells) => cells.First(c => CellScore(c) == cells.Max(CellScore));
}
