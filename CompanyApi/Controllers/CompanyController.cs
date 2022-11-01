using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace CompanyApi.Controllers
{
    [ApiController]
    [Route("companies")]
    public class CompanyController
    {
        private static List<Company> companies = new List<Company>();
        
        [HttpPost]
        public ActionResult<Company> AddNewCompany(Company company)
        {
            var companyNameExists = companies.Exists(companyToCompare => companyToCompare.Name.Equals(company.Name));
            if (companyNameExists)
            {
                return new ConflictResult();
            }
            else
            {
                company.CompanyID = Guid.NewGuid().ToString();
                companies.Add(company);
                return new CreatedResult($"/companies/{company.CompanyID}", company);
            }
        }

        [HttpDelete]
        public void DeleteAllCompanies()
        {
            companies.Clear();
        }

        [HttpGet]
        public List<Company> GetAllCompanies()
        {
            return companies;
        }
    }
}
