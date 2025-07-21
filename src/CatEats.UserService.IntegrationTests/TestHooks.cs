using Reqnroll;

namespace CatEats.UserService.IntegrationTests;

[Binding]
public class TestHooks
{
    private static TestContext? _testContext;

    [BeforeTestRun]
    public static async Task BeforeTestRun()
    {
        _testContext = new TestContext();
        await _testContext.InitializeAsync();
    }

    [AfterTestRun]
    public static async Task AfterTestRun()
    {
        if (_testContext != null)
        {
            await _testContext.DisposeAsync();
        }
    }

    [BeforeScenario]
    public void BeforeScenario(ScenarioContext scenarioContext)
    {
        scenarioContext.ScenarioContainer.RegisterInstanceAs(_testContext!);
    }
}