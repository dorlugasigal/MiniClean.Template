# Overview

Our facade application is structured according to the principles of Clean Architecture. Here's a brief overview of the main components:

- `src/Api`: This is the entry point of our application. It contains the web API controllers (carter modules) and
  startup configuration.
- `src/Core`: This layer contains the business logic and entities of our application.
- `src/Infrastructure`: This layer contains classes for accessing external resources such as databases (DataHub/PDS),
  file systems and external services.
- `tests`: This directory contains all the test projects for our application.

By adhering to the principles of Clean Architecture, we aim to create a system that is independent of UIs, databases,
frameworks, and external agencies. This makes our system testable, independent of the UI, independent of the database,
independent of any external agency, and organized around use cases and features.

## Clean Architecture

Clean Architecture divides the system into layers, each with its own responsibility. The dependencies between these
layers follow the Dependency Rule, which states that dependencies should point inwards towards higher-level policies.

![Clean Architecture](../../assets/clean-architecture.png)

Here's a brief overview of each layer in our Clean Architecture:

### Domain Layer

The Domain Layer, or Entities, encapsulates the most general and high-level rules of a system. It can be an object with
methods, or it can be a set of data structures and functions. In our case, we primarily use models from the FHIR library
as our domain models. Other domain-related models are located in our Core layer.

### Application Layer

The Application Layer, or Use Cases, defines the specific business rules of an application. They encapsulate all the use
cases of a system, which can be initiated by either a user, an external system, or an event like a scheduled job.

### Infrastructure Layer

The Infrastructure Layer, or Interface Adapters, contains all the implementations of the interfaces defined in the
Application Layer. This layer is responsible for communicating with external systems, such as databases, web services,
http clients, etc.

### Presentation Layer

The Presentation Layer is the outermost layer of a system. This layer is responsible for providing a user interface to
the system, in our case a REST API, this layer is also responsible for handling any external events that are triggered
by other systems, and provide a way to communicate with the Application Layer as well for an Api configuration and
startup.

---

## Result Object Pattern

This project uses the [Result Object Pattern](https://www.milanjovanovic.tech/blog/functional-error-handling-in-dotnet-with-the-result-pattern)
to handle errors and exceptions. The Result Object Pattern is a functional
programming pattern that allows us to handle errors and exceptions in a more explicit way than using try/catch blocks.
It also allows us to avoid returning null values from methods. Instead, we return a Result object that contains either
the value, an exception or neither (representing null). We've created a custom Result struct that we use throughout our application.

Example usage from the `DataHubFhirClient` class:

```csharp
public async Task<Result<T>> GetResource<T>(string resourceId) where T : Resource
{
    var resourceType = ModelInfo.GetFhirTypeNameForType(typeof(T));
    logger.LogInformation("Fetching resource {ResourceType}/{ResourceId} from FHIR service.", resourceType, resourceId);

    try
    {
        var response = await dataHubFhirClient.ReadAsync<T>($"{resourceType}/{resourceId}");
        return response; // The return value will be implicitly converted to a Result<T> object with success status
    }
    catch (FhirOperationException ex) when (ex.Status == HttpStatusCode.NotFound)
    {
        logger.LogDebug("Resource {ResourceType}/{ResourceId} not found in FHIR service.", resourceType, resourceId);
        return ex; // The exception will be implicitly converted to a Result<T> object with failure status
    }
    catch (Exception ex)
    {
        logger.LogError("Error fetching resource {ResourceType}/{ResourceId} from FHIR service: {ErrorMessage}", resourceType, resourceId, ex.Message);
        return ex; // The exception will be implicitly converted to a Result<T> object with failure status
    }
}
```

If a method returns a null value, it will be implicitly converted to a corresponding
Result object with neither a value nor an exception. This represents the null value,
and can be checked using the `IsNull` property.

For methods with a void return type, we use the `Result` struct instead of `Result<T>`.
For example:

```csharp
public Result doSomething()
{
    try
    {
        // Do something...
        return Result.Success();
    }
    catch (Exception ex)
    {
        return ex; // Can also return Result.Failure(ex)
    }
}
```

---

## Centralized Project and Package Configuration

In our solution, we use `Directory.Build.props` and `Directory.Packages.props` files to centralize the configuration of our projects and NuGet packages. These files are located at the root of our solution and are automatically imported by all `.csproj` files. This helps us maintain consistency across our projects and manage our NuGet packages more efficiently.

### Directory.Build.props

The `Directory.Build.props` file is used to define common MSBuild properties that are shared across all projects. This includes properties such as the target framework, nullable reference types setting, and others. By defining these properties in a central location, we ensure that all our projects are using the same settings, which helps us maintain consistency and avoid duplication.

Here's an example of what our `Directory.Build.props` file looks like:

```xml
<Project>
    <PropertyGroup>
        <TargetFramework>net8.0</TargetFramework>
        <Nullable>enable</Nullable>
        <ImplicitUsings>enable</ImplicitUsings>
        <InvariantGlobalization>true</InvariantGlobalization>

        <AnalysisMode>Recommended</AnalysisMode>
        <CodeAnalysisTreatWarningsAsErrors>true</CodeAnalysisTreatWarningsAsErrors>
    </PropertyGroup>
</Project>
```

we're setting the target framework to .NET 8.0, enabling nullable reference types and implicit usings, and setting the globalization invariant to true,
in analysis mode we are using the recommended rules and we are treating warnings as errors.

### Directory.Packages.props

The `Directory.Packages.props` file is used to manage our NuGet packages centrally. This file contains `PackageVersion` items for all the NuGet packages that are used across our projects. By defining these items in a central location, we can manage our NuGet packages versions more efficiently and ensure that all our projects are using the same package versions.
in addition to that, this is a great preparation for using tools like Paket for dependency management.

Here's an example of the Common section in our `Directory.Packages.props` file :

```xml
<Project>
    <PropertyGroup>
        <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
    </PropertyGroup>
    
    <ItemGroup Label="Common">
        <PackageVersion Include="Microsoft.Extensions.Http" Version="8.0.0"/>
        <PackageVersion Include="Microsoft.Extensions.Logging.Abstractions" Version="8.0.0"/>
        <PackageVersion Include="Microsoft.Extensions.DependencyInjection.Abstractions" Version="8.0.0"/>
    </ItemGroup>
</Project>
```

we're defining `PackageVersions` for nuget packages that are used across all our projects. This includes packages such as `Microsoft.Extensions.Http`, `Microsoft.Extensions.Logging.Abstractions`, `Microsoft.Extensions.DependencyInjection.Abstractions`.
this way Projects that use both of these packages will use the same version.

### .csproj files

In your `.csproj` files, you don't need to specify the versions of the packages or the common properties that you have defined in `Directory.Build.props` and `Directory.Packages.props`. The versions and properties will be automatically applied to all projects in your solution. Here's an example of how a `.csproj` file might look:

```xml
<Project Sdk="Microsoft.NET.Sdk">
    
  <ItemGroup>
    <PackageReference Include="Microsoft.Extensions.Http" />
    <PackageReference Include="Microsoft.Extensions.Logging.Abstractions" />
    <PackageReference Include="Microsoft.Extensions.DependencyInjection.Abstractions" />
  </ItemGroup>

</Project>
```
