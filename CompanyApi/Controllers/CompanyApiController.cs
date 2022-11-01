using System;
using System.Collections.Generic;
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
    }
}
