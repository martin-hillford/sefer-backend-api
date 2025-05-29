# Sefer.Backend.Api
This is the api in the Sefer Framework that contains all the information related to user and education.
It is the main api and required for the framework to function

## Building
To build the code, please ensure dotnet 8 or later in installed. So the build.sh script in the root of repository
to build the code. Alternatively the Dockerfile can be used.

## Installation
Sefer is a (micro) service framework, for the more in-dept information on installation, please see the 
[docs repository](https://github.com/martin-hillford/sefer-docs). 

## Configuration
This section provides the necessary information on the configurations required for the service to operate.

### Cors
This API supports sending the appropriate CORS headers when configured correctly. **AllowedOrigins** is a 
comma-separated list of origins, while the **MaxAge** setting specifies the duration, in seconds, that the 
browser is permitted to cache the CORS headers.

```json
{
    "Cors": {
        "AllowedOrigins": "localhost:3000;localhost:8000;vscode.dev",
        "MaxAge": "3600"
    }
}
```

### UserSettings
This service supports additional configurable user settings, which are stored as key/value pairs for each user. 
Any front-end that consumes this service is responsible for handling these settings appropriately. 
Only the keys defined in this list will be saved in the database.

```json
{
  "UserSettings": [
    "preferredTextSize",
    "useDyslexiaFont"
  ]
}
```

### File storage
To accommodate different file types, such as images and videos, for inclusion in lessons and on the content page,
file storage is available. Currently, two methods are supported: (local) FileSystem and Azure.

To configure the storage method, a section labeled 'Storage' should be included. The property 'Method' can have the
following values: 'Azure' for storage on Azure, or 'FileSystem' for storage on a local file system, respectively.

Files can be stored in two directories: "public" for files that are accessible on the internet, and "private" for files
that are only available to users who are logged into the application.

#### FileSystem
If files are stored on the local file system, they must be accessible for reading and writing in a standard directory.

The following configuration values should be set in the 'Paths':

| Key         | Meaning                                                                                       |
|-------------|-----------------------------------------------------------------------------------------------|
| PublicPath  | The path where public files can be found.                                                     |
| PrivatePath | The path where private files can be found.                                                    |
| Endpoint    | The endpoint (excluding the private or public paths) where the API will serve the files from. |

Please Note: the Endpoint is typically, is just the base URI of the entire API. When using FileSystem storage, 
the API is responsible for serving the files and will utilize cookies.

This results in the following configuration:

```json
{
  "Storage" : {
    "Method" : "FileSystem",
    "Paths" : {
      "PublicPath" : "/data/sefer/public",
      "PrivatePath" : "/data/sefer/private",
      "Endpoint" : "https://sefer.tld"
    }
  }
}
```

#### Azure
If files are stored on the Azure, they must be accessible for reading and writing using sas url. 

The following configuration values should be set in the 'Paths':

| Key        | Meaning                                                                                   |
|------------|-------------------------------------------------------------------------------------------|
| PublicSas  | A sas url that can be used to read and write to an Azure blob container for public files  |
| PrivateSas | A sas url that can be used to read and write to an Azure blob container for private files |
| PublicUrl  | The url for the public files. Can be the Azure blob url itself with out the access codes  |
| PrivateUrl | The url for the private files. Typical the api url it self.                               |

When using blob storage, the PrivateUrl must be configured to prevent accidental exposure of the files.
To facilitate this prevention, the service is also capable of serving private files through an endpoint.

This results in the following configuration:

```json
{
  "Storage" : {
    "Method" : "FileSystem",
    "Blob" : {
      "PublicSas" : "https://sefer.blob.core.windows.net/public?access=read+write",
      "PrivateSas" : "https://sefer.blob.core.windows.net/private?access=read+write",
      "PublicUrl" : "https://sefer.blob.core.windows.net/public",
      "PrivateUrl" : "https://sefer.tld"
    }
  }
}
```

### GeoIP
This API is a consumer of GeoIP information. Therefor a GeoIP section must be added that define the ApiKey and the url 
that can be used to contact the GeoIP service. 

```json
{
  "GeoIP": {
    "Service": "http://sefer-geoip.api",
    "ApiKey": "04a77b56-b2c2-4e9a-b7b9-a26d22519058"
  }
}
```

### Avatar
This API is a consumer of Avatar information. Therefor an Avatar section must be added that define the ApiKey and the url
that can be used to contact the Avatar service.

```json
{
  "GeoIP": {
    "Service": "http://sefer-avatar.api",
    "ApiKey": "04a77b56-b2c2-4e9a-b7b9-a26d22519058"
  }
}
```

### PDF
This API is a consumer of PDF generation. Therefor a PDF section must be added that define the ApiKey and the url
that can be used to contact the PDF service.

```json
{
  "GeoIP": {
    "Service": "http://sefer-pdf.api",
    "ApiKey": "04a77b56-b2c2-4e9a-b7b9-a26d22519058"
  }
}
```