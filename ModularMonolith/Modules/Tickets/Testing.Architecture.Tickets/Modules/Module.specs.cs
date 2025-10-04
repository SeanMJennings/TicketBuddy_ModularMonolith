namespace Testing.Architecture.Tickets.Modules;

internal partial class ModuleSpecs
{
    [Test]
    public void domain_layer_should_not_reference_application_layer()
    {
        When(checking_the_domain_layer_for_application_layer_references);
        Then(there_should_be_no_references_to_application_layer);
    }

    [Test]
    public void application_layer_should_not_reference_infrastructure_layer()
    {
        Given(checking_the_application_layer_for_infrastructure_layer_references);
        Then(there_should_be_no_references_to_infrastructure_layer);
    }
    
    [Test]
    public void infrastructure_layer_should_not_reference_controller_layer()
    {
        Given(checking_the_infrastructure_layer_for_controller_layer_references);
        Then(there_should_be_no_references_to_controller_layer);
    }

    [Test]
    public void domain_layer_should_only_reference_integration_project_from_another_module()
    {
        Given(checking_the_domain_layer_for_references_to_other_modules);
        Then(there_should_be_no_references_to_other_modules_except_integration_projects);
    }
}