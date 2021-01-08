# Learner Module - Learner Microservice

There are some technical highlights for this project:

- The system deployment mode is Out of Process and Kestrel server so please don't get supprised when you don't see ```<AspNetCoreHostingModel>InProcess</AspNetCoreHostingModel>``` in the csproj.
- And that's why we also removed the block of setting for IIS Express:

```csharp
  "iisSettings": {
    "windowsAuthentication": false,
    "anonymousAuthentication": true,
    "iisExpress": {
      "applicationUrl": "http://localhost:61739/",
      "sslPort": 0
    }
  },
  "IIS Express": {
    "commandName": "IISExpress",
    "launchBrowser": true,
    "launchUrl": "swagger",
    "environmentVariables": {
      "ASPNETCORE_ENVIRONMENT": "Development"
    }
  },
```
- For this project, we use single project solution and separate concerns into folder instead of making separated project.
- The DLL name contains full information: Conexus.Opal.Microservice.Learner.Api
- The default namespace will be in short version: Microservice.Learner
- There is no ".Api" because we are using single project template.
- The name space should be 2 levels only: Application.Commands is enough, avoid Application.Command.Abc
