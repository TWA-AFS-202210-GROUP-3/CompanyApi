using System;
using System.Collections;

namespace CompanyApi
{
    public class Company
    {
        public Company(string name)
        {
            Name = name;
            CompanyID = string.Empty;
        }

        public string Name { get; set; }
        public string CompanyID { get; set; }
    }
}