# Pipelines

## PR Pipeline (`pr-pipeline.yaml`)

### Overview

This pipeline is triggered on pull requests to the `main` branch and consists of two stages: Static Code Analysis and Services Build Verification Tests.

### Stages

#### 1. Static Code Analysis

- **Description:** Executes static code analysis on the codebase.

- **Jobs:**
  - **RunAnalysis:**
    - **Steps:**
      - Uses a template (`static-code-analysis.yaml`) for executing static code analysis.

#### 2. Services Build Verification Tests

- **Description:** Performs Build Verification Tests (BVT) for services and executes End-to-End (E2E) tests.

- **Jobs:**
  - **BVT:**
    - **Strategy:**
      - Builds and tests the specified service (e.g., `api`) with a coverage gate of 0% ATM.
    - **Steps:**
      - Uses a template (`bvt.yaml`) with parameters for service name, project folder, and coverage gate.
  - **E2ETests:**
    - **Dependencies:** Depends on the completion of the BVT job.
    - **Steps:**
      - Uses a template (`e2e-tests.yaml`) for executing End-to-End tests.

---

## CI Pipeline (`ci-pipeline.yaml`)

This pipeline is triggered on changes to the `main` branch and has the same structure of the PR pipeline with Static Code Analysis and Services Build Verification Tests stages.

---

## BVT Template (`bvt.yaml`)

### Overview

This template defines steps for Build Verification Tests (BVT) for a specified service.

### Parameters

- **SERVICE_NAME:** The name of the service to be tested (should be similar to the name in `docker-compose.yml`)
- **PROJECT_FOLDER:** The folder containing the service project.
- **COVERAGE_GATE:** The coverage gate percentage.

### Steps

1. **Build:**
   - Builds the specified service using Docker Compose.

2. **Unit Tests:**
   - Executes unit tests for the service.

3. **Integration Tests:**
   - Runs integration tests using Docker Compose, and if a `start.sh` script exists, it is executed before running tests.
   - the `start.sh` script is used to start the service and any dependencies (e.g., db, local fhir service) before running tests.

4. **Publish Test Results:**
   - Publishes test results in XUnit format.

5. **Unify Test Coverage Reports:**
   - Installs `dotnet-reportgenerator-globaltool` and generates a unified code coverage report in Cobertura format.

6. **Publish Code Coverage Results:**
   - Publishes the generated code coverage results.

7. **Quality Gate:**

- The last section provides an example of adding a quality check based on test line coverage using the [Build Quality Checks](https://marketplace.visualstudio.com/items?itemName=mspremier.BuildQualityChecks) extension.

---

## Static Code Analysis Template (`static-code-analysis.yaml`)

### Overview

This template includes steps for static code analysis, detecting secrets, and lint checks.

### Steps

1. **Detect Secrets Placeholder:**
   - Placeholder for detecting secrets in the codebase.

2. **Lint check Placeholder:**
   - Placeholder for lint checks.

#### Advanced Security Tasks (Optional - Commented Out)

- If using GitHub Advanced Security, the commented-out section provides tasks for CodeQL initialization, autobuild, dependency scanning, and code analysis.

---

## E2E Tests Template (`e2e-tests.yaml`)

### Overview

This template serves as a placeholder for executing End-to-End (E2E) tests.

### Steps

1. **Run E2E Placeholder:**
    - placeholder for running E2E tests.

---

## CD Pipeline (`cd-pipeline.yaml`)

### Overview

This pipeline is meant for the infrastructure and applications deployment of the solution. It can deploy to multiple environments, based on a parameter, and can be triggered manually for any git revision, tag, or commit, thus it gives the flexibility to deploy any version of the application to any environment.  
By default pipeline is triggered on a completion of a CI pipeline instance, for deploying a DEV environment, in a continuous deployment manner.  
Every environment name maps to a corresponding AzDO variable group, which holds the environment specific variables:

### Variable Group Keys

> :warning: The list of keys below is incomplete and will be clarified shortly.

#### dev-secret

- **AZURE_TENANT_ID:** The Azure tenant id to deploy into.
- **azureSubscriptionEndpoint:** Specifies the Azure Resource Manager service connection. In our case, defined using workload identity federation with openid connect.

#### dev-variables

- AZURE_SUBSCRIPTION_ID
- AZURE_TENANT_ID
- azureSubscriptionEndpoint
- location
- shared_resource_group_name
- tf_status_storage_account

### Stages

1. **Deploy infrastructure:**
    2. Add Agent IP to Key Vault vnet_- in order for the ADO agent to be able to access the key vault during deployment, the agent's IP address needs to be added to the key vault's firewall.
    3. Create terraform tfvars file
    4. Apply Terraform Infrastructure as Code, using the tfvars file created in the previous step. Note that the tfstate file is stored in an Azure Storage Account, and the connection details are provided to Terraform via the `backend-config` parameters.

2. **Deploy services:**
    1. Build and push API container image to ACR (with both buildId and `latest` as tags)
    2. Deploy API container image to its dedicated App Service (using the buildId tag)
    3. Update Agent IP in App Gateway WAF policy
    4. Update FHIR Profiles in the FHIR service.
    5. Push Conversion Liquid Template OCI Images to ACR (with both buildId and `latest` as tags).
    6. Run smoke tests to verify the deployment.
