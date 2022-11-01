namespace CompanyApi.Model
{
    public class Employee
    {
        public Employee(string name, string salary)
        {
            Name = name;
            Salary = salary;
        }

        public string Name { get; set; }
        public string Salary { get; set; }
        public string? EmployeeId { get; set; }
    }
}