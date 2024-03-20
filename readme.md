# Azure Event Hub Transmission

A proof of concept as to different ways to upload at scale to Event Hub.

## Command Scratch Space

`azd provision --no-state --no-prompt`

`azd up --no-prompt`

`dotnet new worker -n Azd.Tx.Ingest.Processor --use-program-main`
