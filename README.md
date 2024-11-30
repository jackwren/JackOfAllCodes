## Getting Started
Use these instructions to get the project up and running.

### Prerequisites
You will need the following tools:

* [Visual Studio 2022](https://visualstudio.microsoft.com/downloads/)
* [.Net Core 8.0 or later](https://dotnet.microsoft.com/download/dotnet-core/2.2)
* [PGAdmin](https://www.pgadmin.org/)

### Installing
Follow these steps to get your development environment set up:
1. Clone the repository
2. At the root directory, restore required packages by running:
```csharp
dotnet restore
```
3. Next, build the solution by running:
```csharp
dotnet build
```
4. Next, within the AspnetRun.Web directory, launch the back end by running:
```csharp
dotnet run
```
5. Launch http://localhost:*YOURLOCALPORT*/ in your browser to view the Web UI.

If you have **Visual Studio** after cloning Open solution with your IDE, AspnetRun.Web should be the start-up project. Directly run this project on Visual Studio with **F5 or Ctrl+F5**. You will see index page of project, you can navigate product and category pages and you can perform crud operations on your browser.

### Usage
After cloning or downloading the sample you need to have installed and setup the postgresSQL solution provided. I will provide details on how to setup below:

## Postgres SQL setup:
## TO ADD

```json
"ConnectionStrings": {
  "BlogPostDbConnectionString": "Host=localhost;Port=5432;Database=BlogPost;Username=user;Password=password;"
}
```

1. Ensure your connection strings in ```appsettings.json``` point to a local SQL Server instance.

### Structure of Project
## TO ADD

## Disclaimer

* This repository is not intended to be a definitive solution.
* This repository not implemented a lot of 3rd party packages, we are try to avoid the over engineering when building on best practices.

## Contributing

Please read [Contributing.md]() for details on our code of conduct, and the process for submitting pull requests to us.

## Deployment - AspnetRun Online

This project is deployed via AWS.

## Authors

**JACK WREN**