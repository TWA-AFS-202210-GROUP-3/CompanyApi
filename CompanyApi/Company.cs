using System;
using System.Collections;
using System.Collections.Generic;

namespace CompanyApi
{
    public class Company
    {
        public Company(string name)
        {
            Name = name;
            CompanyID = string.Empty;
            Employees = new List<Employee>();
        }

        public string Name { get; set; }
        public string CompanyID { get; set; }
        public List<Employee> Employees { get; set; }

    }
}