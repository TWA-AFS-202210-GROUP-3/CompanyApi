using CompanyApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CompanyApiTest.Controllers
{
    public class CompanyControllerTest
    {
        [Fact]
        public async Task Should_post_a_company_successfullyAsync()
        {
            //given
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            var company = new Company("SLB");
            var companyJson = JsonConvert.SerializeObject(company);
            var postBody = new StringContent(companyJson, Encoding.UTF8, "application/json");

            //when
            var response = await httpClient.PostAsync("/companies", postBody);

            //then
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var responseBody = await response.Content.ReadAsStringAsync();
            var createdCompany = JsonConvert.DeserializeObject<Company>(responseBody);
            Assert.Equal("SLB", createdCompany.Name);
            Assert.NotEmpty(createdCompany.CompanyID);
        }

        [Fact]
        public async Task Should_not_post_a_company_name_already_exist_Async()
        {
            //given
            HttpClient httpClient = await CreateHttpClient();
            StringContent postBody = await PostCompany(httpClient, new Company("SLB"));

            //when
            var response = await httpClient.PostAsync("/companies", postBody);

            //then
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }

        [Fact]
        public async Task Should_get_all_companies_Async()
        {
            //given
            HttpClient httpClient = await CreateHttpClient();
            await PostCompany(httpClient, new Company("SLB"));
            await PostCompany(httpClient, new Company("AAA"));

            //when
            var response = await httpClient.GetAsync("/companies");

            //then
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var responseBody = await response.Content.ReadAsStringAsync();
            var companies = JsonConvert.DeserializeObject<List<Company>>(responseBody);
            Assert.Equal(2, companies.Count);
        }

        [Fact]
        public async Task Should_get_a_existing_company_by_companyID_Async()
        {
            //given
            HttpClient httpClient = await CreateHttpClient();
            await PostCompany(httpClient, new Company("SLB"));
            await PostCompany(httpClient, new Company("AAA"));

            var response = await httpClient.GetAsync("/companies");
            var responseBody = await response.Content.ReadAsStringAsync();
            var companies = JsonConvert.DeserializeObject<List<Company>>(responseBody);

            var companyShouldBeGot = companies[0];

            //when
            var newResponse = await httpClient.GetAsync($"/companies/{companyShouldBeGot.CompanyID}");

            //then
            Assert.Equal(HttpStatusCode.OK, newResponse.StatusCode);
            var newResponseBody = await newResponse.Content.ReadAsStringAsync();
            var companyGot = JsonConvert.DeserializeObject<Company>(newResponseBody);
            Assert.Equal(companyShouldBeGot.Name, companyGot.Name);
        }

        private static async Task<HttpClient> CreateHttpClient()
        {
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            await httpClient.DeleteAsync("/companies");
            return httpClient;
        }

        private static async Task<StringContent> PostCompany(HttpClient httpClient, Company company)
        {
            var companyJson = JsonConvert.SerializeObject(company);
            var postBody = new StringContent(companyJson, Encoding.UTF8, "application/json");
            await httpClient.PostAsync("/companies", postBody);
            return postBody;
        }
    }
}
