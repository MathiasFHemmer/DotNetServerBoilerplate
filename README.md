# DotNet Api Boilerplate

⚠️THIS IS STILL IN DEVELOPMENT!⚠️   

This project contains a bare minimum of a typical dotnet api server. It includes:

- 🌐 Web API ready to start populating with requests using OpenAPI specifications!
- 🧹 Clean Project-Driven architecture with well defined behavior boundaries!   
- 🧪 Test-Ready project for common patterns (Unit, Integration)!
- 🚢 Docker configuration ready-to-use that exposes the Api!
- 🔧 Flexible and easy code base, planned for modification and extension!

## Design Architecture

This project was designed on a 3 Layer Architecture:

1. Action Triggers
2. Data Transformation
3. Information Distribution

Each layer deals with information exchange in a different way, but they all exists to solve a single problem: How to mutate data? An application can only exist to receive instructions on data to process, gather said data in some way, process it, and provide said data. So in a very simple way: Data must come in, data must be processed somehow, and data must be available in the end.

| Project | Goal |
| ------- | ---- |
| MHemmer.Boilerplate.Api | Provides the Domain with "Actions" that can be used to interact with the Domain models to run data transformations. This defaults to a WebApi in this boilerplate, but can be extended in any other way. |
| MHemmer.Boilerplate.Domain | Provides a developer friendly set of tools to run business operations, while hiding away the complexity of algorithms and other computer nuances/abstractions! |
| MHemmer.Boilerplate.Infra | Provides the with a way to interact with the outside world, by exposing developer friendly interactions with external services and data providers. |
| MHemmer.Boilerplate.Common | This is a shared project that contains commonly used logic/data that is not related to business, and most likely are just design choices or internal structures. |

### Action Triggers

This is how outside users can interact with your application. A web api can expose endpoints for users to call, and react in some way by binding this endpoint to Domain code execution. This is the most common way of letting consumers use an application, but its not the only one! You can also create custom listeners using IHostedService!

In a perfect universe, information flow could be instant, and whatever data needed to be processed could be sent alongside the action trigger. But as it is not, we should consider what is the most cost efficient way of running the action with the least amount of information flow, to minimize network latency. For example, to run a simple "Create Person" command, the data necessary to create the user can be sent alongside the request frame of the action trigger. But if the user wants to create hundreds of persons, a different strategy should be applied, considering the limitations of the real world. A better approach would be to design a common data import format, and provide a way of getting said data in the "Information Distribution" layer, this could be done by an Action Trigger requesting a data import action, uploading the necessary data, and later on querying this data to generate the users. This follows the design principle of this architecture: Data is Key, and it is Heavy!

### Data Transformation

This is the "Domain" layer of your business. It is how your application uses data and information to extract knowledge. This layer goal is to process the most amount of information it can, without worrying on how it got there in the first place. Much like a market does not care exactly how goods arrive on a specific building, but it does care on how to arrange and sell those goods.
But be aware the it still needs to know its data, to properly provide interaction points where developers can hook Action Triggers! 

### Information Distribution

The application does not own the data. It just knows how to properly interpret and manipulate it inside its rules. Data itself can be stored in many different ways, and that is the job of the Information Distribution layer. It abstracts away the problem of "How do I persist my data with the minimal footprint possible, while making it available for my Domain?".

We typically see database providers inside this layer, setting up connections to database services and abstracting data transformation on the way. If you think about it, all it does is connect to a external service, send an Action Trigger to it to fetch information, and receive it, mapping back to the domain model. This is exactly what ties the layer to the whole application: It knows to to make arbitrary data into the shape of the model! The data can come from Postgres as the main storage, a Redis when cache is needed, another web service, or any integration you might need!

This is not the only way of fetching information. Other services can expose information through other interfaces, such as HTTP, gRPC, MCP... All those applications provides ours with information to process. Its here that we can implement those requests, and make sure they map correctly to our Domain models!

## Technology Standards:

- 📖 OpenAPI: Used to describing Action Triggers and its constrains. 
- 🔍 OpenTelemetry: Used to expose Logs, Metrics and Traces in a industry standard way.
    - Otel Instrumentation is enabled by default on the WebApi
- 🔬 XUnit: Used for testing all parts of the application
- 🗄️ EntityFramework: Used to abstract away SQL providers

# Running

No configuration is needed to customize the application run in any way.

## Template

Run the following command to install this template as a system wide template project that will be listed in the `dotnet new` command:
- `dotnet new -i . --force`

## Local

`dotnet run ./src/MHemmer.Boilerplate.Api/MHemmer.Boilerplate.Api.csproj`

## Docker

`docker compose up`