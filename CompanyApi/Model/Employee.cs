using System;

namespace CompanyApi.Model
{
    public class Employee
    {
        public Employee(string name, int salary)
        {
            this.Name = name;
            this.Salary = salary;
            this.EmployeeId = Guid.NewGuid().ToString();
        }

        public string EmployeeId { get; set; }

        public int Salary { get; set; }

        public string Name { get; set; }
    }
}
