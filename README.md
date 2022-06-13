# Introduction

The project is the back-end program of Vincall's Admin Panel, using .NET core 3.1 technology, providing Vincall's agent, report, voice, and other standard API interfaces.

# Initializing
Clone VincallService repository on your computer, install dependencies using:
```
git clone xxx
```
Then if you want to run the project, go to the directory VincallService.
```
dotnet run 
```

The configuration items of the project are as follows and can be modified according to the actual situation.
```json
{
  "ConnectionStrings": {
    "Vincall": "Server=192.168.2.215;User=sa;Password=Aa00000000;Database=vincall"
  },
  "InitDB": "false",
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },   
  "OauthUri": "https://oauth.vincall.net",  
  "TokenPrivateKey": "MIIFHDBOBgkqhkiG9w0***=="

}
```
- ConnectionStrings: The connection string to Microsoft SQL server database.
- InitDB: Whether to recreate the database table structure and initialize the population of data. If the data table structure has already been initialized, it will not be modified again.
- Logging: The definition and type of the log output are changed
- OauthUri: Vincall's OAuth 2 service address.
- TokenPrivateKey: Private key ciphertext required for token issuance.

If you are initializing for the first time, you will need to prepare the MS SQL Server environment. Once you have the database environment ready, you can change the ConnectionString Authdb key and InitDB key.

```
 "ConnectionStrings": {
    "AuthDB": "Server=[your ip];User=[your username];Password=[your passwor];Database=vincall"
  },
  "InitDB": "true",
```
Then to run the program.

```
dotnet run
```
Finally, Let's check database when opening Microsoft SQL server management studio.

![image.png](/.attachments/image-4637c1e5-e09f-4796-b270-607d5b10154c.png)

That is ok.
