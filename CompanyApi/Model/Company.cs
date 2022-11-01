using System;
using System.Collections.Generic;

namespace CompanyApi.Model
{
    public class Company
    {
        public Company(string name)
        {
            Name = name;
            Employees = new List<Employee>();
        }

        public string Name { get; set; }
        public string? CompanyID { get; set; }
        public List<Employee>? Employees { get; set; }
    }
}