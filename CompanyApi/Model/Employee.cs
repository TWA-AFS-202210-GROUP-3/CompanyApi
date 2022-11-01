using System;

namespace CompanyApi.Model
{
    public class Employee
    {
        private readonly string employeeId;

        public Employee(string name, int salary)
        {
            this.Name = name;
            this.Salary = salary;
            this.employeeId = Guid.NewGuid().ToString();
        }

        public int Salary { get; set; }

        public string Name { get; set; }
    }
}
