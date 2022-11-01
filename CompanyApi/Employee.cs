namespace CompanyApi
{
    public class Employee
    {
        public Employee(string name, int salary)
        {
            Name = name;
            Salary = salary;
            EmployeeID = string.Empty;
        }

        public string Name { get; set; }
        public int Salary { get; set; }

        public string EmployeeID { get; set; }
    }
}
