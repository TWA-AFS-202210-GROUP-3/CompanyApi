using CompanyApi.Model;
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
        [Route("{ID}")]
        public Company GetOneCompanyByID([FromRoute] string id)
        {
            return companies.Find(item => item.ID.Equals(id));
        }
    }
}
