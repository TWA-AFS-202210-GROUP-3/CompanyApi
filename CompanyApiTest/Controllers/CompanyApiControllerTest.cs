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
            var httpClient = CreateHttpClient();
            var stringContent = PrepareCompany("SLB");

            var response = await httpClient.PostAsync("/api/companies", stringContent);
            var responseContent = await response.Content.ReadAsStringAsync();
            var createdCompany = JsonConvert.DeserializeObject<Company>(responseContent);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.Equal("SLB", createdCompany.Name);
            Assert.NotNull(createdCompany.CompanyID);
        }

        [Fact]
        public async void Should_return_409_when_create_duplicate_company()
        {
            var httpClient = CreateHttpClient();
            var stringContent = PrepareCompany("SLB");

            //var stringContentList = new List<StringContent>{ PrepareCompany("SLB"), PrepareCompany("SLB") };
            await httpClient.PostAsync("/api/companies", stringContent);
            var response = await httpClient.PostAsync("/api/companies", stringContent);

            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }

        private HttpClient CreateHttpClient()
        {
            var application = new WebApplicationFactory<Program>();
            return application.CreateClient();
        }

        private StringContent PrepareCompany(string name)
        {
            Company company = new Company(name);

            var serializeObject = JsonConvert.SerializeObject(company);
            return new StringContent(serializeObject, Encoding.UTF8, "application/json");
        }
    }
}
