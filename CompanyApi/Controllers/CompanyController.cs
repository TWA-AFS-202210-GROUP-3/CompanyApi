﻿using CompanyApi.Model;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;

namespace CompanyApi.Controllers
{
    [ApiController]
    [Route("companies")]
    public class CompanyController : Controller
    {
        private static List<Company> companies = new List<Company>();

        [HttpPost]
        public ActionResult<Company> AddCompany(Company company)
        {
            var nameAlreadyExist = companies.Exists(item => item.Name.Equals(company.Name));
            if (nameAlreadyExist)
            {
                return new ConflictResult();
            }
            else
            {
                company.ID = Guid.NewGuid().ToString();
                companies.Add(company);
                return new CreatedResult($"/companies/{company.ID}", company);
            }
        }

        [HttpDelete]
        public void DeleteAllCompanies()
        {
            companies.Clear();
        }

        [HttpDelete]
        [Route("{companyID}/employees/{employeeID}")]
        public void DeleteEmployee([FromRoute] string companyID, [FromRoute] string employeeID)
        {
            Company com = companies.Find(item => item.ID == companyID);
            if (com != null)
            {
                var employeeFound = com.Employees.Find(item => item.Id == employeeID);
                com.Employees.Remove(employeeFound);
            }
        }

        [HttpDelete]
        [Route("{companyID}")]
        public void DeleteOneCompanyandEmployees([FromRoute] string companyID)
        {
            Company com = companies.Find(item => item.ID == companyID);
            if (com != null)
            {
                com.Employees.Clear();
                companies.Remove(com);
            }
        }

        [HttpGet]
        public ActionResult<List<Company>> GetCompanies([FromQuery] int? pageSize, [FromQuery] int? pageIndex)
        {
            if (pageSize == null && pageIndex == null)
            {
                return companies;
            }
            else if (pageSize != null && pageIndex != null)
            {
                int skipCount = (int)(pageSize * (pageIndex - 1));
                if (companies.Count - skipCount > 0)
                {
                    if (companies.Count - skipCount <= pageSize)
                    {
                        return companies.GetRange(skipCount, companies.Count - skipCount);
                    }
                    else
                    {
                        return companies.GetRange(skipCount, (int)pageSize);
                    }
                }
            }

            return new BadRequestResult();
        }

        [HttpGet]
        [Route("{ID}/employees")]
        public ActionResult<List<Employee>> GetAllEmployees([FromRoute] string id)
        {
            Company com = companies.Find(item => item.ID == id);
            if (com != null)
            {
                return com.Employees;
            }

            return BadRequest();
        }

        [HttpGet]
        [Route("{ID}")]
        public Company GetOneCompanyByID([FromRoute] string id)
        {
            return companies.Find(item => item.ID.Equals(id));
        }

        [HttpPut]
        [Route("{ID}")]
        public ActionResult<Company> UpdateCompanyInfo([FromRoute] string id, [FromBody] Company company)
        {
            Company com = companies.Find(item => item.ID == id);
            if (com != null)
            {
                com.Name = company.Name;
                return com;
            }

            return BadRequest();
        }

        [HttpPut]
        [Route("{companyID}/employees/{employeeID}")]
        public ActionResult<Employee> UpdateEmployeeInfo([FromRoute] string companyID, [FromRoute] string employeeID, [FromBody] Employee employee)
        {
            Company com = companies.Find(item => item.ID == companyID);
            if (com != null)
            {
                var employeeFound = com.Employees.Find(item => item.Id == employeeID);
                employeeFound.Name = employee.Name;
                employeeFound.Salary = employee.Salary;
                return employeeFound;
            }

            return BadRequest();
        }

        [HttpPost]
        [Route("{ID}/employees")]
        public ActionResult<Employee> AddEmployee([FromBody] Employee employee, [FromRoute] string id)
        {
            Company com = companies.Find(item => item.ID == id);
            if (com != null)
            {
                employee.Id = Guid.NewGuid().ToString();
                com.Employees.Add(employee);
                return employee;
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
