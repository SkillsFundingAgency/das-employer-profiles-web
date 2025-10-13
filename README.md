## ⛔Never push sensitive information such as client id's, secrets or keys into repositories including in the README file⛔

# das-employerprofiles-web

<img src="https://avatars.githubusercontent.com/u/9841374?s=200&v=4" align="right" alt="UK Government logo">

[![Build Status](https://sfa-gov-uk.visualstudio.com/Digital%20Apprenticeship%20Service/_apis/build/status%2Fdas-employer-profiles-web?repoName=SkillsFundingAgency%2Fdas-employer-profiles-web&branchName=main)](https://sfa-gov-uk.visualstudio.com/Digital%20Apprenticeship%20Service/_build/latest?definitionId=3100&repoName=SkillsFundingAgency%2Fdas-employer-profiles-web&branchName=main)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=SkillsFundingAgency_das-employer-profiles-web&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=SkillsFundingAgency_das-employer-profiles-web)
[![License](https://img.shields.io/badge/license-MIT-lightgrey.svg?longCache=true&style=flat-square)](https://en.wikipedia.org/wiki/MIT_License)

The `das-employerprofiles-web` project is used for storing profile information of users that access the Employer Apprenticeship Service. This is the first part of the
journey where the user will first enter the First Name, Last Name which creates a profile which is then linked to a gov login identity.

There is an option to run this project using GovLogin or to use the Stub Authentication instead. In all test environments the stub authentication is instead used. This
is the service that hosts the stub authentication page used by the employer apprenticeship service.

## How It Works

Employer Profiles Web is a .net core front end solution for User Account Management. It connects to the Employer Profiles
outer API in APIM. 

## 🚀 Installation

### Pre-Requisites

* A clone of this repository
* A code editor that supports .net 8.0
* Azurite
* APIM subscription key to employer profiles outer API
   
### Config

appSettings.json file
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "System": "Information",
      "Microsoft": "Information"
    }
  },
  "ConfigurationStorageConnectionString": "UseDevelopmentStorage=true;",
  "ConfigNames": "SFA.DAS.EmployerProfiles.Web,SFA.DAS.Employer.GovSignIn",
  "EnvironmentName": "LOCAL",
  "Version": "1.0",
  "APPINSIGHTS_INSTRUMENTATIONKEY": "",
  "AllowedHosts": "*",
  "cdn": {
    "url": "https://das-at-frnt-end.azureedge.net"
  },
  "ResourceEnvironmentName": "LOCAL",
  "StubAuth": "true"
}
```

Azure Table Storage config

Row Key: SFA.DAS.EmployerProfiles.Web_1.0

Partition Key: LOCAL

Data:

```json
{
  "EmployerProfilesWebConfiguration": {
    "DataProtectionKeysDatabase": "DefaultDatabase={{X}}",
    "BaseUrl": "https://{{GATEWAY_URL}}/employerprofiles/",
    "RedisConnectionString": "{{REDIS_CONNECTIONSTRING}}",
    "Key": "{{GATEWAY_KEY}}"
  }
}
```

## Technologies

* .net 9
* Azure Table Storage
* NUnit
* Moq
* FluentAssertions
* APIM
