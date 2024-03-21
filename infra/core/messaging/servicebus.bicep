param serviceBusNamespaceName string

param serviceBusTopicName string

@description('Specifies the Azure location for all resources.')
param location string

param tags object = {}

resource serviceBusNamespace 'Microsoft.ServiceBus/namespaces@2022-01-01-preview' = {
  name: serviceBusNamespaceName
  tags: tags
  location: location
  sku: {
    name: 'Standard'
  }
  properties: {}
}

resource serviceBusTopic 'Microsoft.ServiceBus/namespaces/topics@2022-10-01-preview' = {
  name: serviceBusTopicName
  parent: serviceBusNamespace
}

resource  serviceBusTopicSubscription 'Microsoft.ServiceBus/namespaces/topics/subscriptions@2022-10-01-preview' = {
  name: 'sub-azd5-dev-001'
  parent: serviceBusTopic
}
