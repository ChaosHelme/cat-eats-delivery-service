using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using AwesomeAssertions;
using CatEats.UserService.Application.Commands;
using CatEats.UserService.Application.DTOs;
using Reqnroll;

namespace CatEats.UserService.IntegrationTests;

[Binding]
public class UserRegistrationStepDefinitions
{
    private readonly TestContext _testContext;
    private RegisterCustomerCommand? _customerCommand;
    private RegisterRiderCommand? _riderCommand;
    private HttpResponseMessage? _response;
    private UserDto? _registeredUser;

    public UserRegistrationStepDefinitions(TestContext testContext)
    {
        _testContext = testContext;
    }

    [Given(@"the user service is running")]
    public void GivenTheUserServiceIsRunning()
    {
        // The test context handles service startup
        _testContext.HttpClient.Should().NotBeNull();
    }

    [Given(@"the database is clean")]
    public async Task GivenTheDatabaseIsClean()
    {
        await _testContext.ResetDatabaseAsync();
    }

    [Given(@"a customer exists with email ""(.*)""")]
    public async Task GivenACustomerExistsWithEmail(string email)
    {
        var command = new RegisterCustomerCommand
        {
            Email = email,
            FirstName = "Existing",
            LastName = "Customer",
            PhoneNumber = "1234567890"
        };

        var response = await _testContext.HttpClient.PostAsJsonAsync("/api/users/customers", command);
        response.EnsureSuccessStatusCode();
    }

    [When(@"I register a new customer with the following details:")]
    public async Task WhenIRegisterANewCustomerWithTheFollowingDetails(Table table)
    {
        var row = table.Rows[0];
        _customerCommand = new RegisterCustomerCommand
        {
            Email = row["Email"],
            FirstName = row["FirstName"],
            LastName = row["LastName"],
            PhoneNumber = row["PhoneNumber"]
        };

        _response = await _testContext.HttpClient.PostAsJsonAsync("/api/users/customers", _customerCommand);
    }

    [When(@"I register a new rider with the following details:")]
    public async Task WhenIRegisterANewRiderWithTheFollowingDetails(Table table)
    {
        var row = table.Rows[0];
        _riderCommand = new RegisterRiderCommand
        {
            Email = row["Email"],
            FirstName = row["FirstName"],
            LastName = row["LastName"],
            PhoneNumber = row["PhoneNumber"]
        };

        _response = await _testContext.HttpClient.PostAsJsonAsync("/api/users/riders", _riderCommand);
    }

    [When(@"I try to register a new customer with email ""(.*)""")]
    public async Task WhenITryToRegisterANewCustomerWithEmail(string email)
    {
        _customerCommand = new RegisterCustomerCommand
        {
            Email = email,
            FirstName = "Test",
            LastName = "User",
            PhoneNumber = "1234567890"
        };

        _response = await _testContext.HttpClient.PostAsJsonAsync("/api/users/customers", _customerCommand);
    }

    [When(@"I try to register a customer with invalid email ""(.*)""")]
    public async Task WhenITryToRegisterACustomerWithInvalidEmail(string invalidEmail)
    {
        _customerCommand = new RegisterCustomerCommand
        {
            Email = invalidEmail,
            FirstName = "Test",
            LastName = "User",
            PhoneNumber = "1234567890"
        };

        _response = await _testContext.HttpClient.PostAsJsonAsync("/api/users/customers", _customerCommand);
    }

    [When(@"I try to register a customer with empty first name")]
    public async Task WhenITryToRegisterACustomerWithEmptyFirstName()
    {
        _customerCommand = new RegisterCustomerCommand
        {
            Email = "test@example.com",
            FirstName = "",
            LastName = "User",
            PhoneNumber = "1234567890"
        };

        _response = await _testContext.HttpClient.PostAsJsonAsync("/api/users/customers", _customerCommand);
    }

    [Then(@"the customer should be registered successfully")]
    public async Task ThenTheCustomerShouldBeRegisteredSuccessfully()
    {
        _response.Should().NotBeNull();
        _response!.StatusCode.Should().Be(HttpStatusCode.Created);

        var content = await _response.Content.ReadAsStringAsync();
        _registeredUser = JsonSerializer.Deserialize<UserDto>(content, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        _registeredUser.Should().NotBeNull();
        _registeredUser!.Email.Should().Be(_customerCommand!.Email);
        _registeredUser.FirstName.Should().Be(_customerCommand.FirstName);
        _registeredUser.LastName.Should().Be(_customerCommand.LastName);
        _registeredUser.PhoneNumber.Should().Be(_customerCommand.PhoneNumber);
    }

    [Then(@"the rider should be registered successfully")]
    public async Task ThenTheRiderShouldBeRegisteredSuccessfully()
    {
        _response.Should().NotBeNull();
        _response!.StatusCode.Should().Be(HttpStatusCode.Created);

        var content = await _response.Content.ReadAsStringAsync();
        _registeredUser = JsonSerializer.Deserialize<UserDto>(content, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        _registeredUser.Should().NotBeNull();
        _registeredUser!.Email.Should().Be(_riderCommand!.Email);
        _registeredUser.FirstName.Should().Be(_riderCommand.FirstName);
        _registeredUser.LastName.Should().Be(_riderCommand.LastName);
        _registeredUser.PhoneNumber.Should().Be(_riderCommand.PhoneNumber);
    }

    [Then(@"the customer should have the role ""(.*)""")]
    public void ThenTheCustomerShouldHaveTheRole(string expectedRole)
    {
        _registeredUser.Should().NotBeNull();
        _registeredUser!.Role.ToString().Should().Be(expectedRole);
    }

    [Then(@"the rider should have the role ""(.*)""")]
    public void ThenTheRiderShouldHaveTheRole(string expectedRole)
    {
        _registeredUser.Should().NotBeNull();
        _registeredUser!.Role.ToString().Should().Be(expectedRole);
    }

    [Then(@"the customer should have the status ""(.*)""")]
    public void ThenTheCustomerShouldHaveTheStatus(string expectedStatus)
    {
        _registeredUser.Should().NotBeNull();
        _registeredUser!.Status.ToString().Should().Be(expectedStatus);
    }

    [Then(@"the rider should have the status ""(.*)""")]
    public void ThenTheRiderShouldHaveTheStatus(string expectedStatus)
    {
        _registeredUser.Should().NotBeNull();
        _registeredUser!.Status.ToString().Should().Be(expectedStatus);
    }

    [Then(@"the registration should fail with ""(.*)""")]
    public async Task ThenTheRegistrationShouldFailWith(string expectedError)
    {
        _response.Should().NotBeNull();
        _response!.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var content = await _response.Content.ReadAsStringAsync();
        content.Should().Contain(expectedError);
    }

    [Then(@"the registration should fail with validation error")]
    public void ThenTheRegistrationShouldFailWithValidationError()
    {
        _response.Should().NotBeNull();
        _response!.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}