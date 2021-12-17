# Email.Net.Mailgun - Email delivery provider [EDP]

send emails using Mailgun.

to get started first install
- **[Email.Net.Mailgun](https://www.nuget.org/packages/Email.Net.Mailgun/):** `Install-Package Email.Net.Mailgun`.  

if you're using Dependency Injection than install 
- **[Email.Net.Mailgun.DependencyInjection](https://www.nuget.org/packages/Email.Net.Mailgun.DependencyInjection/):** `Install-Package Email.Net.Mailgun.DependencyInjection`.  

##### Setup
in order to use the Mailgun EDP, you call the `UseMailgun()` method and pass the api key and server id.

```csharp
// register Mailgun EDP with EmailServiceFactory
EmailServiceFactory.Instance
    .UseMailgun(apiKey: "your-mailgun-api-key", domain: "your-mailgun-domain")
    .Create();

// register Mailgun EDP with Dependency Injection
services.AddEmailNet(MailgunEmailDeliveryProvider.Name)
    .UseMailgun(apiKey: "your-mailgun-api-key", domain: "your-mailgun-domain");
```

##### Custom EDP data
Mailgun EDP allows you to pass the folowwing data with the message instance

```csharp
// create the message
var message = Message.Compose()

    // to use the test mode
    .UseTestMode()
    
    // to enable tracking
    .UseEnableTracking();
```
