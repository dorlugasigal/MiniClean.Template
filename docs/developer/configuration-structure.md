# Configuration Structure
# ------------------------
There are five main configuration files in the `api` project. These are:
1. appsettings.json
2. appsettings.Local.json
3. appsettings.Integration.json
4. appsettings.Development.json
5. appsettings.Production.json

The `appsettings.json` file contains the default/common configuration for the application, and the contains placeholders for the environment specific configuration.  
The other files are used to override the default configuration based on the environment the application is running in. The `appsettings.Local.json` file is used for local development, and the `appsettings.Integration.json` file is used for integration testing. The `appsettings.Development.json` file is used for development environment, and the `appsettings.Production.json` file is used for production and staging.