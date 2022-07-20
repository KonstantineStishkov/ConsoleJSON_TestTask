using Newtonsoft.Json;

namespace EmployeeData
{
    public class Employee
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [JsonProperty("SalaryPerHour")]
        public decimal Salary { get; set; }
        public Employee(int id, string firstName, string lastName, decimal salaryPerHour)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Salary = salaryPerHour;
        }

        public override string ToString()
        {
            return string.Format("Id = {0}, FirstName = {1}, LastName = {2}, SalaryPerHour = {3}",
                                  Id,
                                  FirstName,
                                  LastName,
                                  Salary);
        }
    }
}