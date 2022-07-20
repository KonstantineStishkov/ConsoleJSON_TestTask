using UserInterface;

const string path = @"Lists\Employees.json";

DataManager manager = new DataManager(path);

string result = manager.MakeOperation(args);
Console.WriteLine(result);
Console.ReadKey();


