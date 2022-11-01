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
using Guid = System.Guid;

namespace CompanyApiTest.Controllers
{
    public class CompanyApiControllerTest
    {
        [Fact]
        public async void Should_create_company_successfully()
        {
            //given
            var httpClient = CreateHttpClient();
            var stringContent = PrepareCompany("SLB-3");

            //when
            await httpClient.DeleteAsync("/api/companies");
            var response = await httpClient.PostAsync("/api/companies", stringContent);
            var responseContent = await response.Content.ReadAsStringAsync();
            var createdCompany = JsonConvert.DeserializeObject<Company>(responseContent);

            //then
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.Equal("SLB-3", createdCompany.Name);
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
            await httpClient.PostAsync("/api/companies", stringContent);
            var response = await httpClient.GetAsync($"/api/companies/{1}");
            //then
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async void Should_return_1_company_for_page_2()
        {
            //given
            var httpClient = CreateHttpClient();
            var page1Company = PrepareCompany("SLB");
            var page2Company = PrepareCompany("SLB-1");
            //when
            await httpClient.DeleteAsync("/api/companies");
            var post1Res = httpClient.PostAsync("/api/companies", page1Company);
            while (post1Res.IsCompleted)
            {
                var postRes = await httpClient.PostAsync("/api/companies", page2Company);

                var postResContent = await postRes.Content.ReadAsStringAsync();
                var createdCompany = JsonConvert.DeserializeObject<Company>(postResContent);

                var response = await httpClient.GetAsync($"/api/companies?page=2&size=1");
                var pageResponseContent = await response.Content.ReadAsStringAsync();
                var matchedCompanies = JsonConvert.DeserializeObject<List<Company>>(pageResponseContent);
                //then
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal(matchedCompanies[0].CompanyID, createdCompany.CompanyID);
            }
        }

        [Fact]
        public async void Should_return_empty_list_for_page_oversize()
        {
            //given
            var httpClient = CreateHttpClient();
            var page1Company = PrepareCompany("SLB");
            var page2Company = PrepareCompany("SLB-1");
            //when
            await httpClient.DeleteAsync("/api/companies");
            var post1Res = httpClient.PostAsync("/api/companies", page1Company);
            while (post1Res.IsCompleted)
            {
                var postRes = await httpClient.PostAsync("/api/companies", page2Company);

                var postResContent = await postRes.Content.ReadAsStringAsync();
                var createdCompany = JsonConvert.DeserializeObject<Company>(postResContent);
            }

            var response = await httpClient.GetAsync($"/api/companies?page=3&size=4");
            var pageResponseContent = await response.Content.ReadAsStringAsync();
            var matchedCompanies = JsonConvert.DeserializeObject<List<Company>>(pageResponseContent);
            //then
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(new List<Company>(), matchedCompanies);
        }

        [Fact]
        public async void Should_update_exist_company_name_successfully()
        {
            //given
            var httpClient = CreateHttpClient();
            Company company = new Company("SLB");

            var serializeObject = JsonConvert.SerializeObject(company);
            var stringContent = new StringContent(serializeObject, Encoding.UTF8, "application/json");
            //when
            await httpClient.DeleteAsync("/api/companies");
            var postRes = await httpClient.PostAsync("/api/companies", stringContent);

            var postResContent = await postRes.Content.ReadAsStringAsync();
            var createdCompany = JsonConvert.DeserializeObject<Company>(postResContent);

            createdCompany.Name = "Schlumberger";

            var newSerializeObject = JsonConvert.SerializeObject(createdCompany);
            var newStringContent = new StringContent(newSerializeObject, Encoding.UTF8, "application/json");

            var updatedResContent =
                await httpClient.PutAsync($"/api/companies/{createdCompany.CompanyID}", newStringContent);
            var updatedRes = await updatedResContent.Content.ReadAsStringAsync();
            var updatedCompany = JsonConvert.DeserializeObject<Company>(updatedRes);
            //then
            Assert.Equal("Schlumberger", updatedCompany.Name);
        }

        [Fact]
        public async void Should_insert_employees_to_company_successfully()
        {
            //given
            var httpClient = CreateHttpClient();
            var stringContent = PrepareCompany("SLB");
            Employee employee = new Employee(name: "Bob", salary: 5000);

            //when
            await httpClient.DeleteAsync("/api/companies");
            var postRes = await httpClient.PostAsync("/api/companies", stringContent);

            var postResContent = await postRes.Content.ReadAsStringAsync();
            var createdCompany = JsonConvert.DeserializeObject<Company>(postResContent);

            createdCompany.Employees.Add(employee);

            var updatedCompany = JsonConvert.SerializeObject(createdCompany);
            var updatedCompanyStringContent = new StringContent(updatedCompany, Encoding.UTF8, "application/json");
            var updatedResponseMessage =
                await httpClient.PutAsync($"/api/companies/{createdCompany.CompanyID}/employees", updatedCompanyStringContent);
            var updatedResponseContent = await updatedResponseMessage.Content.ReadAsStringAsync();
            var updatedEmployeeCompany = JsonConvert.DeserializeObject<Company>(updatedResponseContent);

            //then
            Assert.Single(updatedEmployeeCompany.Employees);
        }

        [Fact]
        public async void Should_get_all_employees_from_company_successfully()
        {
            //given
            var httpClient = CreateHttpClient();
            var stringContent = PrepareCompany("SLB");
            Employee employee = new Employee(name: "Bob", salary: 5000);

            //when
            await httpClient.DeleteAsync("/api/companies");
            var postRes = await httpClient.PostAsync("/api/companies", stringContent);

            var postResContent = await postRes.Content.ReadAsStringAsync();
            var createdCompany = JsonConvert.DeserializeObject<Company>(postResContent);

            createdCompany.Employees.Add(employee);

            var updatedCompany = JsonConvert.SerializeObject(createdCompany);
            var updatedCompanyStringContent = new StringContent(updatedCompany, Encoding.UTF8, "application/json");

            await httpClient.PutAsync($"/api/companies/{createdCompany.CompanyID}/employees", updatedCompanyStringContent);

            var allEmployeeMessage = await httpClient.GetAsync($"/api/companies/{createdCompany.CompanyID}/employees");
            var allEmployeesContent = await allEmployeeMessage.Content.ReadAsStringAsync();
            var employees = JsonConvert.DeserializeObject<List<Employee>>(allEmployeesContent);
            //then
            Assert.Equal(HttpStatusCode.OK, allEmployeeMessage.StatusCode);
            Assert.Single(employees);
        }

        [Fact]
        public async void Should_get_employee_from_company_successfully()
        {
            //given
            var httpClient = CreateHttpClient();
            var stringContent = PrepareCompany("SLB");
            Employee employee = new Employee(name: "Bob", salary: 5000);

            //when
            await httpClient.DeleteAsync("/api/companies");
            var postRes = await httpClient.PostAsync("/api/companies", stringContent);

            var postResContent = await postRes.Content.ReadAsStringAsync();
            var createdCompany = JsonConvert.DeserializeObject<Company>(postResContent);

            createdCompany.Employees.Add(employee);

            var updatedCompany = JsonConvert.SerializeObject(createdCompany);
            var updatedCompanyStringContent = new StringContent(updatedCompany, Encoding.UTF8, "application/json");

            await httpClient.PutAsync($"/api/companies/{createdCompany.CompanyID}/employees", updatedCompanyStringContent);

            var allEmployeeMessage = await httpClient.GetAsync($"/api/companies/{createdCompany.CompanyID}/employees");
            var allEmployeesContent = await allEmployeeMessage.Content.ReadAsStringAsync();
            var employees = JsonConvert.DeserializeObject<List<Employee>>(allEmployeesContent);

            employees[0].Name = "OutMan";
            employees[0].Salary = 10000;

            var employeeUpdatedStringContent = new StringContent(JsonConvert.SerializeObject(employees[0]), Encoding.UTF8, "application/json");
            var employeeInfoMessage = await httpClient.PatchAsync(
                $"/api/companies/{createdCompany.CompanyID}/employees/{employees[0].EmployeeId}",
                employeeUpdatedStringContent);
            var employeeInfoContent = await employeeInfoMessage.Content.ReadAsStringAsync();
            var employeeInfo = JsonConvert.DeserializeObject<Employee>(employeeInfoContent);

            //then
            Assert.Equal("OutMan", employeeInfo.Name);
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
