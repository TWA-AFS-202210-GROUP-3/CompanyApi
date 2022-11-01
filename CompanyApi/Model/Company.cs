using System;
using System.Collections.Generic;

namespace CompanyApi.Model
{
    public class Company
    {
        public Company(string name)
        {
            this.Name = name;
            this.CompanyID = string.Empty;
            this.Employees = new List<Employee>();
        }

        public Company()
        {
        }

        public string Name { get; set; }
        public string CompanyID { get; set; }
        public List<Employee> Employees { get; set; }
    }
}
