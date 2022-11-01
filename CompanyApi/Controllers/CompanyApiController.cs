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
        private List<Company> companyList = new List<Company>();

        [HttpPost("companies")]
        public ActionResult<Company> AddNewCompany(Company company)
        {
            company.CompanyID = Guid.NewGuid().ToString();
            companyList.Add(company);
            return new CreatedResult($"/api/companies/{company.CompanyID}", company);
        }
    }
}
