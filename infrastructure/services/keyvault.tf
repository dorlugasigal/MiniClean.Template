resource "azurerm_key_vault" "kv" {
  name                = "kv-miniclean-${var.env}"
  location            = var.location
  resource_group_name = var.resource_group_name
  tenant_id           = var.tenant_id

  sku_name = "standard"
  network_acls {
    default_action = "Deny"
    bypass         = "AzureServices"
    ip_rules       = var.runner_ip
  }
}

resource "azurerm_key_vault_access_policy" "api_user_access" {
  key_vault_id = azurerm_key_vault.kv.id
  tenant_id    = var.tenant_id
  object_id    = azurerm_user_assigned_identity.identity.principal_id

  secret_permissions      = ["Get", "List", "Set", "Delete", "Recover"]
  certificate_permissions = ["Create", "Delete", "Get", "Import", "List", "Purge", "Update"]

  depends_on = [
    azurerm_key_vault_access_policy.terraform_user_access
  ]
}

resource "azurerm_key_vault_access_policy" "agw_user_access" {
  key_vault_id = azurerm_key_vault.kv.id
  tenant_id    = var.tenant_id
  object_id    = azurerm_user_assigned_identity.agw-identity.principal_id

  secret_permissions      = ["Get", "List", "Set", "Delete", "Recover"]
  certificate_permissions = ["Create", "Delete", "Get", "Import", "List", "Purge", "Update"]

  depends_on = [
    azurerm_key_vault_access_policy.terraform_user_access
  ]
}

resource "azurerm_key_vault_access_policy" "terraform_user_access" {
  key_vault_id            = azurerm_key_vault.kv.id
  tenant_id               = var.tenant_id
  object_id               = var.current_object_id
  storage_permissions     = ["Set", "List", "Get"]
  key_permissions         = ["Get", "List", "Update", "Create"]
  secret_permissions      = ["Get", "List", "Set", "Delete", "Purge", "Recover"]
  certificate_permissions = ["Create", "Delete", "Get", "Import", "List", "Purge", "Update"]
}

resource "azurerm_private_endpoint" "kv-pe" {
  name                = "pe-miniclean-${var.env}-kv"
  location            = var.location
  resource_group_name = var.resource_group_name
  subnet_id           = var.services_subnet_id

  private_dns_zone_group {
    name                 = "private-dns-zone-group"
    private_dns_zone_ids = [var.vault_zone_id]
  }

  private_service_connection {
    name                           = "psc-miniclean-${var.env}-kv"
    private_connection_resource_id = azurerm_key_vault.kv.id
    is_manual_connection           = false
    subresource_names              = ["Vault"]
  }
}

data "azurerm_key_vault" "common_kv" {
  name                = "kv-miniclean-common"
  resource_group_name = "dlg-miniclean"
}