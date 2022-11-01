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
        public ActionResult<Employee> AddNewEmployee([FromRoute] string id, Employee employee)
        {
            var companyFound = companies.Find(company => company.CompanyID == id);

            employee.EmployeeID = Guid.NewGuid().ToString();
            companyFound.Employees.Add(employee);
            return new CreatedResult($"/companies/{companyFound.CompanyID}/employees/{employee.EmployeeID}", employee);
        }

        [HttpGet("{id}/employees")]
        public List<Employee> GetAllEmployeee([FromRoute] string id)
        {
            var temp = companies.Find(company => company.CompanyID == id);
            return temp.Employees;
        }

        [HttpPut("{id}/employees/{employeeID}")]
        public Employee UpdateEmployee([FromRoute] string id, string employeeID, Employee employee)
        {
            var companyFound = companies.Find(company => company.CompanyID == id);
            var employeeFound = companyFound.Employees.Find(employee => employee.EmployeeID == employeeID);
            employeeFound.Name = employee.Name;
            return employeeFound;
        }

        [HttpDelete("{id}/employees/{employeeID}")]
        public void DeleteEmployee([FromRoute] string id, string employeeID)
        {
            var companyFound = companies.Find(company => company.CompanyID == id);
            var employeeFound = companyFound.Employees.Find(employee => employee.EmployeeID == employeeID);
            companyFound.Employees.Remove(employeeFound);
        }

        [HttpDelete("{id}")]
        public void DeleteCompany([FromRoute] string id)
        {
            var companyFound = companies.Find(company => company.CompanyID == id);
            companyFound.Employees.Clear();
            companies.Remove(companyFound);
        }
    }
}
