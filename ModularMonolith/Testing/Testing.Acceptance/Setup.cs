using Common.Environment;
using Domain;
using NUnit.Framework;

namespace Acceptance;

[SetUpFixture]
public static class Setup
{
    [OneTimeSetUp]
    public static void BeforeAll()
    {
        CommonEnvironment.LocalTesting.SetEnvironment();
        JsonSerialization.RegisterConverters(Converters.GetConverters);
    }
    
    [OneTimeTearDown]
    public static void AfterAll()
    {
        CommonEnvironment.LocalDevelopment.SetEnvironment();
    }
}