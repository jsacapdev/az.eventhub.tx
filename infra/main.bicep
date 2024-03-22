targetScope = 'resourceGroup'

@minLength(1)
@maxLength(10)
@description('Name of the environment that can be used as part of naming resource convention')
param environmentName string

@minLength(1)
@maxLength(64)
@description('Name of the product that can be used as part of naming resource convention')
param productName string

@minLength(1)
@description('Primary location for all resources')
param location string

@secure()
@description('Id of the user or app to assign application roles')
param principalId string

var abbrs = loadJsonContent('./abbreviations.json')

// Tags that should be applied to all resources.
var tags = {
  'azd-env-name': environmentName
}

var keyVaultName = '${abbrs.keyVaultVaults}${productName}-${environmentName}-001'

var logAnalyticsName = '${abbrs.operationalInsightsWorkspaces}${productName}-${environmentName}-001'
var applicationInsightsName = '${abbrs.insightsComponents}${productName}-${environmentName}-001'
var applicationInsightsDashboardName = '${abbrs.portalDashboards}${productName}-${environmentName}-001'

var eventHubNamespaceName = '${abbrs.eventHubNamespaces}${productName}-${environmentName}-001'
var eventHubName = '${abbrs.eventHubNamespacesEventHubs}${productName}-${environmentName}-001'

var serviceBusNamespaceName = '${abbrs.serviceBusNamespaces}${productName}-${environmentName}-001'
var serviceBusNamespaceTopicName = '${abbrs.serviceBusNamespacesTopics}${productName}-${environmentName}-001'

module monitoring './core/monitor/monitoring.bicep' = {
  name: 'monitoring'
  params: {
    logAnalyticsName: logAnalyticsName
    location: location
    tags: tags
    applicationInsightsName: applicationInsightsName
    applicationInsightsDashboardName: applicationInsightsDashboardName
  }
}

module keyVault './core/security/keyvault.bicep' = {
  name: 'keyvault'
  params: {
    name: keyVaultName
    location: location
    tags: tags
    principalId: principalId
    applicationInsightsConnectionString: monitoring.outputs.applicationInsightsConnectionString
  }
}

module servicebus './core/messaging/servicebus.bicep' = {
  name: 'servicebus'
  params: {
    location: location
    tags: tags
    serviceBusNamespaceName: serviceBusNamespaceName
    serviceBusTopicName: serviceBusNamespaceTopicName
  }
}

module eventhub './core/messaging/eventhub.bicep' = {
  name: 'eventhub'
  params: {
    location: location
    tags: tags
    eventHubNamespaceName: eventHubNamespaceName
    eventHubName: eventHubName
  }
}

output AZURE_LOCATION string = location
output AZURE_TENANT_ID string = tenant().tenantId
