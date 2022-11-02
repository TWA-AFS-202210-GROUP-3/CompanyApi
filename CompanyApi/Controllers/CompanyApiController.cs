﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CompanyApi.Model;
using Microsoft.AspNetCore.Mvc;

namespace CompanyApi.Controllers
{
    [ApiController]
    [Route("api")]
    public class CompanyApiController
    {
        private static List<Company> companyList = new List<Company>();

        [HttpPost("companies")]
        public ActionResult<Company> AddNewCompany(Company company)
        {
            if (companyList.FindAll(item => item.Name == company.Name).Count != 0)
            {
                return new ConflictResult();
            }

            company.CompanyID = Guid.NewGuid().ToString();
            companyList.Add(company);
            return new CreatedResult($"/api/companies/{company.CompanyID}", company);
        }

        [HttpGet("companies")]
        public ActionResult<List<Company>> GetAllCompanies([FromQuery] int? page, [FromQuery] int? size)
        {
            if (page != null && size != null)
            {
                try
                {
                    var matchedCompanies = companyList.GetRange((page.Value - 1) * page.Value, size.Value);
                    return matchedCompanies.Count > 0 ? matchedCompanies : new List<Company>();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return new List<Company>();
                }
            }

            return companyList;
        }

        [HttpGet("companies/{id}")]
        public ActionResult<Company> GetCompanyInfoById([FromRoute] string id)
        {
            var matchedCompany = companyList.Find(company => company.CompanyID == id);
            if (matchedCompany == null)
            {
                return new NotFoundResult();
            }

            return matchedCompany;
        }

        [HttpPut("companies/{id}")]
        public ActionResult<Company> UpdateCompanyInfo(Company company)
        {
            var matchedCompanyIndex = companyList.FindIndex(item => item.CompanyID == company.CompanyID);
            companyList[matchedCompanyIndex].Name = company.Name;
            return companyList[matchedCompanyIndex];
        }

        [HttpPut("companies/{id}/employees")]
        public ActionResult<Company> UpdateEmployeeInCompany([FromRoute] string id, Employee employee)
        {
            var matchedCompanyIndex = companyList.FindIndex(item => item.CompanyID == id);
            companyList[matchedCompanyIndex].Employees.Add(employee);
            return companyList[matchedCompanyIndex];
        }

        [HttpGet("companies/{id}/employees")]
        public ActionResult<List<Employee>> GetAllEmployeesInCompany([FromRoute] string id)
        {
            var matchedCompany = companyList.Find(item => item.CompanyID == id);
            return matchedCompany.Employees;
        }

        [HttpPatch("companies/{companyId}/employees/{employeeId}")]
        public ActionResult<Employee> UpdateEmployeeInfoById([FromRoute] string companyId, [FromRoute] string employeeId, Employee employee)
        {
            var matchedCompanyIndex = companyList.FindIndex(company => company.CompanyID == companyId);
            var matchedEmployeeIndex = companyList[matchedCompanyIndex].Employees
                .FindIndex(item => item.EmployeeId == employeeId);
            companyList[matchedCompanyIndex].Employees[matchedEmployeeIndex] = employee;

            return companyList[matchedCompanyIndex].Employees[matchedEmployeeIndex];
        }

        [HttpDelete("companies/{companyId}/employees/{employeeId}")]
        public void DeleteEmployeeById([FromRoute] string companyId, [FromRoute] string employeeId)
        {
            var matchedCompanyIndex = companyList.FindIndex(company => company.CompanyID == companyId);
            var matchedEmployee = companyList[matchedCompanyIndex].Employees
                .Find(item => item.EmployeeId == employeeId);
            companyList[matchedCompanyIndex].Employees.Remove(matchedEmployee);
        }

        [HttpDelete("companies/{id}")]
        public void DeleteCompanyById([FromRoute] string id)
        {
            var matchCompany = companyList.Find(company => company.CompanyID == id);
            companyList.Remove(matchCompany);
        }

        [HttpDelete("companies")]
        public void DeleteAllCompanies()
        {
            companyList.Clear();
        }
    }
}
