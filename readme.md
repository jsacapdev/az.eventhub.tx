# Azure Event Hub Transmission

A proof of concept as to different ways to upload at scale to Event Hub.

## Build Status

|Name|Status|
|-|-|
|Build|[![Build Application](https://github.com/jsacapdev/az.eventhub.tx/actions/workflows/azure-dev.yml/badge.svg)](https://github.com/jsacapdev/az.eventhub.tx/actions/workflows/azure-dev.yml)|
|Quality Gate|[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=jsacapdev_az.eventhub.tx&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=jsacapdev_az.eventhub.tx)|
|Bugs|[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=jsacapdev_az.eventhub.tx&metric=bugs)](https://sonarcloud.io/summary/new_code?id=jsacapdev_az.eventhub.tx)|
|Code Smells|[![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=jsacapdev_az.eventhub.tx&metric=code_smells)](https://sonarcloud.io/summary/new_code?id=jsacapdev_az.eventhub.tx)|
|Coverage|[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=jsacapdev_az.eventhub.tx&metric=coverage)](https://sonarcloud.io/summary/new_code?id=jsacapdev_az.eventhub.tx)|
|Duplications (%)|[![Duplicated Lines (%)](https://sonarcloud.io/api/project_badges/measure?project=jsacapdev_az.eventhub.tx&metric=duplicated_lines_density)](https://sonarcloud.io/summary/new_code?id=jsacapdev_az.eventhub.tx)|

## Provision Resources using the Developer CLI

`azd provision --no-state --no-prompt`

`azd up --no-prompt`

## Scaffold a new Project

`dotnet new worker -n Azd.Tx.Ingest.Processor --use-program-main`

## Run a worker on the command line

`Get-ChildItem Env:\`

`$env:ServiceBusConnectionString = ""`

`$env:EventHubConnectionString = ""`

`$env:EventHubName = "evh-azd5-dev-001"`

`$env:APPLICATIONINSIGHTS_CONNECTION_STRING = ""`

## Publish the Windows Service

`dotnet publish -o d:/Azd.RxTx.Processor/`

`sc.exe create "Azd RxTx Service" binpath="C:\Program Files\Azd.RxTx.Processor\Azd.RxTx.Processor.exe"`

`sc.exe delete "Azd RxTx Service"`
