using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;

namespace CompanyApi.Controllers
{
    public class Company
    {
        public Company(string name)
        {
            Name = name;
            Id = Guid.NewGuid().ToString();
            Employees = new List<Employee>();
        }

        public string Name { get; set; }
        public string Id { get; set; }
        public List<Employee> Employees { get; set; }
    }
}