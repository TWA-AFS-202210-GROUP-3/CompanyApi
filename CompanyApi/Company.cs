using System;

namespace CompanyApi.Controllers
{
    public class Company
    {
        public Company(string name)
        {
            CompanyID = Guid.NewGuid().ToString();
            Name = name;
        }

        public string Name { get; set; }
        public string CompanyID { get; set; }
    }
}