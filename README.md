# Azure configuration for deployment

## Environment Variables

In Azure functions app, settings - environment, add all the variables which are in local .env file.

AZURE_BLOB_CONNECTION_STRING=blob connection string of storage account
SQL_SERVER_CONNECTION_STRING=Server=(localdb)\MSSQLLocalDB;Database=AIAddressBookDb;Trusted_Connection=True;TrustServerCertificate=True;

## Azure functions app identity

In Azure functions app, settings, identity, set system assigned to on. It will get object principal id from Azure and registered in Entra for authentication.

## Add Azure functions app user in SQL Server

Run the following commands in Azure SQL Server by connecting with SQL Management Studio.

The identity name is the Azure functions app name, not the principal id which is a guid.

CREATE USER [<identity -name>] FROM EXTERNAL PROVIDER;

ALTER ROLE db_datareader ADD MEMBER [<identity-name>];

ALTER ROLE db_datawriter ADD MEMBER [<identity-name>];

