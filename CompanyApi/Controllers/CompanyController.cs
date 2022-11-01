using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

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
            company.CompanyID = Guid.NewGuid().ToString();
            companies.Add(company);
            return new CreatedResult($"/companies/{company.CompanyID}", company);
        }
    }
}
