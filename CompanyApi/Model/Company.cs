using System;

namespace CompanyApi.Model
{
    public class Company
    {
        public Company(string name)
        {
            this.Name = name;
            this.CompanyID = string.Empty;
        }

        public Company()
        {
        }

        public string Name { get; set; }
        public string CompanyID { get; set; }
    }
}
