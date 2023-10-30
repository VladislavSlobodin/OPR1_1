using OPR1_1;

var model = new Model(@$"D:\model.txt");
var path = Solver.Solve(model);
Console.WriteLine($"Path: {path.ToPath()}");
Console.WriteLine($"Distance traveled: {model.CalculatePath(path)}");
//TODO: implement binary tree
//1 -> 7 -> 2 -> 6 -> 4 -> 5 -> 3 -> 1