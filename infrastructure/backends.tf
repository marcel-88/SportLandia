
# Configure Azure remote backend
terraform {
  backend "azurerm" {
    resource_group_name  = "web-backend-marrosca"
    storage_account_name = "webprojectmarrosca"
    container_name       = "tfstate"
    key                  = "terraform.tfstate"
  }
}
