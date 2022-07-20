using UserInterface;

namespace DataManagerTests
{
    public class UpdateTests
    {
        const string operationGet = "-get";
        const int operationIndex = 0;

        const string path = @"Lists\EmployeesTest.json";

        [SetUp]
        public void Setup()
        {
            const string pathToReadOnly = @"Lists\EmployeesTestReadOnly.json";

            File.Copy(pathToReadOnly, path, true);
        }

        [Test]
        [TestCase("Id = 2, FirstName = David, LastName = Smith, SalaryPerHour = 105,4", "-update", "Id:2", "FirstName:David")]
        [TestCase("Id = 2, FirstName = James, LastName = Bond, SalaryPerHour = 105,4", "-update", "Id:2", "LastName:Bond")]
        [TestCase("Id = 2, FirstName = James, LastName = Smith, SalaryPerHour = 99,2", "-update", "Id:2", "Salary:99,2")]
        public void UpdateSingleValuesTest(string expected, params string[] args)
        {
            DataManager manager = new DataManager(path);

            manager.MakeOperation(args);
            args[operationIndex] = operationGet;
            string actual = manager.MakeOperation(args);

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        [TestCase("Id = 2, FirstName = Jake, LastName = Bond, SalaryPerHour = 105,4", "-update", "Id:2", "FirstName:Jake", "LastName:Bond")]
        [TestCase("Id = 2, FirstName = Jake, LastName = Bond, SalaryPerHour = 105,4", "-update", "Id:2", "LastName:Bond", "FirstName:Jake" )]
        [TestCase("Id = 2, FirstName = James, LastName = Bond, SalaryPerHour = 99,4", "-update", "Id:2", "LastName:Bond", "Salary:99,4")]
        [TestCase("Id = 2, FirstName = James, LastName = Bond, SalaryPerHour = 99,4", "-update", "Id:2", "Salary:99,4", "LastName:Bond")]
        [TestCase("Id = 2, FirstName = Alex, LastName = Smith, SalaryPerHour = 99,2", "-update", "Id:2", "FirstName:Alex", "Salary:99,2")]
        [TestCase("Id = 2, FirstName = Alex, LastName = Smith, SalaryPerHour = 99,2", "-update", "Id:2", "Salary:99,2", "FirstName:Alex")]
        [TestCase("Id = 2, FirstName = John, LastName = Doe, SalaryPerHour = 132,7", "-update", "Id:2", "FirstName:John", "LastName:Doe", "Salary:132,7")]
        [TestCase("Id = 2, FirstName = John, LastName = Doe, SalaryPerHour = 132,7", "-update", "Id:2", "LastName:Doe", "Salary:132,7", "FirstName:John")]
        [TestCase("Id = 2, FirstName = John, LastName = Doe, SalaryPerHour = 132,7", "-update", "Id:2", "LastName:Doe", "FirstName:John", "Salary:132,7")]
        public void UpdateMultipleValuesTest(string expected, params string[] args)
        {
            DataManager manager = new DataManager(path);

            manager.MakeOperation(args);
            args[operationIndex] = operationGet;
            string actual = manager.MakeOperation(args);

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        [TestCase("Invalid value(  ) for property: FirstName", "-update", "Id:2", "FirstName:")]
        [TestCase("Invalid value( quitelot ) for property: SalaryPerHour", "-update", "Id:2", "LastName:Bond", "Salary:quitelot")]
        [TestCase("Request has no valid values", "-update", "Id:2")]
        [TestCase("There is no Employee with Id = 127", "-update", "Id:127", "LastName:Bond")]
        [TestCase("No arguments provided")]
        public void UpdateWrongValuesTest(string expected, params string[] args)
        {
            DataManager manager = new DataManager(path);

            string actual = manager.MakeOperation(args);

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        [TestCase("No such file {0}", "Moon", "-getall")]
        [TestCase("No such file {0}", "\\Lists\\NotRealFile.json", "-getall")]
        public void WrongPathTest(string expectedFormat, string wrongPath, params string[] args)
        {
            DataManager manager = new DataManager(wrongPath);

            string expected = string.Format(expectedFormat, manager.FilePath);
            string actual = manager.MakeOperation(args);

            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}