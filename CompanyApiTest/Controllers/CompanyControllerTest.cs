using CompanyApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
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
            HttpClient httpClient = await CreateHttpClient();

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

        [Fact]
        public async Task Should_get_page_size_companies_from_page_index_Async()
        {
            //given
            HttpClient httpClient = await CreateHttpClient();
            await PostCompany(httpClient, new Company("SLB"));
            await PostCompany(httpClient, new Company("AAA"));
            await PostCompany(httpClient, new Company("BBB"));
            await PostCompany(httpClient, new Company("CCC"));
            await PostCompany(httpClient, new Company("DDD"));
            await PostCompany(httpClient, new Company("EEE"));
            await PostCompany(httpClient, new Company("FFF"));
            await PostCompany(httpClient, new Company("GGG"));

            var pageSize = 3;
            var pageIndex = 2;

            //when
            var response = await httpClient.GetAsync($"/companies?pageSize={pageSize}&pageIndex={pageIndex}");

            //then
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var responseBody = await response.Content.ReadAsStringAsync();
            var companiesGot = JsonConvert.DeserializeObject<List<Company>>(responseBody);
            Assert.Equal(pageSize, companiesGot.Count);
            Assert.Equal("CCC", companiesGot[0].Name);
        }

        [Fact]
        public async Task Should_update_basic_information_of_an_existing_company()
        {
            //given
            HttpClient httpClient = await CreateHttpClient();
            Company company = new Company("Schlumberger");
            await PostCompany(httpClient, company);

            var responseOld = await httpClient.GetAsync("/companies");
            var responseBodyOld = await responseOld.Content.ReadAsStringAsync();
            var companyToUpdate = JsonConvert.DeserializeObject<List<Company>>(responseBodyOld)[0];

            company.Name = "SLB";
            var companyJson = JsonConvert.SerializeObject(company);
            var putBody = new StringContent(companyJson, Encoding.UTF8, "application/json");

            //when
            var response = await httpClient.PutAsync($"/companies/{companyToUpdate.CompanyID}", putBody);

            //then
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var responseBody = await response.Content.ReadAsStringAsync();
            var companyUpdated = JsonConvert.DeserializeObject<Company>(responseBody);
            Assert.Equal("SLB", companyUpdated.Name);
        }

        [Fact]
        public async Task Should_add_an_employee_to_a_specific_company_Async()
        {
            //given
            HttpClient httpClient = await CreateHttpClient();
            await PostCompany(httpClient, new Company("SLB"));

            var getCompanyResponse = await httpClient.GetAsync("/companies");
            var getCompanyBody = await getCompanyResponse.Content.ReadAsStringAsync();
            var companyToAdd = JsonConvert.DeserializeObject<List<Company>>(getCompanyBody)[0];

            var employeeJson = JsonConvert.SerializeObject(new Employee("Lucy", 5000));
            var postBody = new StringContent(employeeJson, Encoding.UTF8, "application/json");

            //when
            var response = await httpClient.PostAsync($"/companies/{companyToAdd.CompanyID}/employees", postBody);

            //then
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var responseBody = await response.Content.ReadAsStringAsync();
            var employee = JsonConvert.DeserializeObject<Employee>(responseBody);
            Assert.Equal("Lucy", employee.Name);
        }

        [Fact]
        public async Task Should_get_all_employees_under_company()
        {
            //given
            HttpClient httpClient = await CreateHttpClient();
            await PostCompany(httpClient, new Company("SLB"));

            var getCompanyResponse = await httpClient.GetAsync("/companies");
            var getCompanyBody = await getCompanyResponse.Content.ReadAsStringAsync();
            var companyToGet = JsonConvert.DeserializeObject<List<Company>>(getCompanyBody)[0];

            await PostEmployeeToACompany(httpClient, new Employee("Lucy", 5000), companyToGet);
            await PostEmployeeToACompany(httpClient, new Employee("Jerry", 7000), companyToGet);

            //when
            var response = await httpClient.GetAsync($"/companies/{companyToGet.CompanyID}/employees");

            //then
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var responseBody = await response.Content.ReadAsStringAsync();
            var employees = JsonConvert.DeserializeObject<List<Employee>>(responseBody);
            Assert.Equal(2, employees.Count);
        }

        [Fact]
        public async Task Should_update_basic_information_of_a_specific_employee_under_a_specific_company()
        {
            //given
            HttpClient httpClient = await CreateHttpClient();
            await PostCompany(httpClient, new Company("SLB"));

            var companyResponse = await httpClient.GetAsync("/companies");
            var companyBody = await companyResponse.Content.ReadAsStringAsync();
            var companyToGet = JsonConvert.DeserializeObject<List<Company>>(companyBody)[0];

            await PostEmployeeToACompany(httpClient, new Employee("Lucy", 5000), companyToGet);

            var employeeResponse = await httpClient.GetAsync($"/companies/{companyToGet.CompanyID}/employees");
            var employeeBody = await employeeResponse.Content.ReadAsStringAsync();
            var employeeToGet = JsonConvert.DeserializeObject<List<Employee>>(employeeBody)[0];

            employeeToGet.Name = "Lulu";
            var employeeJson = JsonConvert.SerializeObject(employeeToGet);
            var postBody = new StringContent(employeeJson, Encoding.UTF8, "application/json");

            //when
            var response = await httpClient.PutAsync($"/companies/{companyToGet.CompanyID}/employees/{employeeToGet.EmployeeID}", postBody);

            //then
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var responseBody = await response.Content.ReadAsStringAsync();
            var employee = JsonConvert.DeserializeObject<Employee>(responseBody);
            Assert.Equal("Lulu", employee.Name);
        }

        private static async Task PostEmployeeToACompany(HttpClient httpClient, Employee employee, Company companyToPost)
        {
            var employeeJson = JsonConvert.SerializeObject(employee);
            var postBody = new StringContent(employeeJson, Encoding.UTF8, "application/json");
            await httpClient.PostAsync($"/companies/{companyToPost.CompanyID}/employees", postBody);
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
