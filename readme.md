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

## Command Scratch Space

`azd provision --no-state --no-prompt`

`azd up --no-prompt`

`dotnet new worker -n Azd.Tx.Ingest.Processor --use-program-main`
