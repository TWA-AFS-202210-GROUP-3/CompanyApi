using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CompanyApi.Controllers;
using Xunit;

namespace CompanyApiTest.Controllers
{
    public class CompanyControllerTest
    {
        [Fact]
        public async void Should_add_new_company_successfully()
        {
            // given
            var application = new WebApplicationFactory<Program>();
            var httpclient = application.CreateClient();
            await httpclient.DeleteAsync("companies");
            var company = new Company(name: "SLB");
            var companyJson = JsonConvert.SerializeObject(company);
            var postBody = new StringContent(companyJson, Encoding.UTF8, "application/json");
            //when
            var response = await httpclient.PostAsync("/companies", postBody);
            //then
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var responseBody = await response.Content.ReadAsStringAsync();
            var createdCompany = JsonConvert.DeserializeObject<Company>(responseBody);
            Assert.NotEmpty(createdCompany.CompanyID);
        }

        [Fact]
        public async void Should_return_conflict_when_company_already_exsited()
        {
            // given
            var application = new WebApplicationFactory<Program>();
            var httpclient = application.CreateClient();
            await httpclient.DeleteAsync("companies");
            var company = new Company(name: "SLB");
            var companyJson = JsonConvert.SerializeObject(company);
            var postBody = new StringContent(companyJson, Encoding.UTF8, "application/json");
            //when
            await httpclient.PostAsync("/companies", postBody);
            var response = await httpclient.PostAsync("/companies", postBody);
            //then
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }

        [Fact]
        public async void Should_obtain_all_exsiting_company()
        {
            // given
            var application = new WebApplicationFactory<Program>();
            var httpclient = application.CreateClient();
            await httpclient.DeleteAsync("companies");
            var companies = new List<Company>
            {
                new Company(name: "Katy"),
                new Company(name: "Toms"),
                new Company(name: "Andy")
            };
            foreach (var company in companies)
            {
                var companyJson = JsonConvert.SerializeObject(company);
                var postBody = new StringContent(companyJson, Encoding.UTF8, "application/json");
                await httpclient.PostAsync("/companies", postBody);
            }

            //when
            var response = await httpclient.GetAsync("/companies");
            //then
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var responseBody = await response.Content.ReadAsStringAsync();
            var companinesObtained = JsonConvert.DeserializeObject<List<Company>>(responseBody);
            Assert.Equal(3, companinesObtained.Count);
        }
    }
}
