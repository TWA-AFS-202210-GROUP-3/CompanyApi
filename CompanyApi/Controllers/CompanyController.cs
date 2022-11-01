using System;
using Microsoft.AspNetCore.Mvc;
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

        //[HttpGet]
        //public List<Company> GetAllCompanies()
        //{
        //    return companies;
        //}

        [HttpGet("{companyId}")]
        public Company GetCompanyByCompanyId([FromRoute] string companyId)
        {
            var companyGot = companies.Find(company => company.CompanyID == companyId);
            return companyGot;
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
    }
}
