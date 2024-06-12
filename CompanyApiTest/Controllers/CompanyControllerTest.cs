using CompanyApi.Model;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System.Collections.Generic;
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
        public async void Should_create_Company_Successfully()
        {
            //given
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            await httpClient.DeleteAsync("companies");
            var company = new Company(name: "SLB");
            var serializedObject = JsonConvert.SerializeObject(company);
            var postBody = new StringContent(serializedObject, Encoding.UTF8, "application/json");
            //when
            var response = await httpClient.PostAsync("companies", postBody);
            //then
            //response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var responseBody = await response.Content.ReadAsStringAsync();
            var createdcompany = JsonConvert.DeserializeObject<Company>(responseBody);
            Assert.Equal("SLB", createdcompany.Name);
            Assert.NotEmpty(createdcompany.ID);
        }

        [Fact]
        public async void Should_return_conflict_when_create_same_Companies()
        {
            //given
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            await httpClient.DeleteAsync("companies");
            var company = new Company(name: "SLB");
            var postBody = CreatePostBody(company);
            await httpClient.PostAsync("companies", postBody);
            var response = await httpClient.PostAsync("companies", postBody);
            //response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }

        [Fact]
        public async void Should_get_all_companies()
        {
            //given
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            await httpClient.DeleteAsync("companies");
            var company = new Company(name: "Google");
            var postBody = CreatePostBody(company);
            await httpClient.PostAsync("companies", postBody);
            //when
            var response = await httpClient.GetAsync("companies");
            //then
            //response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var responseBody = await response.Content.ReadAsStringAsync();
            var allcompanies = JsonConvert.DeserializeObject<List<Company>>(responseBody);
            Assert.Equal(company.Name, allcompanies[0].Name);
        }

        [Fact]
        public async void Should_get_existing_companies()
        {
            //given
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            await httpClient.DeleteAsync("companies");
            var company = new Company(name: "IBM");
            var createdcompany = await Createdcompany(company, httpClient);
            //when
            var newresponse = await httpClient.GetAsync($"companies/{createdcompany.ID}");
            var newresponseBody = await newresponse.Content.ReadAsStringAsync();
            var existingcompany = JsonConvert.DeserializeObject<Company>(newresponseBody);
            //then
            //response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, newresponse.StatusCode);
            Assert.Equal(createdcompany.ID, existingcompany.ID);
        }

        [Fact]
        public async void Should_get_X_companies_from_index_of_Y()
        {
            //given
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            await httpClient.DeleteAsync("companies");
            var companies = new List<Company>
            {
                new Company("IBM"),
                new Company("SLB"),
                new Company("Tesla"),
                new Company("Star"),
            };
            foreach (var company in companies)
            {
                var postBody = CreatePostBody(company);
                await httpClient.PostAsync("companies", postBody);
            }

            //when
            var response = await httpClient.GetAsync($"/companies?pageSize=2&pageIndex=2");
            var responseBody = await response.Content.ReadAsStringAsync();
            var allcompanies = JsonConvert.DeserializeObject<List<Company>>(responseBody);
            //then
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(2, allcompanies.Count);
            Assert.Equal("Tesla", allcompanies[0].Name);
        }

        [Fact]
        public async void Should_Update_Information_Of_An_Existing_Company()
        {
            //given
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            await httpClient.DeleteAsync("companies");
            var company = new Company(name: "IBM");
            var createdcompany = await Createdcompany(company, httpClient);
            //when
            company.Name = "IBMM";

            var postBodyModified = CreatePostBody(company);
            var responsemodified = await httpClient.PatchAsync($"companies/{createdcompany.ID}", postBodyModified);
            var responseBodymodified = await responsemodified.Content.ReadAsStringAsync();
            var modifiedcompany = JsonConvert.DeserializeObject<Company>(responseBodymodified);

            //then
            //response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, responsemodified.StatusCode);
            Assert.Equal("IBMM", modifiedcompany.Name);
        }

        [Fact]
        public async void Should_create_employee_in_an_existing_Company_Successfully()
        {
            //given
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            await httpClient.DeleteAsync("companies");
            var company = new Company(name: "SLB");
            var createdcompany = await Createdcompany(company, httpClient);
            var Cid = createdcompany.ID;
            Employee employee = new Employee("DENG", 1);
            //when
            var serializedEmployee = JsonConvert.SerializeObject(employee);
            var postBody = new StringContent(serializedEmployee, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync($"companies/{Cid}/employees", postBody);
            var responsebody = await response.Content.ReadAsStringAsync();
            var newemployee = JsonConvert.DeserializeObject<Employee>(responsebody);
            //then
            response.EnsureSuccessStatusCode();
            Assert.Equal(1, newemployee.EmployeeSalary);
        }

        [Fact]
        public async Task Should_get_all_employees()
        {
            //given
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            await httpClient.DeleteAsync("/companies");

            var company = new Company("SLB");
            var createdCompany = await Createdcompany(company, httpClient);

            //when
            var employees = new List<Employee>
            {
                new Employee("marry", 2),
                new Employee("lisa", 3),
            };
            foreach (var employee in employees)
            {
                var employeeObject = JsonConvert.SerializeObject(employee);
                var postBody = new StringContent(employeeObject, Encoding.UTF8, "application/json");
                await httpClient.PostAsync($"/companies/{createdCompany.ID}/employees", postBody);
            }

            var reponse = await httpClient.GetAsync($"/companies/{createdCompany.ID}/employees");
            var responseBody = await reponse.Content.ReadAsStringAsync();
            var allemployees = JsonConvert.DeserializeObject<List<Employee>>(responseBody);
            //then
            Assert.Equal(2, allemployees.Count);
        }

        [Fact]
        public async Task Should_get_modified_employee_when_update_employee_information()
        {
            //given
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            await httpClient.DeleteAsync("/companies");

            var company = new Company("SLB");
            var createdCompany = await Createdcompany(company, httpClient);

            Employee employee = new Employee("Julia", 10000);
            var employeeObject = JsonConvert.SerializeObject(employee);
            var postBody = new StringContent(employeeObject, Encoding.UTF8, "application/json");
            var postEmployeeResponse = await httpClient.PostAsync($"/companies/{createdCompany.ID}/employees", postBody);
            var employeeResponseBody = await postEmployeeResponse.Content.ReadAsStringAsync();
            var newEmployee = JsonConvert.DeserializeObject<Employee>(employeeResponseBody);

            //when
            newEmployee.EmployeeSalary = 8000;
            var employeeJson1 = JsonConvert.SerializeObject(newEmployee);
            var postRequestBody1 = new StringContent(employeeJson1, Encoding.UTF8, "application/json");
            var reponse = await httpClient.PutAsync($"/companies/{createdCompany.ID}/employees/{newEmployee.EmployeeID}", postRequestBody1);
            var responseBody = await reponse.Content.ReadAsStringAsync();
            var modifiedEmployee = JsonConvert.DeserializeObject<Employee>(responseBody);
            //then
            Assert.Equal(8000, modifiedEmployee.EmployeeSalary);
        }

        [Fact]
        public async Task Should_delete_one_employee_successfully()
        {
            //given
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            await httpClient.DeleteAsync("/companies");

            var company = new Company("SLB");
            var createdCompany = await Createdcompany(company, httpClient);

            var employees = new List<Employee>
            {
                new Employee("marry", 2),
                new Employee("lisa", 3),
            };
            var deleteEmployeeId = employees[0].EmployeeID;
            foreach (Employee employee in employees)
            {
                var employeeObject = JsonConvert.SerializeObject(employee);
                var postBody = new StringContent(employeeObject, Encoding.UTF8, "application/json");
                await httpClient.PostAsync($"/companies/{createdCompany.ID}/employees", postBody);
            }

            //when
             var reponse = await httpClient.DeleteAsync($"/companies/{createdCompany.ID}/employees/{deleteEmployeeId}");
             //then
            Assert.Equal(HttpStatusCode.OK, reponse.StatusCode);
        }

        [Fact]
        public async Task Should_delete_one_company_and_all_of_its_employees()
        {
            //given
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            await httpClient.DeleteAsync("/companies");

            var company = new Company("SLB");
            var createdCompany = await Createdcompany(company, httpClient);

            var employees = new List<Employee>
            {
                new Employee("marry", 2),
                new Employee("lisa", 3),
            };
            foreach (Employee employee in employees)
            {
                var employeeObject = JsonConvert.SerializeObject(employee);
                var postBody = new StringContent(employeeObject, Encoding.UTF8, "application/json");
                await httpClient.PostAsync($"/companies/{createdCompany.ID}/employees", postBody);
            }

            //when
            var reponse = await httpClient.DeleteAsync($"/companies/{createdCompany.ID}");
            //then
            Assert.Equal(HttpStatusCode.OK, reponse.StatusCode);
        }

        public static async Task<Company> Createdcompany(Company company, HttpClient httpClient)
        {
            var postBody = CreatePostBody(company);
            var response = await httpClient.PostAsync("companies", postBody);
            var responseBody = await response.Content.ReadAsStringAsync();
            var createdcompany = JsonConvert.DeserializeObject<Company>(responseBody);
            return createdcompany;
        }

        public static StringContent CreatePostBody(Company company)
        {
            var serializedObject = JsonConvert.SerializeObject(company);
            var postBody = new StringContent(serializedObject, Encoding.UTF8, "application/json");
            return postBody;
        }
    }
}
