# Azure Event Hub Transmission

A proof of concept as to different ways to upload at scale to Event Hub.

## Description

Scratch space for implementations that upload data to Event Hub in batches. All implementations uses the [.NET Worker](https://learn.microsoft.com/en-us/dotnet/core/extensions/workers) scaffold.

The first [implementation](https://github.com/jsacapdev/az.eventhub.tx/tree/main/src/dotnet/Azd.RxTx.Processor) uses a event driven approach, where messages are downloaded from a Service Bus, and then uploaded to Event Hub. The approach can up the `MaxConcurrentCalls` as part of the `ServiceBusProcessorOptions`. Here we hack the approach so that for every 1 message downloaded from a Service Bus subscription, 10 threads are created that upload 99 messages per batch (1 x 10 x 99 = 990 messages). The per second upload will eventually throttle the Event Hub, but thats to be [expected](https://learn.microsoft.com/en-us/azure/event-hubs/compare-tiers#quotas) on a standard SKU.

The second [implementation](https://github.com/jsacapdev/az.eventhub.tx/tree/main/src/dotnet/Azd.RxTx.Processor.v2) uses a approach where getting messages is dependent on the interface implementation. The approach to processing messages uses an long running worker thread (long-running work in this context refers to a thread that's running for the lifetime of the application uploading messages in batch to Event Hub). Note that the approach uses a specific instruction to the `TaskFactory` that it will be long running. Thread pool threads are by default not designed for this; they should complete quickly so they can be returned to the thread pool to be re-used for something else. So, the approach provides the `TaskCreationOptions.LongRunning` instruction to do long-running blocking work. Create as many threads [as are required](https://github.com/jsacapdev/az.eventhub.tx/blob/c64b9d753a8f24c41df99d72c82fe2f1b28b1fb4/src/dotnet/Azd.RxTx.Processor.v2/Implementation/MessageProcessor.cs#L22) to meet the requirements of the deployment.

Other general comments are:

- Logging is enabled to disk (Serilog) and to Application Insights
- Supporting infrastructure is provisioned using Bicep
- Deployment is manual at the moment (publish, zip, move, unzip, etc) and targets an Azure VM (that was created manually outside of the Bicep)
- Worker is deployed as a Windows Service.
- Repository is currently hooked up to an Action (build, no deployment) and Sonar (see badges below)

## Build Status

|Name|Status|
|-|-|
|Build|[![Build Application](https://github.com/jsacapdev/az.eventhub.tx/actions/workflows/azure-dev.yml/badge.svg)](https://github.com/jsacapdev/az.eventhub.tx/actions/workflows/azure-dev.yml)|
|Quality Gate|[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=jsacapdev_az.eventhub.tx&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=jsacapdev_az.eventhub.tx)|
|Bugs|[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=jsacapdev_az.eventhub.tx&metric=bugs)](https://sonarcloud.io/summary/new_code?id=jsacapdev_az.eventhub.tx)|
|Code Smells|[![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=jsacapdev_az.eventhub.tx&metric=code_smells)](https://sonarcloud.io/summary/new_code?id=jsacapdev_az.eventhub.tx)|
|Coverage|[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=jsacapdev_az.eventhub.tx&metric=coverage)](https://sonarcloud.io/summary/new_code?id=jsacapdev_az.eventhub.tx)|
|Duplications (%)|[![Duplicated Lines (%)](https://sonarcloud.io/api/project_badges/measure?project=jsacapdev_az.eventhub.tx&metric=duplicated_lines_density)](https://sonarcloud.io/summary/new_code?id=jsacapdev_az.eventhub.tx)|

## Commands used during the implementation

### Provision Resources using the Developer CLI

`azd provision --no-state --no-prompt`

`azd up --no-prompt`

### Scaffold a new Project

`dotnet new worker -n Azd.Tx.Ingest.Processor --use-program-main`

### Run a worker on the command line

`Get-ChildItem Env:\`

`$env:ServiceBusConnectionString = ""`

`$env:EventHubConnectionString = ""`

`$env:EventHubName = "evh-azd5-dev-001"`

`$env:APPLICATIONINSIGHTS_CONNECTION_STRING = ""`

### Publish the Windows Service

`dotnet publish -o d:/Azd.RxTx.Processor/`

`sc.exe create "Azd RxTx Service" binpath="C:\Program Files\Azd.RxTx.Processor\Azd.RxTx.Processor.exe"`

`sc.exe delete "Azd RxTx Service"`
