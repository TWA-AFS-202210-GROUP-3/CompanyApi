using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CompanyApi.Controllers;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using Xunit;

namespace CompanyApiTest.Controllers
{
    public class CompanyControllerTest
    {
        [Fact]
        public async Task Should_add_company_successfully()
        {
            // given
            var httpClient = await InitHttpClient();

            var company = new Company(name: "SLB");
            var response = await HttpResponseMessage(company, httpClient);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var responseString = await response.Content.ReadAsStringAsync();
            var createdCompany = JsonConvert.DeserializeObject<Company>(responseString);
            response.EnsureSuccessStatusCode();
            Assert.Equal("SLB", createdCompany.Name);
            Assert.NotEmpty(createdCompany.Id);
        }

        [Fact]
        public async Task Should_confilict_when_add_same_name()
        {
            // given
            var httpClient = await InitHttpClient();

            var company = new Company(name: "SLB");
            await CreateCompany(company, httpClient);

            var company1 = new Company(name: "SLB");
            var response1 = await HttpResponseMessage(company1, httpClient);
            //then
            Assert.Equal(HttpStatusCode.Conflict, response1.StatusCode);
        }

        [Fact]
        public async Task Should_get_all_company_successfully()
        {
            // given
            var httpClient = await InitHttpClient();
            var company = new Company(name: "SLB");
            await AddCompany(company, httpClient);

            // when
            var response = await httpClient.GetAsync("/companies");
            var responseString = await response.Content.ReadAsStringAsync();

            // then
            var createdCompany = JsonConvert.DeserializeObject<List<Company>>(responseString);
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("SLB", createdCompany[0].Name);
        }

        [Fact]
        public async Task Should_get_company_by_id_successfully()
        {
            // given
            var httpClient = await InitHttpClient();

            var companySLB = new Company(name: "SLB");
            var companySLBB = new Company(name: "SLBB");
            var createdCompany = await CreateCompany(companySLB, httpClient);
            await AddCompany(companySLBB, httpClient);

            // when
            var response = await httpClient.GetAsync("/companies/" + createdCompany.Id);
            var responseString = await response.Content.ReadAsStringAsync();
            var companyById = JsonConvert.DeserializeObject<Company>(responseString);
            // then
            response.EnsureSuccessStatusCode();
            Assert.Equal("SLB", companyById.Name);
        }

        [Fact]
        public async Task Should_get_company_by_pagesize_and_pageindex_successfully()
        {
            // given
            var httpClient = await InitHttpClient();

            var companySLB = new Company(name: "SLB");
            var companySLBB = new Company(name: "SLBB");
            var companySLCC = new Company(name: "SLCC");
            await CreateCompany(companySLB, httpClient);
            await CreateCompany(companySLBB, httpClient);
            await CreateCompany(companySLCC, httpClient);

            // when
            var response = await httpClient.GetAsync("/companies?pageSize=2&pageIndex=2");
            var responseString = await response.Content.ReadAsStringAsync();
            var companies = JsonConvert.DeserializeObject<List<Company>>(responseString);
            // then
            response.EnsureSuccessStatusCode();
            Assert.Equal("SLCC", companies[0].Name);
            Assert.Equal(1, companies.Count);
        }

        [Fact]
        public async Task Should_modify_company_name_successfully()
        {
            // given
            var httpClient = await InitHttpClient();

            var companySlb = new Company(name: "SLB");
            var company = await CreateCompany(companySlb, httpClient);
            var response = await httpClient.GetAsync("/companies/" + company.Id);

            var responseString = await response.Content.ReadAsStringAsync();
            var companySlBbyId = JsonConvert.DeserializeObject<Company>(responseString);

            companySlb.Name = "Test";
            var serializeObjectModify = JsonConvert.SerializeObject(companySlb);
            var postBodyModify = new StringContent(serializeObjectModify, Encoding.UTF8, "application/json");

            // when
            var responseModify = await httpClient.PatchAsync("/companies/" + companySlBbyId.Id, postBodyModify);
            var responseStringModify = await responseModify.Content.ReadAsStringAsync();
            var companies = JsonConvert.DeserializeObject<Company>(responseStringModify);
            // then
            response.EnsureSuccessStatusCode();
            //Assert.Equal(HttpStatusCode.OK, responsemodify.StatusCode);
            Assert.Equal("Test", companies.Name);
        }

        [Fact]
        public async Task Should_add_employee_successfully()
        {
            // given
            var httpClient = await InitHttpClient();

            var companySlb = new Company(name: "SLB");
            var company = await CreateCompany(companySlb, httpClient);
            var response = await httpClient.GetAsync("/companies/" + company.Id);

            var responseString = await response.Content.ReadAsStringAsync();
            var companySlBbyId = JsonConvert.DeserializeObject<Company>(responseString);

            var employees = new List<Employee>
            {
                new Employee(name: "Hans", salary: 50000),
            };
            var postBodyModify = PostNewEmployee(company, employees);

            // when
            var responseModify = await httpClient.PostAsync("/companies/" + companySlBbyId.Id + "/employees", postBodyModify);
            var responseStringModify = await responseModify.Content.ReadAsStringAsync();
            var companies = JsonConvert.DeserializeObject<Company>(responseStringModify);
            // then
            response.EnsureSuccessStatusCode();
            Assert.Equal("Hans", companies.Employees[0].Name);
        }

        [Fact]
        public async Task Should_get_all_employee_successfully()
        {
            // given
            var httpClient = await InitHttpClient();

            var companySlb = new Company(name: "SLB");
            var company = await CreateCompany(companySlb, httpClient);
            var response = await httpClient.GetAsync("/companies/" + company.Id);

            var responseString = await response.Content.ReadAsStringAsync();
            var companySlbById = JsonConvert.DeserializeObject<Company>(responseString);

            var employees = new List<Employee>
            {
                new Employee(name: "Hans", salary: 50000),
                new Employee(name: "Jason", salary: 50000),
            };
            await AddEmployee(company, employees, httpClient, companySlbById);

            // when
            var responseModify = await httpClient.GetAsync("/companies/" + companySlbById.Id + "/employees");
            var responseStringModify = await responseModify.Content.ReadAsStringAsync();
            var getEmployees = JsonConvert.DeserializeObject<List<Employee>>(responseStringModify);
            // then
            response.EnsureSuccessStatusCode();
            Assert.Equal(2, getEmployees.Count);
        }

        [Fact]
        public async Task Should_modify_employee_name_successfully()
        {
            // given
            var httpClient = await InitHttpClient();

            var companySlb = new Company(name: "SLB");
            var company = await CreateCompany(companySlb, httpClient);
            var response = await httpClient.GetAsync("/companies/" + company.Id);

            var responseString = await response.Content.ReadAsStringAsync();
            var companySlBbyId = JsonConvert.DeserializeObject<Company>(responseString);

            var employees = new List<Employee>
            {
                new Employee(name: "Hans", salary: 50000),
            };
            await AddEmployee(company, employees, httpClient, companySlBbyId);

            company.Employees[0].Name = "Jason";
            string employeeId = company.Employees[0].Id;
            var serializeObjectEmployeeModify = JsonConvert.SerializeObject(company);
            var employeeModify = new StringContent(serializeObjectEmployeeModify, Encoding.UTF8, "application/json");
            // when
            var responseModify = await httpClient.PostAsync("/companies/" + companySlBbyId.Id + "/employees/" + employeeId, employeeModify);
            var responseStringModify = await responseModify.Content.ReadAsStringAsync();
            var companyModify = JsonConvert.DeserializeObject<Company>(responseStringModify);
            Employee employee = companyModify.Employees[0];
            // then
            response.EnsureSuccessStatusCode();
            Assert.Equal("Jason", employee.Name);
        }

        [Fact]
        public async Task Should_delete_employee_by_name_successfully()
        {
            // given
            var httpClient = await InitHttpClient();

            var companySlb = new Company(name: "SLB");
            var company = await CreateCompany(companySlb, httpClient);
            var response = await httpClient.GetAsync("/companies/" + company.Id);

            var responseString = await response.Content.ReadAsStringAsync();
            var companySlBbyId = JsonConvert.DeserializeObject<Company>(responseString);

            var employees = new List<Employee>
            {
                new Employee(name: "Hans", salary: 50000),
                new Employee(name: "Jason", salary: 50000),
            };
            await AddEmployee(company, employees, httpClient, companySlBbyId);

            // when
            var responseModify = await httpClient.DeleteAsync("/companies/" + companySlBbyId.Id + "/employees/" + company.Employees[0].Id);
            var responseStringModify = await responseModify.Content.ReadAsStringAsync();
            var companyModify = JsonConvert.DeserializeObject<Company>(responseStringModify);

            // then
            response.EnsureSuccessStatusCode();
            Assert.Equal(1, companyModify.Employees.Count);
        }

        // Public Method
        private static async Task<HttpClient> InitHttpClient()
        {
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            await httpClient.DeleteAsync("/companies");
            return httpClient;
        }

        private static async Task<Company> CreateCompany(Company company, HttpClient httpClient)
        {
            var postResponse = await AddCompany(company, httpClient);
            var postResponseString = await postResponse.Content.ReadAsStringAsync();
            var createdCompany = JsonConvert.DeserializeObject<Company>(postResponseString);
            return createdCompany;
        }

        private static async Task<HttpResponseMessage> AddCompany(Company company, HttpClient httpClient)
        {
            var companyJson = JsonConvert.SerializeObject(company);
            var postBody = new StringContent(companyJson, Encoding.UTF8, "application/json");
            return await httpClient.PostAsync("/companies", postBody);
        }

        private static StringContent PostNewEmployee(Company company, List<Employee> employees)
        {
            company.Employees = employees;
            var serializeObjectModify = JsonConvert.SerializeObject(company);
            var postBodyModify = new StringContent(serializeObjectModify, Encoding.UTF8, "application/json");
            return postBodyModify;
        }

        private static async Task AddEmployee(Company company, List<Employee> employees, HttpClient httpClient, Company? companyById)
        {
            company.Employees = employees;
            var serializeObjectModify = JsonConvert.SerializeObject(company);
            var postBodyModify = new StringContent(serializeObjectModify, Encoding.UTF8, "application/json");
            await httpClient.PostAsync("/companies/" + companyById.Id + "/employees", postBodyModify);
        }

        private static async Task<HttpResponseMessage> HttpResponseMessage(Company company, HttpClient httpClient)
        {
            var companyJson = JsonConvert.SerializeObject(company);
            var postBody = new StringContent(companyJson, Encoding.UTF8, "application/json");
            // when
            var response = await httpClient.PostAsync("/companies", postBody);
            return response;
        }
    }
}
