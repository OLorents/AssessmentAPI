using Newtonsoft.Json.Linq;
using NUnit.Framework;
using RestSharp;

namespace AssestmentApi.IntegrationTests
{
    [TestFixture]
   public class TestBase
    {      
        protected string Token;
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
    }
}
