using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using RestSharp;

namespace AssestmentApi.IntegrationTests
{
    [TestFixture]
   public class TestBase
    {      
        protected string Token;
        protected string InvalidToken = "InvalidToken";
        protected string UserName = "testName1";
        protected string Password = "test";
        protected string GrantType = "password";
        protected const string ServiceBaseUrl = "https://mobilewebserver9-pokertest8ext.installprogram.eu/TestApi";
        protected string Authorization = "Authorization";
        protected string Bearer = "Bearer ";

        [SetUp]
        public virtual void TestSetUp()
        {
            Token = GetToken();
        }

        public string GetToken()
        {            
            var client = new RestClient(ServiceBaseUrl);
            var request = new RestRequest("/token", Method.POST);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("application/x-www-form-urlencoded", $"Username={UserName}&Password={Password}&grant_type={GrantType}", ParameterType.RequestBody);
            var response = client.Execute(request);
            JObject results = JObject.Parse(response.Content);
            var token = (string)results["access_token"];
            return token.Length > 0 ? token : null;
        }

        #region Helper Methods

        protected void Create(RestClient client, string name)
        {
            var request = new RestRequest(Method.POST);
            request.AddHeader(Authorization, Bearer + Token);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(new { Name = name });
            client.Execute(request);
        }

        protected RestResponse GetAll(RestClient client)
        {
            var request = new RestRequest(Method.GET);
            request.AddHeader(Authorization, Bearer + Token);
            return client.Execute(request);
        }

        protected RestResponse GetById(RestClient client, int? id)
        {
            var request = new RestRequest($"/id/{id}", Method.GET);
            request.AddHeader(Authorization, Bearer + Token);
            return client.Execute(request);
        }

        protected RestResponse DeleteById(RestClient client, int? id)
        {
            var request = new RestRequest($"/id/{id}", Method.DELETE);
            request.AddHeader(Authorization, Bearer + Token);
            return client.Execute(request);
        }

        protected List<ResponseBody> ParseResponse(RestResponse response)
        {
            return JsonConvert.DeserializeObject<List<ResponseBody>>(response.Content);
        }
        #endregion
    }
}
