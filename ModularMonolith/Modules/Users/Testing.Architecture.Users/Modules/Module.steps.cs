using System.Reflection;
using BDD;
using Domain.Users.Entities;
using NetArchTest.Rules;

namespace Testing.Architecture.Users.Modules;

[TestFixture]
internal partial class ModuleSpecs : Specification
{
    private static Assembly DomainAssembly => typeof(User).Assembly;
    private static Assembly ApplicationAssembly => typeof(Application.Users.Commands.UserCommands).Assembly;
    private static Assembly InfrastructureAssembly => typeof(Infrastructure.Users.Persistence.UserRepository).Assembly;
    private static Assembly ControllerAssembly => typeof(Controllers.Users.UserController).Assembly;
    private TestResult testResult;
    
    protected override void before_each()
    {
        base.before_each();
        testResult = null!;
    }
    
    private void checking_the_domain_layer_for_application_layer_references()
    {
        testResult = Types.InAssembly(DomainAssembly)
            .Should()
            .NotHaveDependencyOn(ApplicationAssembly.GetName().Name)
            .GetResult();
    }

    private void checking_the_application_layer_for_infrastructure_layer_references()
    {
        testResult = Types.InAssembly(ApplicationAssembly)
            .Should()
            .NotHaveDependencyOn(InfrastructureAssembly.GetName().Name)
            .GetResult();
    }    
    
    private void checking_the_infrastructure_layer_for_controller_layer_references()
    {
        testResult = Types.InAssembly(InfrastructureAssembly)
            .Should()
            .NotHaveDependencyOn(ControllerAssembly.GetName().Name)
            .GetResult();
    }

    private void there_should_be_no_references_to_infrastructure_layer()
    {
        Assert.That(testResult.FailingTypes, Is.Null.Or.Empty);
    }

    private void there_should_be_no_references_to_application_layer()
    {
        Assert.That(testResult.FailingTypes, Is.Null.Or.Empty);
    }

    private void there_should_be_no_references_to_controller_layer()
    {
        Assert.That(testResult.FailingTypes, Is.Null.Or.Empty);
    }
}