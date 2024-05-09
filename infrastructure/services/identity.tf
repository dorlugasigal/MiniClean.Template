resource "azurerm_user_assigned_identity" "identity" {
  resource_group_name = var.resource_group_name
  location            = var.location
  name                = "id-miniclean-${var.env}"
}

resource "azurerm_user_assigned_identity" "agw-identity" {
  resource_group_name = var.resource_group_name
  location            = var.location
  name                = "id-miniclean-${var.env}-agw"
}
