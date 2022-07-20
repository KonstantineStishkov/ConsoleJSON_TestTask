using EmployeeData;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserInterface
{
    public enum EmployeeProperties
    {
        Id = 0,
        FirstName = 1,
        LastName = 2,
        SalaryPerHour = 3
    }

    public class DataManager
    {
        const string successMessage = "Operation is successfull";

        const string errorOperationMessage = "No such operation";
        const string errorNoFileFormatMessage = "No such file {0}";
        const string errorArgumentFormatMessage = "Invalid value( {0} ) for property: {1}";
        const string errorNoValueFormatMessage = "No value for property: {1} provided";
        const string errorIdFormatMessage = "There is no Employee with Id = {0}";
        const string errorEmptyListMessage = "List of Employees is empty";
        const string errorNoValidValuesMessage = "Request has no valid values";

        const string operationAdd = "-add";
        const string operationUpdate = "-update";
        const string operationGet = "-get";
        const string operationDelete = "-delete";
        const string operationGetAll = "-getall";

        const int firstId = 1;

        public string FilePath { get; private set; }
        public DataManager(string filePathRelative)
        {
            FilePath = Path.GetFullPath(filePathRelative);
        }

        public string MakeOperation(string[] input)
        {
            if (!File.Exists(FilePath))
                return string.Format(errorNoFileFormatMessage, FilePath);

            string[] values = GetVauesOfProperties(input);
            string operation = input[0];

            switch (operation)
            {
                case operationAdd:
                    return Add(values);

                case operationUpdate:
                    return Update(values);

                case operationGet:
                    string value = values[(int)EmployeeProperties.Id];
                    if (int.TryParse(value, out int id))
                        return Get(id);
                    else
                        return string.Format(errorArgumentFormatMessage, value, EmployeeProperties.Id);

                case operationDelete:
                    value = values[(int)EmployeeProperties.Id];
                    if (int.TryParse(value, out id))
                        return Delete(id);
                    else
                        return string.Format(errorArgumentFormatMessage, value, EmployeeProperties.Id);

                case operationGetAll:
                    return GetAll();

                default:
                    return errorOperationMessage;
            }
        }

        private string[] GetVauesOfProperties(string[] input)
        {
            const int leftSubStringIndex = 0;
            const int rightSubStringIndex = 1;

            System.Reflection.PropertyInfo[]? info = typeof(Employee).GetProperties();
            string[] values = new string[info.Length];

            foreach (string? text in input)
            {
                int index = Array.FindIndex(info, p => p.Name == text.Split(':')[leftSubStringIndex]);
                if (index != -1)
                    values[index] = text.Split(':')[rightSubStringIndex];
            }

            return values;
        }

        private string Add(string[] values)
        {
            string value = values[(int)EmployeeProperties.FirstName];
            if (value == null)
                return string.Format(errorNoValueFormatMessage, EmployeeProperties.FirstName);

            value = values[(int)EmployeeProperties.LastName];
            if (value == null)
                return string.Format(errorNoValueFormatMessage, EmployeeProperties.LastName);

            value = values[(int)EmployeeProperties.SalaryPerHour];

            if (value == null)
                return string.Format(errorNoValueFormatMessage, EmployeeProperties.SalaryPerHour);

            if (!decimal.TryParse(value, out decimal salary))
                return string.Format(errorArgumentFormatMessage, value, EmployeeProperties.SalaryPerHour);

            int id;
            List<Employee>? employees = GetEmployeeList();

            if (employees == null)
            {
                id = firstId;
                employees = new List<Employee>();
            }
            else
            {
                id = employees.Max(e => e.Id) + 1;
            }

            employees.Add(new Employee(id,
                                       values[(int)EmployeeProperties.FirstName],
                                       values[(int)EmployeeProperties.LastName],
                                       salary));
            SaveEmployeeList(employees);

            return successMessage;
        }

        private string Update(string[] values)
        {
            bool hasValidValues = false;
            string value = values[(int)EmployeeProperties.Id];
            if (!int.TryParse(value, out int id))
                return string.Format(errorArgumentFormatMessage, value, EmployeeProperties.Id);

            List<Employee>? employees = GetEmployeeList();

            if (employees == null)
                return errorEmptyListMessage;

            Employee? employee = employees.FirstOrDefault(e => e.Id == id);

            if (employee == null)
                return string.Format(errorIdFormatMessage, id);

            value = values[(int)EmployeeProperties.FirstName];
            if (value != null)
                if (value != string.Empty)
                {
                    hasValidValues = true;
                    employee.FirstName = value;
                }
                else
                    return string.Format(errorArgumentFormatMessage, value, EmployeeProperties.FirstName);

            value = values[(int)EmployeeProperties.LastName];
            if (value != null)
                if (value != string.Empty)
                {
                    hasValidValues = true;
                    employee.LastName = value;
                }
                else
                    return string.Format(errorArgumentFormatMessage, value, EmployeeProperties.LastName);

            value = values[(int)EmployeeProperties.SalaryPerHour];
            if (value != null)
                if (decimal.TryParse(value, out decimal salary))
                {
                    hasValidValues = true;
                    employee.Salary = salary;
                }
                else
                    return string.Format(errorArgumentFormatMessage, value, EmployeeProperties.SalaryPerHour);

            if (!hasValidValues)
            {
                return errorNoValidValuesMessage;
            }

            return SaveEmployeeList(employees) ? successMessage : errorOperationMessage;

        }
        private string Delete(int id)
        {
            List<Employee>? employees = GetEmployeeList();

            if (employees == null)
                return errorEmptyListMessage;

            employees.RemoveAll(employee => employee.Id == id);
            SaveEmployeeList(employees);

            return successMessage;
        }

        private string Get(int id)
        {
            List<Employee>? employees = GetEmployeeList();

            if (employees == null)
                return errorEmptyListMessage;

            foreach (Employee item in employees)
            {
                if (item.Id == id)
                    return item.ToString();
            }

            return string.Format("There is no Employee with id = {0}", id);
        }

        private string GetAll()
        {
            List<Employee>? employees = GetEmployeeList();
            StringBuilder builder = new StringBuilder();

            if (employees == null)
                return errorEmptyListMessage;

            foreach (Employee item in employees)
            {
                builder.Append(item.ToString());
                builder.Append('\n');
            }

            if (builder.Length == 0)
                return string.Format("Employee list is empty");
            else
                return builder.ToString();
        }

        private List<Employee>? GetEmployeeList()
        {
            List<Employee>? employees;
            using (FileStream stream = new FileStream(FilePath, FileMode.Open, FileAccess.Read))
            {
                JsonSerializer serializer = new JsonSerializer();
                JsonTextReader jsonReader = new JsonTextReader(new StreamReader(stream));
                employees = serializer.Deserialize<List<Employee>>(jsonReader);
            }

            return employees;
        }

        private bool SaveEmployeeList(List<Employee> employees)
        {
            using (JsonTextWriter jsonWriter = new JsonTextWriter(File.CreateText(FilePath)))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(jsonWriter, employees);
            }

            return true;
        }
    }
}
