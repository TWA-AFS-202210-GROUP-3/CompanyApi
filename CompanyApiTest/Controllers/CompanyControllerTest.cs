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
            var companyJson = JsonConvert.SerializeObject(company);
            var postbody = new StringContent(companyJson, Encoding.UTF8, "application/json");
            // when
            var response = await httpClient.PostAsync("/companies", postbody);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var responseString = await response.Content.ReadAsStringAsync();

            // then
            var createdcompany = JsonConvert.DeserializeObject<Company>(responseString);
            response.EnsureSuccessStatusCode();
            Assert.Equal("SLB", createdcompany.Name);
            Assert.NotEmpty(createdcompany.Id);
        }

        [Fact]
        public async Task Should_confilict_when_add_same_name()
        {
            // given
            var httpClient = await InitHttpClient();

            var company = new Company(name: "SLB");
            var companyJson = JsonConvert.SerializeObject(company);
            var postbody = new StringContent(companyJson, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync("/companies", postbody);

            var company1 = new Company(name: "SLB");
            var companyJson1 = JsonConvert.SerializeObject(company1);
            var postbody1 = new StringContent(companyJson1, Encoding.UTF8, "application/json");
            var response1 = await httpClient.PostAsync("/companies", postbody1);
            // when
            Assert.Equal(HttpStatusCode.Conflict, response1.StatusCode);
        }

        [Fact]
        public async Task Should_get_all_company_successfully()
        {
            // given
            var httpClient = await InitHttpClient();

            var company = new Company(name: "SLB");
            var companyJson = JsonConvert.SerializeObject(company);
            var postbody = new StringContent(companyJson, Encoding.UTF8, "application/json");
            await httpClient.PostAsync("/companies", postbody);

            // when
            var response = await httpClient.GetAsync("/companies");
            var responseString = await response.Content.ReadAsStringAsync();

            // then
            var createdcompany = JsonConvert.DeserializeObject<List<Company>>(responseString);
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("SLB", createdcompany[0].Name);
        }

        [Fact]
        public async Task Should_get_company_by_id_successfully()
        {
            // given
            var httpClient = await InitHttpClient();

            var companySLB = new Company(name: "SLB");
            var companySLBB = new Company(name: "SLBB");
            var createdcompany = await CreatCompany(companySLB, httpClient);
            await AddCompany(companySLBB, httpClient);

            // when
            var response = await httpClient.GetAsync("/companies/" + createdcompany.Id);
            var responseString = await response.Content.ReadAsStringAsync();
            var companybyID = JsonConvert.DeserializeObject<Company>(responseString);
            // then
            response.EnsureSuccessStatusCode();
            Assert.Equal("SLB", companybyID.Name);
        }

        [Fact]
        public async Task Should_get_company_by_pagesize_and_pageindex_successfully()
        {
            // given
            var httpClient = await InitHttpClient();

            var companySLB = new Company(name: "SLB");
            var companySLBB = new Company(name: "SLBB");
            var companySLCC = new Company(name: "SLCC");
            await CreatCompany(companySLB, httpClient);
            await CreatCompany(companySLBB, httpClient);
            await CreatCompany(companySLCC, httpClient);

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

            var companySLB = new Company(name: "SLB");
            var company = await CreatCompany(companySLB, httpClient);
            var response = await httpClient.GetAsync("/companies/" + company.Id);

            var responseString = await response.Content.ReadAsStringAsync();
            var companySLBbyID = JsonConvert.DeserializeObject<Company>(responseString);

            companySLB.Name = "Test";
            var serializeObjectmodify = JsonConvert.SerializeObject(companySLB);
            var postBodymodify = new StringContent(serializeObjectmodify, Encoding.UTF8, "application/json");

            // when
            var responsemodify = await httpClient.PatchAsync("/companies/" + companySLBbyID.Id, postBodymodify);
            var responseStringmodify = await responsemodify.Content.ReadAsStringAsync();
            var companies = JsonConvert.DeserializeObject<Company>(responseStringmodify);
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

            var companySLB = new Company(name: "SLB");
            var company = await CreatCompany(companySLB, httpClient);
            var response = await httpClient.GetAsync("/companies/" + company.Id);

            var responseString = await response.Content.ReadAsStringAsync();
            var companySLBbyID = JsonConvert.DeserializeObject<Company>(responseString);

            var employees = new List<Employee>
            {
                new Employee(name: "Hans", salary: 50000),
            };

            company.Employees = employees;
            var serializeObjectmodify = JsonConvert.SerializeObject(company);
            var postBodymodify = new StringContent(serializeObjectmodify, Encoding.UTF8, "application/json");

            // when
            var responsemodify = await httpClient.PostAsync("/companies/" + companySLBbyID.Id + "/employees", postBodymodify);
            var responseStringmodify = await responsemodify.Content.ReadAsStringAsync();
            var companies = JsonConvert.DeserializeObject<Company>(responseStringmodify);
            // then
            response.EnsureSuccessStatusCode();
            Assert.Equal("Hans", companies.Employees[0].Name);
        }

        [Fact]
        public async Task Should_get_all_employee_successfully()
        {
            // given
            var httpClient = await InitHttpClient();

            var companySLB = new Company(name: "SLB");
            var company = await CreatCompany(companySLB, httpClient);
            var response = await httpClient.GetAsync("/companies/" + company.Id);

            var responseString = await response.Content.ReadAsStringAsync();
            var companySLBbyID = JsonConvert.DeserializeObject<Company>(responseString);

            var employees = new List<Employee>
            {
                new Employee(name: "Hans", salary: 50000),
                new Employee(name: "Jason", salary: 50000),
            };

            company.Employees = employees;
            var serializeObjectmodify = JsonConvert.SerializeObject(company);
            var postBodymodify = new StringContent(serializeObjectmodify, Encoding.UTF8, "application/json");
            await httpClient.PostAsync("/companies/" + companySLBbyID.Id + "/employees", postBodymodify);

            // when
            var responsemodify = await httpClient.GetAsync("/companies/" + companySLBbyID.Id + "/employees");
            var responseStringmodify = await responsemodify.Content.ReadAsStringAsync();
            var getemployees = JsonConvert.DeserializeObject<List<Employee>>(responseStringmodify);
            // then
            response.EnsureSuccessStatusCode();
            Assert.Equal(2, getemployees.Count);
        }

        [Fact]
        public async Task Should_modify_employee_name_successfully()
        {
            // given
            var httpClient = await InitHttpClient();

            var companySLB = new Company(name: "SLB");
            var company = await CreatCompany(companySLB, httpClient);
            var response = await httpClient.GetAsync("/companies/" + company.Id);

            var responseString = await response.Content.ReadAsStringAsync();
            var companySLBbyID = JsonConvert.DeserializeObject<Company>(responseString);

            var employees = new List<Employee>
            {
                new Employee(name: "Hans", salary: 50000),
            };

            company.Employees = employees;
            var serializeObjectmodify = JsonConvert.SerializeObject(company);
            var postBodymodify = new StringContent(serializeObjectmodify, Encoding.UTF8, "application/json");
            await httpClient.PostAsync("/companies/" + companySLBbyID.Id + "/employees", postBodymodify);

            company.Employees[0].Name = "Jason";
            string employeeID = company.Employees[0].Id;
            var serializeObjectemployeemodify = JsonConvert.SerializeObject(company);
            var employeemodify = new StringContent(serializeObjectemployeemodify, Encoding.UTF8, "application/json");
            // when
            var responsemodify = await httpClient.PostAsync("/companies/" + companySLBbyID.Id + "/employees/" + employeeID, employeemodify);
            var responseStringmodify = await responsemodify.Content.ReadAsStringAsync();
            var companymodify = JsonConvert.DeserializeObject<Company>(responseStringmodify);
            Employee employee = companymodify.Employees[0];
            // then
            response.EnsureSuccessStatusCode();
            Assert.Equal("Jason", employee.Name);
        }

        [Fact]
        public async Task Should_delete_employee_by_name_successfully()
        {
            // given
            var httpClient = await InitHttpClient();

            var companySLB = new Company(name: "SLB");
            var company = await CreatCompany(companySLB, httpClient);
            var response = await httpClient.GetAsync("/companies/" + company.Id);

            var responseString = await response.Content.ReadAsStringAsync();
            var companySLBbyID = JsonConvert.DeserializeObject<Company>(responseString);

            var employees = new List<Employee>
            {
                new Employee(name: "Hans", salary: 50000),
                new Employee(name: "Jason", salary: 50000),
            };

            company.Employees = employees;
            var serializeObjectmodify = JsonConvert.SerializeObject(company);
            var postBodymodify = new StringContent(serializeObjectmodify, Encoding.UTF8, "application/json");
            await httpClient.PostAsync("/companies/" + companySLBbyID.Id + "/employees", postBodymodify);

            string employeeID = company.Employees[0].Id;
            // when
            var responsemodify = await httpClient.DeleteAsync("/companies/" + companySLBbyID.Id + "/employees/" + employeeID);
            var responseStringmodify = await responsemodify.Content.ReadAsStringAsync();
            var companymodify = JsonConvert.DeserializeObject<Company>(responseStringmodify);

            // then
            response.EnsureSuccessStatusCode();
            Assert.Equal(1, companymodify.Employees.Count);
        }

        // Public Method
        private static async Task<HttpClient> InitHttpClient()
        {
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            await httpClient.DeleteAsync("/companies");
            return httpClient;
        }

        private static async Task<Company> CreatCompany(Company company, HttpClient httpClient)
        {
            var postResponse = await AddCompany(company, httpClient);
            var postResponseString = await postResponse.Content.ReadAsStringAsync();
            var createdcompany = JsonConvert.DeserializeObject<Company>(postResponseString);
            return createdcompany;
        }

        private static async Task<HttpResponseMessage> AddCompany(Company company, HttpClient httpClient)
        {
            var companyJson = JsonConvert.SerializeObject(company);
            var postbody = new StringContent(companyJson, Encoding.UTF8, "application/json");
            return await httpClient.PostAsync("/companies", postbody);
        }

        //private static async Task<List<Company>> CreatEmployee(List<Employee> employees, HttpClient httpClient)
        //{
        //    var postResponse = await AddEmployee(employees, httpClient);
        //    var postResponseString = await postResponse.Content.ReadAsStringAsync();
        //    var createdemployees = JsonConvert.DeserializeObject<List<Company>>(postResponseString);
        //    return createdemployees;
        //}

        //private static async Task<HttpResponseMessage> AddCompany(List<Employee> employees, HttpClient httpClient)
        //{
        //    var companyJson = JsonConvert.SerializeObject(company);
        //    var postbody = new StringContent(companyJson, Encoding.UTF8, "application/json");
        //    return await httpClient.PostAsync("/companies", postbody);
        //}
    }
}
