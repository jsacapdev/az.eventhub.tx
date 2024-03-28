param name string
param location string = resourceGroup().location
param tags object = {}

@secure()
param principalId string

@secure()
param vmPrincipalId string

@secure()
param applicationInsightsConnectionString string

param eventHubNamespaceName string 

param eventHubName string

resource eventHubProducerAuthorizationRules 'Microsoft.EventHub/namespaces/eventhubs/authorizationRules@2023-01-01-preview' existing = {
  name: '${eventHubNamespaceName}/${eventHubName}/Producer'
}

resource keyVault 'Microsoft.KeyVault/vaults@2022-07-01' = {
  name: name
  location: location
  tags: tags
  properties: {
    tenantId: subscription().tenantId
    sku: { family: 'A', name: 'standard' }
    accessPolicies: !empty(principalId) ? [
      {
        objectId: principalId
        permissions: { secrets: [ 'all' ] }
        tenantId: subscription().tenantId
      }
      {
        objectId: vmPrincipalId
        permissions: { secrets: [ 'get', 'list' ] }
        tenantId: subscription().tenantId
      }
    ] : []
  }
}

resource applicationInsightsConnectionSecret 'Microsoft.KeyVault/vaults/secrets@2023-07-01' = {
  parent: keyVault
  name: 'ApplicationInsightsConnectionString'
  properties: {
    value: applicationInsightsConnectionString
  }
}

resource eventHubConnectionStringSecret 'Microsoft.KeyVault/vaults/secrets@2023-07-01' = {
  parent: keyVault
  name: 'EventHubConnectionString'
  properties: {
    value: eventHubProducerAuthorizationRules.listKeys().primaryConnectionString
  }
}
resource eventHubNameSecret 'Microsoft.KeyVault/vaults/secrets@2023-07-01' = {
  parent: keyVault
  name: 'EventHubName'
  properties: {
    value: eventHubName
  }
}

output endpoint string = keyVault.properties.vaultUri
output name string = keyVault.name
