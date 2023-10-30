using System.Drawing;
using System.Text;

namespace OPR1_1;

public static class ExtensionMethods
{
    public static void ForEach<T>(this IEnumerable<T> values, Action<T> action)
    {
        if (values is null || values.Count() == 0)
        {
            return;
        }

        foreach (var value in values)
        {
            action(value);
        }
    }

    public static void RemoveRange<T>(this List<T> values, Func<T, bool> predicate)
    {
        var valuesToRemove = values.Where(predicate).ToList();
        valuesToRemove.ForEach(v => values.Remove(v));
    }

    public static string ToPath(this List<Point> points)
    {
        StringBuilder stringBuilder = new();
        var point = points.First();
        for (int i = 0; i < points.Count; i++)
        {
            stringBuilder.Append($"{point.X + 1} -> ");
            point = points.First(p => p.X == point.Y);
        }

        stringBuilder.Append($"{point.X + 1}");
        return stringBuilder.ToString();
    }

    public static List<T> Clone<T>(this List<T> values) where T : ICloneable => values.Select(v => (T)v.Clone()).ToList();

    public static double? MinOrDefault<T>(this IEnumerable<T> values, Func<T, double> selector, double? defaultValue = 0) where T : IComparable<T>
        => values.Any()
        ? values.Min(selector)!
        : defaultValue;
}