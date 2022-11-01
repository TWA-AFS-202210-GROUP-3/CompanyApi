using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
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
        public List<Company> GetCompanies([FromQuery]int? pageSize, int? pageIndex)
        {
            if (pageSize.HasValue && pageIndex.HasValue)
            {
                var start = (pageIndex - 1) * pageSize;
                var end = (pageIndex * pageSize) - 1;
                return companies.Where((company, index) => index >= start && index <= end)
                                .ToList();
            }

            return companies;
        }

        [HttpGet("{id}")]
        public Company GetAllCompanies([FromRoute] string id)
        {
            return companies.Find(company => company.CompanyID == id);
        }

        [HttpPut("{id}")]
        public Company UpdateCompany([FromRoute] string id, Company company)
        {
            var companyFound = companies.Find(company => company.CompanyID == id);
            companyFound.Name = company.Name;
            return companyFound;
        }

        [HttpPost("{id}/employees")]
        public Employee AddNewEmployee([FromRoute] string id, Employee employee)
        {
            var companyFound = companies.Find(company => company.CompanyID == id);
            companyFound.Employees.Add(employee);
            return employee;
        }
    }
}
