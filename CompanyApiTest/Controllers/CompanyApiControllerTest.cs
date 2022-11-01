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
            //given
            var httpClient = CreateHttpClient();
            var stringContent = PrepareCompany("SLB");
            //when
            await httpClient.DeleteAsync("/api/companies");
            await httpClient.PostAsync("/api/companies", stringContent);
            var response = await httpClient.PostAsync("/api/companies", stringContent);
            //then
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }

        [Fact]
        public async void Should_get_all_companies_successfully()
        {
            //given
            var httpClient = CreateHttpClient();
            var stringContent = PrepareCompany("SLB");
            //when
            await httpClient.DeleteAsync("/api/companies");
            await httpClient.PostAsync("/api/companies", stringContent);
            var response = await httpClient.GetAsync("/api/companies");
            //then
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async void Should_get_one_company_info_successfully()
        {
            //given
            var httpClient = CreateHttpClient();
            var stringContent = PrepareCompany("SLB");
            //when
            await httpClient.DeleteAsync("/api/companies");
            var postRes = await httpClient.PostAsync("/api/companies", stringContent);

            var responseContent = await postRes.Content.ReadAsStringAsync();
            var createdCompany = JsonConvert.DeserializeObject<Company>(responseContent);
            var response = await httpClient.GetAsync($"/api/companies/{createdCompany.CompanyID}");
            //then
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async void Should_get_one_company_info_failed()
        {
            //given
            var httpClient = CreateHttpClient();
            var stringContent = PrepareCompany("SLB");
            //when
            await httpClient.DeleteAsync("/api/companies");
            var postRes = await httpClient.PostAsync("/api/companies", stringContent);

            await postRes.Content.ReadAsStringAsync();
            var response = await httpClient.GetAsync($"/api/companies/{1}");
            //then
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
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
