<a href="https://cognite.com/">
    <img src="./cognite_logo.png" alt="Cognite logo" title="Cognite" align="right" height="80" />
</a>

# CogniteSdk for .NET

![Build and Test](https://github.com/cognitedata/cognite-sdk-dotnet/workflows/Build%20and%20Test/badge.svg)
[![codecov](https://codecov.io/gh/cognitedata/cognite-sdk-dotnet/branch/master/graph/badge.svg?token=da8aPB6l9U)](https://codecov.io/gh/cognitedata/cognite-sdk-dotnet)
[![Nuget](https://img.shields.io/nuget/vpre/CogniteSdk)](https://www.nuget.org/packages/CogniteSdk/)

CogniteSdk for .NET is a cross platform asynchronous SDK for accessing the [Cognite Data Fusion](https://docs.cognite.com/) [API (v1)](https://docs.cognite.com/api/v1/) using [.NET Standard 2.0](https://docs.microsoft.com/en-us/dotnet/standard/net-standard) that works for all .NET implementations i.e both [.NET Core](https://en.wikipedia.org/wiki/.NET_Core) and [.NET Framework](https://en.wikipedia.org/wiki/.NET_Framework).

**Unofficial**: please note that this is an unofficial and community
driven SDK. Feel free to open issues, or provide PRs if you want to
improve the library.

The SDK may be used from both C# and F#.

- **C# SDK**: The C# SDK is a fluent API using objects and method chaining. Errors will be raised as exceptions. The API is asynchronous and all API methods returns `Task` and is awaitable using `async/await`.

- **F# SDK**: The F# API is written using plain asynchronous functions returning `Task` built on top of the [Oryx](https://github.com/cognitedata/oryx) HTTP handler library.

## Supported Resources

- [Assets](https://docs.cognite.com/api/v1/#tag/Assets)
- [TimeSeries & DataPoints](https://docs.cognite.com/api/v1/#tag/Time-series)
- [Events](https://docs.cognite.com/api/v1/#tag/Events)
- [Files](https://docs.cognite.com/api/v1/#tag/Files)
- [Login](https://docs.cognite.com/api/v1/#tag/Login) (partial)
- [Raw](https://docs.cognite.com/api/v1/#tag/Raw)
- [Sequences](https://docs.cognite.com/api/v1/#tag/Sequences)
- [Relationships](https://docs.cognite.com/api/v1/#tag/Relationships)
- [3D Models](https://docs.cognite.com/api/v1/#tag/3D-Models)
- [3D Files](https://docs.cognite.com/api/v1/#tag/3D-Files)
- [3D Asset Mapping](https://docs.cognite.com/api/v1/#tag/3D-Asset-Mapping)
- [Data sets](https://docs.cognite.com/api/v1/#tag/Data-sets)

## Documentation
* SDK Documentation. TBW.
* [API Documentation](https://doc.cognitedata.com/)
* [API Guide](https://doc.cognitedata.com/guides/api-guide.html)

## Installation

CogniteSdk is available as a [NuGet package](https://www.nuget.org/packages/CogniteSdk/). To install:

Using Package Manager:
```sh
Install-Package CogniteSdk
```

Using .NET CLI:
```sh
dotnet add package CogniteSdk
```

Or [directly in Visual Studio](https://docs.microsoft.com/en-us/nuget/quickstart/install-and-use-a-package-in-visual-studio).

## Quickstart

The SDK supports authentication through api-keys. The best way to use the SDK is by setting authentication values to environment values:

Using Windows Commands:
```cmd
setx PROJECT=myprojet
setx API_KEY=mysecretkey
```

Using Shell:
```sh
export PROJECT=myprojet
export API_KEY=mysecretkey
```

All SDK methods are called with a `Client` object. A valid client requires:
- `API Key` - key used for authentication with CDF.
- `Project Name` - the name of your CDF project e.g `publicdata`.
- `App ID` - an identifier for your application. It is a free text string. Example: `asset-hierarchy-extractor`
- `HTTP Client` - The [HttpClient](https://docs.microsoft.com/en-us/dotnet/api/system.net.http.httpclient?view=netcore-3.1) that will be used for the remote connection. Having this separate from the SDK have many benefits like using e.g [Polly](https://github.com/App-vNext/Polly) for policy handling.

```c#
using CogniteSdk;

var apiKey = Environment.GetEnvironmentVariable("API_KEY");
var project = Environment.GetEnvironmentVariable("PROJECT");

using var httpClient = new HttpClient(handler);
var builder = new Client.Builder();
var client =
    builder
        .SetAppId("playground")
        .SetHttpClient(httpClient)
        .SetApiKey(apiKey)
        .SetProject(project)
        .Build();

// your logic using the client
var query = new Assets.AssetQuery
{
    Filter = new Assets.AssetFilter { Name = assetName }
};
var result = await client.Assets.ListAsync(query);
```

## Examples

There are examples for both C# and F# in the Playground folder. To play with the example code, you need to set the CDF project and API key as environment variables.

## Developing

### Dotnet Tools 

A dotnet tools manifest is used to version tools used by this repo.  Install these tools with:

```sh
> dotnet tool restore
```

This will install Paket locally which is used for dependency management.

### Dependencies

Dependencies for all projects are handled using [Paket](https://fsprojects.github.io/Paket/). To install dependencies:

```sh
> dotnet paket install
```

This will install the main dependencies and sub-dependencies. The main dependencies are:

- [Oryx](https://www.nuget.org/packages/Oryx/) - HTTP Handlers.
- [Oryx.Cognite](https://www.nuget.org/packages/Oryx.Cognite/) - Oryx HTTP Handlers for Cognite API.
- [Oryx.SystemTextJson](https://www.nuget.org/packages/Oryx.SystemTextJson/) - JSON handlers for Oryx
- [Oryx.Protobuf](https://www.nuget.org/packages/Oryx.Protobuf/) - Protobuf handlers for Oryx
- [CogniteSdk.Protobuf](https://www.nuget.org/packages/CogniteSdk.Protobuf/) - Protobuf definitions for Cognite API.
- [System.Text.Json](https://www.nuget.org/packages/System.Text.Json/) - for Json support.
- [Google.Protobuf](https://www.nuget.org/packages/Google.Protobuf) - for Protobuf support.

### Running tests locally
```sh
sh ./test.sh
```
For this script AAD env variables need to be defined: `TEST_TENANT_ID_WRITE`, `TEST_CLIENT_ID_WRITE`, `TEST_CLIENT_SECRET_WRITE`.

You also need read credentials for publicdata project `TEST_TENANT_ID_READ`, `TEST_CLIENT_ID_READ`, `TEST_CLIENT_SECRET_READ`.

# Code of Conduct

This project follows https://www.contributor-covenant.org, see our [Code of Conduct](https://github.com/cognitedata/cognite-sdk-dotnet/blob/master/CODE_OF_CONDUCT.md).

## License

Apache v2, see [LICENSE](https://github.com/cognitedata/cognite-sdk-dotnet/blob/master/LICENSE).
