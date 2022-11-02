﻿using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using CompanyApi.Model;

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

        [HttpPost("{companyId}/employees")]
        public List<Company> AddNewEmployee([FromRoute] string companyId, Employee employee)
        {
            employee.EmployeeId = Guid.NewGuid().ToString();
            companies.Find(item => item.CompanyID == companyId).Employees.Add(employee);
            return companies;
        }

        [HttpDelete]
        public void DeleteAllCompanies()
        {
            companies.Clear();
        }

        [HttpGet("{companyId}")]
        public Company GetCompanyByCompanyId([FromRoute] string companyId)
        {
            var companyGot = companies.Find(company => company.CompanyID == companyId);
            return companyGot;
        }

        [HttpGet("{companyId}/employees")]
        public List<Employee> GetAllEmployee([FromRoute] string companyId)
        {
            var employees = companies.Find(company => company.CompanyID == companyId).Employees;
            return employees;
        }

        [HttpGet]
        public List<Company> GetCompany([FromQuery] int? pageSize, [FromQuery] int? pageIndex)
        {
            if (pageSize != null && pageIndex != null)
            {
                return companies.
                    Skip((pageIndex.Value - 1) * pageSize.Value).
                    Take(pageSize.Value).ToList();
            }

            return companies;
        }

        [HttpPut("{companyId}")]
        public List<Company> UpdateCompany([FromRoute] string companyId, Company company)
        {
            companies.Find(item => item.CompanyID == company.CompanyID).Name = company.Name;
            return companies;
        }

        [HttpPatch("{companyId}/employees/{employeeId}")]
        public List<Company> UpdateEmployeeInfo([FromRoute] string companyId, [FromRoute] string employeeId, Company company)
        {
            var matchedCompany = companies.Find(item => item.CompanyID == companyId);
            matchedCompany.Employees.Find(item => item.EmployeeId == employeeId).Salary = company.Employees.Find(item => item.EmployeeId == employeeId).Salary;
            return companies;
        }

        [HttpDelete("{companyId}/employees/{employeeId}")]
        public List<Company> DeleteEmployee([FromRoute] string companyId, [FromRoute] string employeeId)
        {
            var matchedCompany = companies.Find(item => item.CompanyID == companyId);
            var matchedEmployee = matchedCompany.Employees.Find(item => item.EmployeeId == employeeId);
            matchedCompany.Employees.Remove(matchedEmployee);
            return companies;
        }

        [HttpDelete("{companyId}")]
        public void DeleteEmployee([FromRoute] string companyId)
        {
            var matchedCompany = companies.Find(item => item.CompanyID == companyId);
            companies.Remove(matchedCompany);
        }
    }
}
