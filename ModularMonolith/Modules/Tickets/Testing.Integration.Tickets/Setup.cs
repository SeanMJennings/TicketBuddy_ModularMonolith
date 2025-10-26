using Common.Environment;
using NUnit.Framework;

namespace Integration;

[SetUpFixture]
public static class Setup
{
    [OneTimeSetUp]
    public static void BeforeAll()
    {
        CommonEnvironment.LocalTesting.SetEnvironment();
    }
    
    [OneTimeTearDown]
    public static void AfterAll()
    {
        CommonEnvironment.LocalDevelopment.SetEnvironment();
    }
}