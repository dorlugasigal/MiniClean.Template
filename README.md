# MiniClean.Template

Welcome to MiniClean.Template! This repository holds the starting point for a .NET 8 minimal API, neatly organized using clean architecture principles. It's datastore is a FHIR service at the moment but can be easily switch to suit your needs.

Here's what you'll find:

- **Clean Architecture**: We've organized the project using clean architecture principles to make it easier to understand, maintain, and extend.
  
- **Ready for CRUD Operations**: an example Patient Read operation is already implemented. You can easily extend or replace it with your own operations.

- **Testing Suites:** Start on solid ground with unit tests, integration tests, and end-to-end tests, all ready to go.

- **Azure DevOps CI Pipeline**: We've included a CI pipeline that handle static code analysis and build verification tests. a similar PR pipelines ensure code quality before merging a PR.

- **CD Pipeline:** Simplify deployment with a CD pipeline, managing Terraform infrastructure stored in the 'infrastructure' folder.

Feel free to explore, submit issues, and use MiniClean.Template to make your small-medium sized .NET 8 projects ready for work!

---

## Getting Started

Follow these steps to get started with the template:

1. **Install Tools**:
   These tools are essential for the CI pipeline to run successfully:
   - [GitLeaks](https://marketplace.visualstudio.com/items?itemName=Foxholenl.Gitleaks)
   - [BuildQualityChecks](https://marketplace.visualstudio.com/items?itemName=mspremier.BuildQualityChecks)

2. **Rename Project**:
   - Replace all instances of "miniclean" with your `appname` in lowercase.

3. **Azure Setup**:
   - Create a common resource group.
   - Create a storage account and a container named "terraform". (used to store the state of terraform)
   - Create a variable groups:
     - `dev-variables`:
       - `location`: As desired
       - `azureSubscriptionEndpoint`: The connection name
       - `shared_resource_group_name`: The common resource group
       - `tf_status_storage_account`: Storage account name
       - `azure_tenant_id`: Azure tenant ID
       - `azure_subscription_id`: Azure subscription ID
   - Create an app registration and save its client ID and secret.
   - In your Azure subscription, add an "Owner" role to this app registration.
   - Create a key vault in the common resource group and assign it a secret reader role for your pipeline connection name.
   - Add these secrets to your key vault:
     - `sp-<appname>-deployment-client-id`: App registration client ID
     - `sp-<appname>-deployment-client-secret`: App registration client secret
   - create an additional variable group `dev-secrets` variable group to Azure Key Vault and add the service principal secrets from the key vault.
   - Assign an AcrPush role to your service principal.

4. **Set up Azure DevOps Pipelines**:
   Choose existing YAML file and pick the appropriate file under the `/.pipelines/` folder
   - Create PR pipeline
   - Create CI pipeline
   - Create CD pipeline
