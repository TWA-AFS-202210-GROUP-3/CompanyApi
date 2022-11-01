using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CompanyApi.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace CompanyApiTest.Controllers
{
    public class CompanyApiControllerTest
    {
        [Fact]
        public async void Should_create_company_successfully()
        {
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();

            Company company = new Company(name: "SLB");

            var serializeObject = JsonConvert.SerializeObject(company);
            var stringContent = new StringContent(serializeObject, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("/api/companies", stringContent);
            var responseContent = await response.Content.ReadAsStringAsync();
            var createdCompany = JsonConvert.DeserializeObject<Company>(responseContent);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.Equal("SLB", createdCompany.Name);
            Assert.NotNull(createdCompany.CompanyID);
        }
    }
}
