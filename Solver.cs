using System.Drawing;

namespace OPR1_1;

public static class Solver
{
    private static BinaryTree<Model> _tree = new();

    public static List<Point> Solve(Model model) => SolveFromStepOne((Model)model.Clone());

    private static List<Point> SolveFromStepOne(Model model)
    {
        model.Print();
        if (model.Size == 2)
        {
            return model.CalculateLastStep();
        }

        var rowMins = model.FindRowMins();
        model.SubstractFromRows(rowMins);
        var columnMins = model.FindColumnMins();
        model.SubstractFromColumns(columnMins);
        return SolveFromStepFour(model);
    }

    private static List<Point> SolveFromStepFour(Model model)
    {
        model.Print();
        if (model.Size == 2)
        {
            return model.CalculateLastStep();
        }

        var zeroValueCells = model.FindZeroValueCells();
        var maxScoreCell = model.FindCellWithMaxScore(zeroValueCells);
        var model1 = model.TakeNode(maxScoreCell);
        var model2 = model.RemoveNode(maxScoreCell);
        return model1.Path <= model2.Path 
            ? SolveFromStepFour(model1) 
            : SolveFromStepOne(model2);
    }
}
