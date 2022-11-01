using Microsoft.AspNetCore.Hosting.Server;
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
using CompanyApi.Model;

namespace CompanyApiTest.Controllers
{
    public class CompanyControllerTest
    {
        [Fact]
        public async Task Should_add_company_successfullyAsync()
        {
            //given
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            var company = new Company("SLB");
            var companyJson = JsonConvert.SerializeObject(company);
            var requestBody = new StringContent(companyJson, Encoding.UTF8, "application/json");

            //when
            var reponse = await httpClient.PostAsync("/companies", requestBody);
            //then
            Assert.Equal(HttpStatusCode.Created, reponse.StatusCode);
            var responseBody = await reponse.Content.ReadAsStringAsync();
            var createdCompany = JsonConvert.DeserializeObject<Company>(responseBody);
            Assert.Equal("SLB", createdCompany.Name);
            Assert.NotEmpty(createdCompany.ID);
        }

        [Fact]
        public async Task Should_return_409_when_companyName_already_exist()
        {
            //given
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            var company = new Company("SLB");
            var companyJson = JsonConvert.SerializeObject(company);
            var requestBody = new StringContent(companyJson, Encoding.UTF8, "application/json");
            await httpClient.PostAsync("/companies", requestBody);

            //when
            var company1 = new Company("SLB");
            var companyJson1 = JsonConvert.SerializeObject(company1);
            var requestBody1 = new StringContent(companyJson1, Encoding.UTF8, "application/json");
            var reponse = await httpClient.PostAsync("/companies", requestBody1);
            //then
            Assert.Equal(HttpStatusCode.Conflict, reponse.StatusCode);
        }
    }
}
