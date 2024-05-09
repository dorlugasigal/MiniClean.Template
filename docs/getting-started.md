# Getting Started

## Project Structure

This project is structured into several parts:

- `src`: This is where the main application code resides. It's a .NET 8.0 application.
- `tests`: This is where the tests reside.
- `templates`: This directory contains the liquid templates which are used during the FHIR mapping and conversion operations.
- `docker`: This directory contains the Docker Compose file used to run the application locally, along with its dependencies and mounted storage volume(s).
- `docs`: Contains the project documentation, which can be built into a static site using MkDocs, and supplied configuration and docker-compose file.
- `data`: contains sample and test data.

## Local Development

See below for details to get up and running.

Further information for developers wanting to contribute to the project can be found [here](./developer/contribution-guide.md)

### Prerequisites

- [Docker](https://docs.docker.com/get-docker/)
- [.NET SDK 8](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- IDE of choice

### Running the Application Using Docker Compose

#### Set environment variables

Copy the contents of the `docker/.env.template` file to a new `.env` file:

```bash
cp docker/.env.template docker/.env
```

#### Run the API and its dependencies

The docker-compose file located under the 'docker' folder will build and run
a containerized version of the application,
along its dependencies such as the [OSS FHIR server](https://github.com/microsoft/fhir-server)
and the SQL Server database for the FHIR server, as well as an
[Azurite Azure Storage Emulator](https://learn.microsoft.com/en-us/azure/storage/common/storage-use-azurite?tabs=visual-studio%2Cblob-storage)
container for local emulation of Azure Blob Storage.

To run the application using Docker Compose, run the following command from the root of the repository:

```bash
docker-compose -f docker/docker-compose.yml up
```

You can also run individual containers by specifying the service name,
for example to run the FHIR server (and the SQL Server database it depends on):

```bash
docker-compose -f docker/docker-compose.yml up fhir-server
```

> **Note:** When running on Apple Silicon, you need to set the following environment variable on the `fhir-api` services: `DOTNET_EnableWriteXorExecute=0`.

### Running the Application Using the .NET SDK

To run the application using the .NET CLI, run the following commands from the root of the repository:

```bash
cd src/Api
dotnet run
```

Alternatively, you can run and debug the application from your IDE of choice for faster development iterations.
All services defined in the docker-compose have container-to-host port mappings defined, so
the application can be run directly and it will still have access to any relevant dependencies.

### Test the Setup

Once the application is running, check if you have the following services running:

![alt text](assets/api-containers-running.png)

You can test the setup by navigating to the following URLs:
[http://localhost:8000/Patient?family=Smith&gender=female&birthdate=2010-10-22](http://localhost:8000/Patient?family=Smith&gender=female&birthdate=2010-10-22).
You should see a FHIR resource of type Patient with id 9000000009 in JSON format.
