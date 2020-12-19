provider "azurerm" {
  features {
  }
}

resource "azurerm_resource_group" "codetest" {
  name     = var.resource_group.name
  location = var.resource_group.location
}

module "azure_webapp" {
  source = "./web-app"
  count  = length(var.webapps)

  resource_group = azurerm_resource_group.codetest

  webapp_name = var.webapps[count.index].name

  webapp_sku = {
    tier = "Free"
    size = "F1"
  }
}
