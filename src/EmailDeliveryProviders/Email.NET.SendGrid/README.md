# Email.NET.SendGrid - Email delivery provider [EDP]

send emails using SendGrid.

to get started first install
- **[Email.Net.SendGrid](https://www.nuget.org/packages/Email.Net.SendGrid/):** `Install-Package Email.Net.SendGrid`.  

if you're using Dependency Injection than install 
- **[Email.Net.SendGrid.DependencyInjection](https://www.nuget.org/packages/Email.Net.SendGrid.DependencyInjection/):** `Install-Package Email.Net.SendGrid.DependencyInjection`.  

##### Setup
in order to use the SendGrid EDP, you call the `UseSendGrid()` method and pass the api key and server id.

```csharp
// register SendGrid EDP with EmailServiceFactory
EmailServiceFactory.Instance
    .UseSendGrid(apiKey: "your-sendgrid-api-key")
    .Create();

// register SendGrid EDP with Dependency Injection
services.AddEmailNet(SendGridEmailDeliveryProvider.Name)
    .UseSendGrid(apiKey: "your-sendgrid-api-key");
```

##### Custom EDP data
SendGrid EDP allows you to pass the folowwing data with the message instance

```csharp
// create the message
var message = Message.Compose()

    // to pass tracking settings
    .UseTrackingSettings(new TrackingSettings
    {
        // set the tracking options
    })
    
    // to pass a defrent api key
    .UseCustomApiKey("you-custom-api-key");
```
