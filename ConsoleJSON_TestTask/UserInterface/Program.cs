using UserInterface;

const string path = @"Lists\Employees.json";

DataManager manager = new DataManager(path);

string result = manager.MakeOperation(args);
Console.WriteLine(result);

result = manager.MakeOperation(new[] { "-getall" });
Console.WriteLine(result);


