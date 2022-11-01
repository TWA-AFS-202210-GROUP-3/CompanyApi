using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

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
            if (companies.Select(company => company.Name).ToList().Contains(company.Name))
            {
                return new ConflictResult();
            }

            company.CompanyID = Guid.NewGuid().ToString();
            companies.Add(company);
            return new CreatedResult($"/companies/{company.CompanyID}", company);
        }

        [HttpDelete]
        public void DelAllCompanies()
        {
            companies.Clear();
        }

        [HttpGet]
        public List<Company> GetAllCompanies()
        {
            return companies;
        }

        [HttpGet("{id}")]
        public Company GetAllCompanies([FromRoute] string id)
        {
            return companies.Find(company => company.CompanyID == id);
        }
    }
}
