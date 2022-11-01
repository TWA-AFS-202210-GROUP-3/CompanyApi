using System;
using System.Collections;
using System.Collections.Generic;

namespace CompanyApi.Controllers
{
    public class Employee
    {
        public Employee(string name, int salary)
        {
            Id = Guid.NewGuid().ToString();
            Name = name;
            Salary = salary;
        }

        public string Name { get; set; }
        public string Id { get; set; }

        public int Salary { get; set; }
    }
}
