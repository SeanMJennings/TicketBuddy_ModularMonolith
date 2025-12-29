namespace Testing.Architecture.Events.Modules;

internal partial class ModuleSpecs
{
    [Test]
    public void domain_project_should_only_reference_system_and_common_domain()
    {
        When(checking_the_domain_project_dependencies);
        Then(it_should_only_have_dependencies_on_system_and_common_domain);
    }

    [Test]
    public void application_project_should_only_reference_domain_and_messages()
    {
        When(checking_the_application_project_dependencies);
        Then(it_should_only_have_dependencies_on_domain_and_messages);
    }

    [Test]
    public void infrastructure_project_should_only_reference_application_and_common_infrastructure()
    {
        When(checking_the_infrastructure_project_dependencies);
        Then(it_should_only_have_dependencies_on_application_and_common_infrastructure);
    }

    [Test]
    public void controllers_project_should_only_reference_application_and_domain()
    {
        When(checking_the_controllers_project_dependencies);
        Then(it_should_only_have_dependencies_on_application_and_domain);
    }

    [Test]
    public void messaging_project_should_only_reference_application_and_messages()
    {
        When(checking_the_messaging_project_dependencies);
        Then(it_should_only_have_dependencies_on_application_and_messages);
    }

    [Test]
    public void messages_project_should_only_reference_domain()
    {
        When(checking_the_messages_project_dependencies);
        Then(it_should_only_have_dependencies_on_domain);
    }
}