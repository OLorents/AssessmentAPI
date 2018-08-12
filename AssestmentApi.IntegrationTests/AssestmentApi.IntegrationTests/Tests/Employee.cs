using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using NUnit.Framework;
using RestSharp;

namespace AssestmentApi.IntegrationTests.Tests
{
   public class Employee : TestBase
    {
        private const string EmployeesBaseUrl = "https://mobilewebserver9-pokertest8ext.installprogram.eu/TestApi/api/automation/employees";
        private RestClient _client;

        [SetUp]
        public override void TestSetUp()
        {
            base.TestSetUp();
            _client = new RestClient(EmployeesBaseUrl);

            var response = GetAll(_client);
            var restMessages = ParseResponse(response);
            foreach (var restMessage in restMessages)
            {
                DeleteById(_client, restMessage.Id);
            }
        }

        /// <summary>
        /// This test checks that 'GET /employees' request returns empty data when no employees exist
        /// </summary>
        [Test]
        public void GetAllEmployees_EmptyData()
        {
            //GET All employees
            var response = GetAll(_client);

            Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode, "Status Code is not 'OK'");
            Assert.AreEqual(200, (int)response.StatusCode, "Status Code is not '200'");
            Assert.AreEqual("[]", response.Content);
        }

        /// <summary>
        /// This test checks that 'GET /employees' request returns all employees
        /// </summary>
        [Test]
        public void GetAllEmployees_TwoEmployeesExist()
        {
            //Create two employees
            var expectedEmployees = new List<string> { "TestEmployee1", "TestEmployee2" };
            foreach (var employee in expectedEmployees)
            {
                Create(_client, employee);
            }

            //GET All employees
            var response = GetAll(_client);

            Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode, "Status Code is not 'OK'");
            Assert.AreEqual(200, (int)response.StatusCode, "Status Code is not '200'");

            var restMessages = ParseResponse(response);
            Assert.AreEqual(2, restMessages.Count, "Two employees must be in the list");

            var actualEmployees = new List<string>();
            var actualIds = new List<int?>();
            foreach (var message in restMessages)
            {
                actualIds.Add(message.Id);
                actualEmployees.Add(message.Name);
            }
            Assert.IsTrue(expectedEmployees.All(actualEmployees.Contains), "Actual employees must be same as expected");
            Assert.AreEqual(2, actualIds.Count, "'Id' should be present in the Response Body for each employee ");
        }

        /// <summary>
        /// This test checks that 'GET /employees/id/{employeesId}' request returns employees by id
        /// </summary>
        [Test]
        public void GetEmployeeById()
        {
            //Create three employees
            var expectedEmployees = new List<string> { "TestEmployee1", "TestEmployee2", "TestEmployee3" };
            foreach (var employee in expectedEmployees)
            {
                Create(_client, employee);
            }

            var response = GetAll(_client);
            Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode, "Status Code is not 'OK'");
            Assert.AreEqual(200, (int)response.StatusCode, "Status Code is not '200'");

            //Check Response Body
            var allEmployees = ParseResponse(response);
            foreach (var message in allEmployees)
            {
                var getByIdResponse = GetById(_client, message.Id);
                var employeeById = JsonConvert.DeserializeObject<ResponseBody>(getByIdResponse.Content);
                Assert.IsTrue(expectedEmployees.Contains(employeeById.Name), "employees name is not expected");
                Assert.IsTrue(employeeById.Id == null, "'Id' should not be present in the Response Body");
            }
        }

        /// <summary>
        /// This test checks that 'GET /employees/id/{employeeId}'request for unknow resource returns 404 NotFound status code
        /// </summary>
        [Test]
        public void GetCompanyByInvalidId()
        {
            var response = GetById(_client, 1000);
            Assert.AreEqual(System.Net.HttpStatusCode.NotFound, response.StatusCode, "Status Code is not 'NotFound'");
            Assert.AreEqual(404, (int)response.StatusCode, "Status Code is not '404'");
        }

        /// <summary>
        /// This test checks that 'POST /employees' request creates employees 
        /// </summary>
        [Test]
        public void CreateEmployee()
        {
            //Create two employees
            var expectedEmployees = new List<string> { "TestEmployee1", "TestEmployee2" };
            foreach (var employee in expectedEmployees)
            {
                Create(_client, employee);
            }

            //make a GET request and ensure the data was saved correctly
            var response = GetAll(_client);

            Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode, "Status Code is not 'OK'");
            Assert.AreEqual(200, (int)response.StatusCode, "Status Code is not '200'");

            //Check Response Body
            var restMessages = ParseResponse(response);
            Assert.AreEqual(2, restMessages.Count, "Two employees must be in the list");

            var actualEmployees = new List<string>();
            var actualIds = new List<int?>();
            foreach (var message in restMessages)
            {
                actualIds.Add(message.Id);
                actualEmployees.Add(message.Name);
            }
            Assert.IsTrue(expectedEmployees.All(actualEmployees.Contains), "Created employees are different from expected");
            Assert.AreEqual(2, actualIds.Count, "'Id' shoud be present in the Response Body for each created employee");
        }

        /// <summary>
        /// This test checks that 'POST /employees' request fails with incorrect data 
        /// </summary>
        [Test]
        public void CreateEmployeeWithIncorrectData()
        {
            RestRequest request = new RestRequest(Method.POST);
            request.AddHeader(Authorization, Bearer + Token);
            request.RequestFormat = DataFormat.Json;
            //Set 'Id' instead od 'Name' in the RequestBody
            request.AddBody(new { Id = "TestEmployee" });
            var response = _client.Execute(request);

            Assert.AreEqual(System.Net.HttpStatusCode.BadRequest, response.StatusCode, "Status Code is not 'BadRequest'");
            Assert.AreEqual(400, (int)response.StatusCode, "Status Code is not '400'");
        }

        /// <summary>
        /// This test checks that 'DELETE /employees/id/{employeeId}' request delets employee by Id 
        /// </summary>
        [Test]
        public void DeleteEmployees()
        {
            //Create employee and check that it is created
            Create(_client, "TestEmployee");
            var employee = GetAll(_client);
            var restMessages = ParseResponse(employee);
            Assert.AreEqual(1, restMessages.Count, "One employee must be in the list");
            //Delete employee
            var response = DeleteById(_client, restMessages[0].Id);
            Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode, "Status Code is not 'OK'");
            Assert.AreEqual(200, (int)response.StatusCode, "Status Code is not '200'");
            //Check that Employees is deleted, empty data should be returned
            employee = GetAll(_client);
            restMessages = ParseResponse(employee);
            Assert.AreEqual(0, restMessages.Count, "Zero employees must be in the list");
        }

        /// <summary>
        /// This test checks that 'DELETE /employees/id/{employeeId}' request for unknow resource returns 404 NotFound status code
        /// </summary>
        [Test]
        public void DeleteUnknownResource()
        {
            //Delete Employee with Id = 1000, such Id is not in the system
            var response = DeleteById(_client, 1000);
            Assert.AreEqual(System.Net.HttpStatusCode.NotFound, response.StatusCode, "Status Code is not 'NotFound'");
            Assert.AreEqual(404, (int)response.StatusCode, "Status Code is not '200'");
        }

        //==========================InvalidToken========================================================

        /// <summary>
        /// This test checks that 'GET /employees' request with invalid token returns 401 Unauthorized status code
        /// </summary>
        [Test]
        public void GetAllEmployeesWithInvalidToken()
        {
            var request = new RestRequest(Method.GET);
            request.AddHeader(Authorization, Bearer + InvalidToken);
            var response = _client.Execute(request);

            Assert.AreEqual(System.Net.HttpStatusCode.Unauthorized, response.StatusCode, "Status Code is not 'Unauthorized'");
            Assert.AreEqual(401, (int)response.StatusCode, "Status Code is not '401'");
        }

        /// <summary>
        /// This test checks that employees/id/{employeeId}' request with invalid token returns 401 Unauthorized status code
        /// </summary>
        [Test]
        public void GetEmployeeByIdWithInvalidToken()
        {
            var expectedCompanies = new List<string> { "Employee1", "Employee2", "Employee3" };
            foreach (var company in expectedCompanies)
            {
                Create(_client, company);
            }

            var response = GetAll(_client);
            Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode, "Status Code is not 'OK'");
            Assert.AreEqual(200, (int)response.StatusCode, "Status Code is not '200'");

            //Check Response Body
            var allCompanies = ParseResponse(response);
            foreach (var message in allCompanies)
            {
                var request = new RestRequest($"/id/{message.Id}", Method.GET);
                request.AddHeader(Authorization, Bearer + InvalidToken);
                var getByIdResponse = _client.Execute(request);
                Assert.AreEqual(System.Net.HttpStatusCode.Unauthorized, getByIdResponse.StatusCode, "Status Code is not 'Unauthorized'");
                Assert.AreEqual(401, (int)getByIdResponse.StatusCode, "Status Code is not '401'");
            }
        }

        /// <summary>
        /// This test checks that 'POST /employees' request with invalid token returns 401 Unauthorized status code
        /// </summary>
        [Test]
        public void CreateEmployeeWithInvalidToken()
        {
            //Create two companies
            var request = new RestRequest(Method.POST);
            request.AddHeader(Authorization, Bearer + InvalidToken);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(new { Name = "Test" });
            var response = _client.Execute(request);

            Assert.AreEqual(System.Net.HttpStatusCode.Unauthorized, response.StatusCode, "Status Code is not 'Unauthorized'");
            Assert.AreEqual(401, (int)response.StatusCode, "Status Code is not '401'");
        }

        /// <summary>
        /// This test checks that 'DELETE /employees/id/{employeeId}' request with invalid token returns 401 Unauthorized status code
        /// </summary>
        [Test]
        public void DeleteEmployeeWithInvalidToken()
        {
            //Create Company and check that it is created
            Create(_client, "Employee1");
            var company = GetAll(_client);
            var restMessages = ParseResponse(company);
            Assert.AreEqual(1, restMessages.Count, "One employee must be in the list");
            //Delete Company
            var request = new RestRequest($"/id/{restMessages[0].Id}", Method.DELETE);
            request.AddHeader(Authorization, Bearer + InvalidToken);
            var response = _client.Execute(request);

            Assert.AreEqual(System.Net.HttpStatusCode.Unauthorized, response.StatusCode, "Status Code is not 'Unauthorized'");
            Assert.AreEqual(401, (int)response.StatusCode, "Status Code is not '401'");
        }
    }
}

