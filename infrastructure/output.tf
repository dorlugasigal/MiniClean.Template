output "resource_group_name" {
  value = azurerm_resource_group.rg.name
}

output "key_vault_name" {
  value = module.services.key_vault_name
}

output "current_object_id" {
  value = data.azurerm_client_config.current.object_id
}

output "fhir_url" {
  value = module.health-services.fhir_url
}

output "web_app_host" {
  value = module.services.web_app_host
}