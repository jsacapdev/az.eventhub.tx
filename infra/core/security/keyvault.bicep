param name string
param location string = resourceGroup().location
param tags object = {}

@secure()
param principalId string = ''

@secure()
param applicationInsightsConnectionString string


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
    ] : []
  }
}

resource sonarSecret 'Microsoft.KeyVault/vaults/secrets@2023-07-01' = {
  parent: keyVault
  name: 'applicationInsightsConnectionString'
  properties: {
    value: applicationInsightsConnectionString
  }
}

output endpoint string = keyVault.properties.vaultUri
output name string = keyVault.name
