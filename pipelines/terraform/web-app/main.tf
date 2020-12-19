resource "azurerm_app_service_plan" "codetest" {
  name                = "asp-${var.webapp_name}"
  location            = var.resource_group.location
  resource_group_name = var.resource_group.name

  sku {
    tier = var.webapp_sku.tier
    size = var.webapp_sku.size
  }

  kind     = "Linux"
  reserved = true
}

resource "azurerm_app_service" "codetest" {
  name                = var.webapp_name
  location            = var.resource_group.location
  resource_group_name = var.resource_group.name
  app_service_plan_id = azurerm_app_service_plan.codetest.id

  site_config {
    dotnet_framework_version = "v4.0"
    linux_fx_version         = "DOTNETCORE|3.1"
  }

  app_settings = var.webapp_settings
}
