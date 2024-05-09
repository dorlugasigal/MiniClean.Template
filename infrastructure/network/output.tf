output "core_vnet_id" {
  value = azurerm_virtual_network.core_vnet.id
}

output "services_subnet_id" {
  value = azurerm_subnet.services_subnet.id
}

output "app_plan_subnet_id" {
  value = azurerm_subnet.app_plan_subnet.id
}

output "acr_dns_zone_id" {
  value = azurerm_private_dns_zone.acr_dns_zone.id
}

output "app_service_dns_zone_id" {
  value = azurerm_private_dns_zone.app_service_dns_zone.id
}

output "kv_dns_zone_id" {
  value = azurerm_private_dns_zone.kv_dns_zone.id
}

output "storage_dns_zone_id" {
  value = azurerm_private_dns_zone.str_dns_zone.id
}

output "health_dns_zone_id" {
  value = azurerm_private_dns_zone.health_dns_zone.id
}