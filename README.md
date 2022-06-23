## Entain Tech Task (DotNet)

[![build and test](https://github.com/lachis/Entain-DotNet/actions/workflows/build-and-test.yml/badge.svg?branch=master&event=push)](https://github.com/lachis/Entain-DotNet/actions/workflows/build-and-test.yml) 


### Directory Structure

- `.github`: Github actions directory for Github Actions / workflows 
- `API`: An ASP.NET Core implementation of the golang REST gateway, responsible forwarding requests onto Racing / Sports Service
- `Contracts`:  Defines that contracts that the solution depends on (repo, dbcontext interfaces, etc). In real life, this folder would grow and contain more contract projects
- `Proto`: Single directory housing the Proto files used by the rest of the solution
- `Racing`: Contains the projects directly related to the Racing service.
- `Racing/Infrastructure`: Concrete implementations of the `Contracts`
- `Racing/Server`: An ASP.NET Core gRPC implementation of the Racing Service
- `Racing/Tests`: Contains Unit and Integration Tests related to the Racing Service implementation
- `Sports`:  Contains the projects directly related to the Sports service. 
- `Sports/Infrastructure`: Concrete implementations of the `Contracts`
- `Sports/Server`: An ASP.NET Core gRPC implementatioon of the Sports Service
- `Sports/Tests`: Contains Unit and Integration Tests related to the Sports Service implementation

```
Entain-Dotnet/
├─ .github/
|  ├─ workflows
├─ API
├─ Contracts/
|  ├─ Intrastructure.Contracts
├─ Proto/
├─ Racing/
|  ├─ Infrastructure
|  ├─ Server
|  ├─ Tests
├─ Sports
|  ├─ Infrastructure
|  ├─ Server
|  ├─ Tests
├─ Entain-DotNet.sln
├─ README.md
```

### Getting Started 
1. Download & Install Visual Studio 2022 Community Edition
2. Download & Install the .NET 6.0 sdk/runtime 
3. Download & Install the .NET 7.0.0-preview.5.22303.8 sdk/runtime

### Building the solution
1. With Visual Studio 2022: Menu --> Build --> Build Solution
2. With dotnet CLI: dotnet build --configuration Debug

### Running the Tests
1. With Visual  Studio 2022: Menu --> Test --> Run All Tests
2. With dotnet CLI: dotnet test 

### Running the application services
1. With Visual Studio 2022: Configure multiple startup projects that includes the API, Racing and Sports Services and hit F5 (or Debug --> Start)
![image](https://user-images.githubusercontent.com/5248669/175417673-480afdc2-16c3-497b-8d2d-11d3fe806235.png)

### Make Requests eg.
Get Races with Filter / Order
```
curl -X "POST" "http://localhost:8000/v1/list-races"      -H 'Content-Type: application/json'      -d $'{
  "filter": {}, "order": {}
}'
```
Get a Single Event
```
curl -X "GET" "http://localhost:8000/v1/events/1"     
```
