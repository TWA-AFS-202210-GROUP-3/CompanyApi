using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace CompanyApi.Controllers
{
    [ApiController]
    [Route("companies")]

    public class Compancontroller : ControllerBase
    {
        private static List<Company> companies = new List<Company>();

        [HttpDelete]
        public List<Company> DeleteAll()
        {
            companies.Clear();
            return companies;
        }

        [HttpPost]
        public ActionResult<Company> AddNewCompanies(Company company)
        {
            var isExists = companies.Exists(_ => _.Name == company.Name);
            if (isExists)
            {
                return new ConflictResult();
            }

            company.Id = Guid.NewGuid().ToString();
            companies.Add(company);
            return new CreatedResult($"/companies/{company.Id}", company);
        }

        [HttpGet]
        public List<Company> GetCompanies([FromQuery] int? pageSize, [FromQuery] int? pageIndex)
        {
            if (pageSize != null && pageIndex != null)
            {
                return companies.Skip((pageIndex.Value - 1) * pageSize.Value).Take(pageSize.Value).ToList();
            }

            return companies;
        }

        [HttpGet("{Id}")]
        public ActionResult<Company> GetbyId([FromRoute] string id)
        {
            var foundCompany = companies.Find(_ => _.Id == id);
            if (foundCompany.Name == null)
            {
                return new NotFoundResult();
            }

            return foundCompany;
        }

        [HttpPatch("{Id}")]
        public ActionResult<Company> ChangeInformation([FromRoute] string id, Company companymodify)
        {
            var foundCompany = companies.Find(_ => _.Id == id);
            if (foundCompany == null)
            {
                return new NotFoundResult();
            }

            foundCompany.Name = companymodify.Name;

            return foundCompany;
        }

        [HttpPost("{Id}/employees")]
        public ActionResult<Company> AddNewEmployee([FromRoute] string id, Company companyaddemployee)
        {
            var foundCompany = companies.Find(_ => _.Id == id);
            if (foundCompany == null)
            {
                return new BadRequestResult();
            }

            foundCompany.Employees = companyaddemployee.Employees;

            return foundCompany;
        }

        [HttpGet("{Id}/employees")]
        public ActionResult<List<Employee>> GetEmployeebyId([FromRoute] string id)
        {
            var foundCompany = companies.Find(_ => _.Id == id);
            if (foundCompany.Name == null)
            {
                return new NotFoundResult();
            }

            return foundCompany.Employees;
        }

        [HttpPost("{Id}/employees/{EmployeeID}")]
        public ActionResult<Company> GetEmployeebyId([FromRoute] string id, [FromRoute] string employeeid, Company employeemodify)
        {
            var foundCompany = companies.Find(_ => _.Id == id);
            if (foundCompany.Name == null)
            {
                return new NotFoundResult();
            }

            foundCompany.Employees = employeemodify.Employees;
            return foundCompany;
        }

        [HttpDelete("{Id}/employees/{EmployeeID}")]
        public ActionResult<Company> GetEmployeebyId([FromRoute] string id, [FromRoute] string employeeid)
        {
            var foundCompany = companies.Find(_ => _.Id == id);
            var foundemployee = foundCompany.Employees.Find(_ => _.Id == employeeid);

            if (foundCompany.Name == null)
            {
                return new NotFoundResult();
            }

            foundCompany.Employees.Remove(foundemployee);

            return foundCompany;
        }
    }
}
