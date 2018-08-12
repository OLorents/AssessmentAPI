using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using NUnit.Framework;
using RestSharp;

namespace AssestmentApi.IntegrationTests.Tests
{
  public class Company : TestBase
    {
        private const string CompaniesBaseUrl = "https://mobilewebserver9-pokertest8ext.installprogram.eu/TestApi/api/automation/companies";
        private RestClient _client;
        

        [SetUp]
        public override void TestSetUp()
        {
            base.TestSetUp();
            _client = new RestClient(CompaniesBaseUrl);

            var response = GetAll(_client);
            var restMessages = ParseResponse(response);
            foreach (var restMessage in restMessages)
            {
                DeleteById(_client, restMessage.Id);
            }
        }
       
        /// <summary>
        /// This test checks that 'GET /companies' request returns empty data when no companies exist
        /// </summary>
        [Test]
        public void GetAllCompanies_EmptyData()
        {
            //GET All companies
            var response = GetAll(_client);
           
            Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode, "Status Code is not 'OK'");
            Assert.AreEqual(200, (int)response.StatusCode, "Status Code is not '200'");
            Assert.AreEqual("[]", response.Content);
        }

        /// <summary>
        /// This test checks that 'GET /companies' request returns all companies
        /// </summary>
        [Test]
        public void GetAllCompanies_TwoCompaniesExist()
        {
            //Create two companies
            var expectedCompanies = new List<string>{ "Company1", "Company2" };
            foreach (var company in expectedCompanies)
            {
                Create(_client, company);
            }

            //GET All companies
            var response = GetAll(_client);

            Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode, "Status Code is not 'OK'");
            Assert.AreEqual(200, (int)response.StatusCode, "Status Code is not '200'");

            var restMessages = ParseResponse(response);
            Assert.AreEqual(2, restMessages.Count, "Two companies must be in the list");

            var actualCompanies = new List<string>();
            var actualIds = new List<int?>();
            foreach (var message in restMessages)
            {
                actualIds.Add(message.Id);
                actualCompanies.Add(message.Name);
            }
            Assert.IsTrue(expectedCompanies.All(actualCompanies.Contains), "Actual companies must be same as expected");
            Assert.AreEqual(2, actualIds.Count, "'Id' should be present in the Response Body for each company ");
        }

        /// <summary>
        /// This test checks that 'GET /companies/id/{companyId}' request returns company by id
        /// </summary>
        [Test]
        public void GetCompanyById()
        {
            //Create three companies
            var expectedCompanies = new List<string> { "Company1", "Company2", "Company3" };
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
                var getByIdResponse = GetById(_client, message.Id);
                var companyById = JsonConvert.DeserializeObject<ResponseBody>(getByIdResponse.Content);
                Assert.IsTrue(expectedCompanies.Contains(companyById.Name), "Company name is not expected");
                Assert.IsTrue(companyById.Id == null, "'Id' should not be present in the Response Body");
            }
        }

        /// <summary>
        /// This test checks that 'GET /companies/id/{companyId}'request for unknow resource returns 404 NotFound status code
        /// </summary>
        [Test]
        public void GetCompanyByInvalidId()
        {
            var response = GetById(_client, 1000);
            Assert.AreEqual(System.Net.HttpStatusCode.NotFound, response.StatusCode, "Status Code is not 'NotFound'");
            Assert.AreEqual(404, (int)response.StatusCode, "Status Code is not '404'");
        }

        /// <summary>
        /// This test checks that 'POST /companies' request creates company 
        /// </summary>
        [Test]
        public void CreateCompany()
        {
            //Create two companies
            var expectedCompanies = new List<string> { "Company1", "Company2" };
            foreach (var company in expectedCompanies)
            {
                Create(_client, company);
            }

           //make a GET request and ensure the data was saved correctly
           var response = GetAll(_client);

            Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode, "Status Code is not 'OK'");
            Assert.AreEqual(200, (int)response.StatusCode, "Status Code is not '200'");

            //Check Response Body
            var restMessages = ParseResponse(response);
            Assert.AreEqual(2, restMessages.Count, "Two companies must be in the list");

            var actualCompanies = new List<string>();
            var actualIds = new List<int?>();
            foreach (var message in restMessages)
            {
                actualIds.Add(message.Id);
                actualCompanies.Add(message.Name);
            }
            Assert.IsTrue(expectedCompanies.All(actualCompanies.Contains), "Created companies are different from expected");
            Assert.AreEqual(2, actualIds.Count, "'Id' shoud be present in the Response Body for each created company");
        }

        /// <summary>
        /// This test checks that 'POST /companies' request fails with incorrect data 
        /// </summary>
        [Test]
        public void CreateCompanyWithIncorrectData()
        {
            RestRequest request = new RestRequest(Method.POST);
            request.AddHeader(Authorization, Bearer + Token);
            request.RequestFormat = DataFormat.Json;
            //Set 'Id' instead od 'Name' in the RequestBody
            request.AddBody(new { Id = "TestCompany" });
            var response = _client.Execute(request);
           
            Assert.AreEqual(System.Net.HttpStatusCode.BadRequest, response.StatusCode, "Status Code is not 'BadRequest'");
            Assert.AreEqual(400, (int)response.StatusCode, "Status Code is not '400'");
        }

        /// <summary>
        /// This test checks that 'DELETE /companies/id/{companyId}' request delets company by Id 
        /// </summary>
        [Test]
        public void DeleteCompany()
        {
            //Create Company and check that it is created
            Create(_client, "Company1");
            var company = GetAll(_client);
            var restMessages = ParseResponse(company);
            Assert.AreEqual(1, restMessages.Count, "One company must be in the list");
            //Delete Company
            var response = DeleteById(_client, restMessages[0].Id);
            Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode, "Status Code is not 'OK'");
            Assert.AreEqual(200, (int)response.StatusCode, "Status Code is not '200'");
            //Check that Company is deleted, empty data should be returned
            company = GetAll(_client);
            restMessages = ParseResponse(company);
            Assert.AreEqual(0, restMessages.Count, "Zero companies must be in the list");
        }

        /// <summary>
        /// This test checks that 'DELETE /companies/id/{companyId}' request for unknow resource returns 404 NotFound status code
        /// </summary>
        [Test]
        public void DeleteUnknownResource()
        {
            //Delete Company with Id = 1000, such Id is not in the system
            var response = DeleteById(_client, 1000);
            Assert.AreEqual(System.Net.HttpStatusCode.NotFound, response.StatusCode, "Status Code is not 'NotFound'");
            Assert.AreEqual(404, (int)response.StatusCode, "Status Code is not '404'");
        }

        //==========================InvalidToken========================================================

        /// <summary>
        /// This test checks that 'GET /companies' request with invalid token returns 401 Unauthorized status code
        /// </summary>
        [Test]
        public void GetAllCompaniesWithInvalidToken()
        {
            var request = new RestRequest(Method.GET);
            request.AddHeader(Authorization, Bearer + InvalidToken);
            var response = _client.Execute(request);

            Assert.AreEqual(System.Net.HttpStatusCode.Unauthorized, response.StatusCode, "Status Code is not 'Unauthorized'");
            Assert.AreEqual(401, (int)response.StatusCode, "Status Code is not '401'");
        }

        /// <summary>
        /// This test checks that companies/id/{companyId}' request with invalid token returns 401 Unauthorized status code
        /// </summary>
        [Test]
        public void GetCompanyByIdWithInvalidToken()
        {
            var expectedCompanies = new List<string> { "Company1", "Company2", "Company3" };
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
        /// This test checks that 'POST /companies' request with invalid token returns 401 Unauthorized status code
        /// </summary>
        [Test]
        public void CreateCompanyWithInvalidToken()
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
        /// This test checks that 'DELETE /companies/id/{companyId}' request with invalid token returns 401 Unauthorized status code
        /// </summary>
        [Test]
        public void DeleteCompanyWithInvalidToken()
        {
            //Create Company and check that it is created
            Create(_client, "Company1");
            var company = GetAll(_client);
            var restMessages = ParseResponse(company);
            Assert.AreEqual(1, restMessages.Count, "One company must be in the list");
            //Delete Company
            var request = new RestRequest($"/id/{restMessages[0].Id}", Method.DELETE);
            request.AddHeader(Authorization, Bearer + InvalidToken);
            var response = _client.Execute(request);

            Assert.AreEqual(System.Net.HttpStatusCode.Unauthorized, response.StatusCode, "Status Code is not 'Unauthorized'");
            Assert.AreEqual(401, (int)response.StatusCode, "Status Code is not '401'");
        }
    }
}

