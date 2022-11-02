namespace CompanyApi.Model
{
    public class Employee
    {
        public Employee(string name, float salary)
        {
            Name = name;
            Salary = salary;
        }

        public string? Id { get; set; }
        public string Name { get; set; }
        public float Salary { get; set; }
    }
}
