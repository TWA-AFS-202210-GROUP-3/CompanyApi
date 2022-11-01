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
using Microsoft.AspNetCore.Mvc.ViewFeatures;

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
            await httpClient.DeleteAsync("/companies");
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
            await httpClient.DeleteAsync("/companies");
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

        [Fact]
        public async Task Should_return_all_companies()
        {
            //given
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            await httpClient.DeleteAsync("/companies");
            var company = new Company("SLB");
            var company1 = new Company("HappyDog");
            var companies = new List<Company>() { company, company1 };
            foreach (Company com in companies)
            {
                var companyJson = JsonConvert.SerializeObject(com);
                var requestBody = new StringContent(companyJson, Encoding.UTF8, "application/json");
                await httpClient.PostAsync("/companies", requestBody);
            }

            //when
            var reponse = await httpClient.GetAsync("/companies");
            var responseBody = await reponse.Content.ReadAsStringAsync();
            var allCompanies = JsonConvert.DeserializeObject<List<Company>>(responseBody);
            //then
            Assert.Equal(2, allCompanies.Count);
        }

        [Fact]
        public async Task Should_return_one_company_when_search_by_name()
        {
            //given
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            await httpClient.DeleteAsync("/companies");
            var company = new Company("SLB");
            var companyJson = JsonConvert.SerializeObject(company);
            var requestBody = new StringContent(companyJson, Encoding.UTF8, "application/json");
            var postResponse = await httpClient.PostAsync("/companies", requestBody);
            var postResponseBody = await postResponse.Content.ReadAsStringAsync();
            var createdCompany = JsonConvert.DeserializeObject<Company>(postResponseBody);

            //when
            var reponse = await httpClient.GetAsync($"/companies/{createdCompany.ID}");
            var responseBody = await reponse.Content.ReadAsStringAsync();
            var getCompany = JsonConvert.DeserializeObject<Company>(responseBody);
            //then
            Assert.Equal("SLB", getCompany.Name);
        }

        [Fact]
        public async Task Should_return_one_company_when_given_pageSize_and_pageIndex()
        {
            //given
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            await httpClient.DeleteAsync("/companies");
            var company = new Company("SLB");
            var company1 = new Company("HappyDog");
            var company2 = new Company("BadTom");
            var company3 = new Company("Sweety");
            var company4 = new Company("Cute");
            var company5 = new Company("Molly");
            var company6 = new Company("Crazy");
            var companies = new List<Company>() { company, company1, company2, company3, company4, company5, company6 };
            foreach (Company com in companies)
            {
                var companyJson = JsonConvert.SerializeObject(com);
                var requestBody = new StringContent(companyJson, Encoding.UTF8, "application/json");
                await httpClient.PostAsync("/companies", requestBody);
            }

            //when
            var reponse = await httpClient.GetAsync("/companies?pageSize=3&pageIndex=3");
            var responseBody = await reponse.Content.ReadAsStringAsync();
            var allCompaniesInPageIndex = JsonConvert.DeserializeObject<List<Company>>(responseBody);
            //then
            Assert.Single(allCompaniesInPageIndex);
        }

        [Fact]
        public async Task Should_return_modified_company_when_update_info()
        {
            //given
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            await httpClient.DeleteAsync("/companies");
            var company = new Company("SLB");
            var companyJson = JsonConvert.SerializeObject(company);
            var requestBody = new StringContent(companyJson, Encoding.UTF8, "application/json");
            var postResponse = await httpClient.PostAsync("/companies", requestBody);
            var postResponseBody = await postResponse.Content.ReadAsStringAsync();
            var createdCompany = JsonConvert.DeserializeObject<Company>(postResponseBody);

            //when
            createdCompany.Name = "AnotherName";
            var companyJson1 = JsonConvert.SerializeObject(createdCompany);
            var putRequestBody = new StringContent(companyJson1, Encoding.UTF8, "application/json");
            var reponse = await httpClient.PutAsync($"/companies/{createdCompany.ID}", putRequestBody);
            var responseBody1 = await reponse.Content.ReadAsStringAsync();
            var modifiedCompany = JsonConvert.DeserializeObject<Company>(responseBody1);
            //then
            Assert.Equal("AnotherName", modifiedCompany.Name);
        }

        [Fact]
        public async Task Should_add_employee_successfully()
        {
            //given
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            await httpClient.DeleteAsync("/companies");

            var company = new Company("SLB");
            var companyJson = JsonConvert.SerializeObject(company);
            var requestBody = new StringContent(companyJson, Encoding.UTF8, "application/json");
            var postResponse = await httpClient.PostAsync("/companies", requestBody);
            var postResponseBody = await postResponse.Content.ReadAsStringAsync();
            var createdCompany = JsonConvert.DeserializeObject<Company>(postResponseBody);

            //when
            Employee employee = new Employee("Liming", 5000);
            var employeeJson = JsonConvert.SerializeObject(employee);
            var postRequestBody = new StringContent(employeeJson, Encoding.UTF8, "application/json");
            var reponse = await httpClient.PostAsync($"/companies/{createdCompany.ID}/employees", postRequestBody);
            var responseBody1 = await reponse.Content.ReadAsStringAsync();
            var employeeAdded = JsonConvert.DeserializeObject<Employee>(responseBody1);
            //then
            Assert.Equal("Liming", employeeAdded.Name);
        }

        [Fact]
        public async Task Should_return_all_employees_successfully()
        {
            //given
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            await httpClient.DeleteAsync("/companies");

            var company = new Company("SLB");
            var companyJson = JsonConvert.SerializeObject(company);
            var requestBody = new StringContent(companyJson, Encoding.UTF8, "application/json");
            var postResponse = await httpClient.PostAsync("/companies", requestBody);
            var postResponseBody = await postResponse.Content.ReadAsStringAsync();
            var createdCompany = JsonConvert.DeserializeObject<Company>(postResponseBody);

            //when
            Employee employee = new Employee("Liming", 5000);
            Employee employee1 = new Employee("Zhaomin", 7000);
            List<Employee> employees = new List<Employee>() { employee, employee1 };
            foreach (Employee item in employees)
            {
                var employeeJson = JsonConvert.SerializeObject(item);
                var postRequestBody = new StringContent(employeeJson, Encoding.UTF8, "application/json");
                await httpClient.PostAsync($"/companies/{createdCompany.ID}/employees", postRequestBody);
            }

            var reponse = await httpClient.GetAsync($"/companies/{createdCompany.ID}/employees");
            var responseBody1 = await reponse.Content.ReadAsStringAsync();
            var allEmployee = JsonConvert.DeserializeObject<List<Employee>>(responseBody1);
            //then
            Assert.Equal(2, allEmployee.Count);
        }

        [Fact]
        public async Task Should_return_modified_employee_when_update_employee_info_successfully()
        {
            //given
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            await httpClient.DeleteAsync("/companies");

            var company = new Company("SLB");
            var companyJson = JsonConvert.SerializeObject(company);
            var requestBody = new StringContent(companyJson, Encoding.UTF8, "application/json");
            var postResponse = await httpClient.PostAsync("/companies", requestBody);
            var postResponseBody = await postResponse.Content.ReadAsStringAsync();
            var createdCompany = JsonConvert.DeserializeObject<Company>(postResponseBody);

            Employee employee = new Employee("Liming", 5000);
            var employeeJson = JsonConvert.SerializeObject(employee);
            var postRequestBody = new StringContent(employeeJson, Encoding.UTF8, "application/json");
            var postEmployeeResponse = await httpClient.PostAsync($"/companies/{createdCompany.ID}/employees", postRequestBody);
            var postEmployeeResponseBody = await postEmployeeResponse.Content.ReadAsStringAsync();
            var addedEmployee = JsonConvert.DeserializeObject<Employee>(postEmployeeResponseBody);

            //when
            addedEmployee.Salary = 9000;
            var employeeJson1 = JsonConvert.SerializeObject(addedEmployee);
            var postRequestBody1 = new StringContent(employeeJson1, Encoding.UTF8, "application/json");
            var reponse = await httpClient.PutAsync($"/companies/{createdCompany.ID}/employees/{addedEmployee.Id}", postRequestBody1);
            var responseBody1 = await reponse.Content.ReadAsStringAsync();
            var employeeModified = JsonConvert.DeserializeObject<Employee>(responseBody1);
            //then
            Assert.Equal(9000, employeeModified.Salary);
        }
    }
}
