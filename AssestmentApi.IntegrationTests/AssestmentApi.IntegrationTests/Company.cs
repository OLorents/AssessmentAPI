using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using RestSharp;
using Newtonsoft.Json;

namespace AssestmentApi.IntegrationTests
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

            var response = GetAllCompanies();
            var restMessages = ParseResponse(response);
            foreach (var restMessage in restMessages)
            {
                DeleteCompany(restMessage.Id);
            }
        }

        #region Helper Methods

        private RestResponse GetAllCompanies()
        {
            RestRequest request = new RestRequest(Method.GET);
            request.AddHeader(Authorization, Bearer + Token);
            return _client.Execute(request);
        }

        private List<ResponseBody> ParseResponse(RestResponse response)
        {
            return JsonConvert.DeserializeObject<List<ResponseBody>>(response.Content);
        }

        private RestResponse GetCompanyById(int? id)
        {
            RestRequest request = new RestRequest($"/id/{id}", Method.GET);
            request.AddHeader(Authorization, Bearer + Token);
            return _client.Execute(request);
        }

        private RestResponse DeleteCompany(int? id)
        {
            RestRequest request = new RestRequest($"/id/{id}", Method.DELETE);
            request.AddHeader(Authorization, Bearer + Token);
            return _client.Execute(request);
        }

        private void CreateTestCompany(string name)
        {
            RestRequest request = new RestRequest(Method.POST);
            request.AddHeader(Authorization, Bearer + Token);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(new { Name = name });
            _client.Execute(request);
        }

        #endregion

        /// <summary>
        /// This test checks that 'GET /companies' request returns empty data when no companies exist
        /// </summary>
        [Test]
        public void GetAllCompanies_EmptyData()
        {
            //GET All companies
            var response = GetAllCompanies();
           
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
                CreateTestCompany(company);
            }

            //GET All companies
            var response = GetAllCompanies();

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
                CreateTestCompany(company);
            }

            var response = GetAllCompanies();
            Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode, "Status Code is not 'OK'");
            Assert.AreEqual(200, (int)response.StatusCode, "Status Code is not '200'");

            //Check Response Body
            var allCompanies = ParseResponse(response);
            foreach (var message in allCompanies)
            {
                var getByIdResponse = GetCompanyById(message.Id);
                var companById = JsonConvert.DeserializeObject<ResponseBody>(getByIdResponse.Content);
                Assert.IsTrue(expectedCompanies.Contains(companById.Name), "Company name is not expected");
                Assert.IsTrue(companById.Id == null, "'Id' should not be present in the Response Body");
            }
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
                CreateTestCompany(company);
            }

           //make a GET request and ensure the data was saved correctly
           var response = GetAllCompanies();

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
        /// This test checks that 'POST /companies' request fail with incorrect data 
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
            CreateTestCompany("Company1");
            var company = GetAllCompanies();
            var restMessages = ParseResponse(company);
            Assert.AreEqual(1, restMessages.Count, "One company must be in the list");
            //Delete Company
            var response = DeleteCompany(restMessages[0].Id);
            Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode, "Status Code is not 'OK'");
            Assert.AreEqual(200, (int)response.StatusCode, "Status Code is not '200'");
            //Check that Company is deleted, empty data should be returned
            company = GetAllCompanies();
            restMessages = ParseResponse(company);
            Assert.AreEqual(0, restMessages.Count, "Zero company must be in the list");
        }

        /// <summary>
        /// This test checks that 'DELETE /companies/id/{companyId}' request for unknow resource will return 404 NotFound status code
        /// </summary>
        [Test]
        public void DeleteUnknownResource()
        {
            //Delete Company with Id = 1000, such Id is not in the system
            var response = DeleteCompany(1000);
            Assert.AreEqual(System.Net.HttpStatusCode.NotFound, response.StatusCode, "Status Code is not 'NotFound'");
            Assert.AreEqual(404, (int)response.StatusCode, "Status Code is not '200'");
        }
    }
}
