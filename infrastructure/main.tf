
# Create a resource group
resource "azurerm_resource_group" "rg" {
  name     = "web-project-marrosca"
  location = "germanywestcentral"

  tags = {
    environment = "production"
  }
}

# Create a virtual network within the resource group
resource "azurerm_virtual_network" "vnet" {
  name                = "web-project-marrosca"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  address_space       = ["10.0.0.0/24"]

  tags = {
    environment = "production"
  }
}

# Create the subnet for this vnet
resource "azurerm_subnet" "subnet" {
  name                 = "web-project-marrosca-subnet"
  resource_group_name  = azurerm_resource_group.rg.name
  virtual_network_name = azurerm_virtual_network.vnet.name
  address_prefixes     = ["10.0.0.0/28"]
}

resource "azurerm_public_ip" "ip" {
  name                = "web-project-marrosca-db-ip"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  allocation_method   = "Dynamic"
}

# Create the nic
resource "azurerm_network_interface" "nic" {
  name                = "web-project-marrosca-nic"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name

  ip_configuration {
    name                          = "internal"
    subnet_id                     = azurerm_subnet.subnet.id
    private_ip_address_allocation = "Dynamic"
    public_ip_address_id          = azurerm_public_ip.ip.id
  }
}

# Create the Linux VM
resource "azurerm_linux_virtual_machine" "vm" {
  name                = "web-project-marrosca-db"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  size                = "Standard_B2S"
  admin_username      = "azureuser"
  network_interface_ids = [
    azurerm_network_interface.nic.id,
  ]

  admin_ssh_key {
    username   = "azureuser"
    public_key = file("${path.root}/.ssh/db_id_rsa.pub")
  }

  os_disk {
    caching              = "ReadWrite"
    storage_account_type = "Standard_LRS"
  }

  source_image_reference {
    publisher = "Canonical"
    offer     = "0001-com-ubuntu-server-jammy"
    sku       = "22_04-lts"
    version   = "latest"
  }
}


# Create SQL Server
# resource "azurerm_mssql_server" "sql-server" {
#  name                         = "mssql-server-sportlandia"
#  resource_group_name          = azurerm_resource_group.rg.name
#  location                     = azurerm_resource_group.rg.location
#  version                      = "12.0"
#  administrator_login          = "Very_Muscular"
#  administrator_login_password = "@Very_Muscular$69"
#  minimum_tls_version          = "1.2"

#  tags = {
#    environment = "production"
#  }
#}

# Create the database
#resource "azurerm_mssql_database" "web-db" {
#  name         = "web-db"
#  server_id    = azurerm_mssql_server.sql-server.id
#  collation    = "Romanian_CS_AS_UTF8"
#  license_type = "BasePrice"
#  max_size_gb  = 4
#  sku_name     = "Basic"

#  tags = {
#    environment = "production"
#  }

# prevent the possibility of accidental data loss
#  lifecycle {
#    prevent_destroy = true
#  }
#}
