using System;

namespace CompanyApi.Model
{
    public class Employee
    {
        public Employee()
        {
        }

        public Employee(string employeeName, double salary)
        {
            EmployeeSalary = salary;
            EmployeeName = employeeName;
            EmployeeID = Guid.NewGuid().ToString();
        }

        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public double EmployeeSalary { get; set; }
    }
}