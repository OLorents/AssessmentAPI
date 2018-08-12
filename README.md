# AssessmentAPI
This project provides Integration Tests for 'The Assessment API'
-  Project has been created using Microsoft Visual Studio 2017
- **RESTSharp** is included as nuget package and used to test REST API
- **Newtonsoft.json** is included as nuget package and used to DeserializeObject
- Tests have been written in **NUnit**

**Run Tests**

  Do the folowing steps to run tests:
- Clone this repository
- Open the solution in Visual Studio
- Rebuild
- From the Test menu item, run the tests

![alt text](https://github.com/OLorents/AssessmentAPI/blob/master/AssestmentApi.IntegrationTests/Images/RunTests.png)

- Open the Test Explorer window, and check test results

![alt text](https://github.com/OLorents/AssessmentAPI/blob/master/AssestmentApi.IntegrationTests/Images/OpenTestExplorer.png)

![alt text](https://github.com/OLorents/AssessmentAPI/blob/master/AssestmentApi.IntegrationTests/Images/TestExplorer.png)

**Test Coverage**

Following cases are covered:
1. **Create**: Post /companies and Post /employees
 - Create company/employee and check a 200 OK status code is returned
 - Check the data is saved correctly
 - Check that POST request fails with incorrect data in Request Body, a 400 BadRequest status code is returned
 - Check that POST request fails with InvalidToken, a 401 Unauthorized status code is returned(Autherization is required for all requests)
2. **GetAll**: GET - /companies and GET - /employees
  - Check that valid GET request returns a 200 OK status code
  - Check that GET request returns empty data when no data exists
  - Check that GET request returns correct data if it exists, check that
  - Check that GET request fails with InvalidToken, a 401 Unauthorized status code is returned
3. **GetById**: GET - /companies/id/{companyId} and GET - /employees /id/{employeeId}
   - Check that a valid GetById request returns correct data ('id' is not present in Response Body)
   - Check that valid GetById request returns a 200 OK status code
   - Check that GetById request fails with InvalidToken, a 401 Unauthorized status code is returned
   - Check that GetById request with invalid 'Id' fails. a 404 NotFound status code is returned
4. **DeleteById**: DELETE - /companies/id/{companyId} and DELETE - /employees/id/{employeeId}
    - Create company/employee and then delete it, check that resourse is deleted
    - Check that valid DELETE request returns a 200 OK status code
    - Check that DELETE unknown resource returns a 404 not found status code
    - Check that DELETE request fails with InvalidToken, a 401 Unauthorized status code is returned
    
  Project could be extended with additional integration tests
  
  Company related tests are located in Tests/Company.cs class
  
  Employee related tests are located in Tests/Emloyee.cs class
  
  
 





