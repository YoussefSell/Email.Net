# Email.NET.AmazonSES - Email delivery provider [EDP]

send emails using AmazonSES.

to get started first install
- **[Email.Net.AmazonSES](https://www.nuget.org/packages/Email.Net.AmazonSES/):** `Install-Package Email.Net.AmazonSES`.  

if you're using Dependency Injection than install 
- **[Email.Net.AmazonSES.DependencyInjection](https://www.nuget.org/packages/Email.Net.AmazonSES.DependencyInjection/):** `Install-Package Email.Net.AmazonSES.DependencyInjection`.  

##### Setup
in order to use the AmazonSES EDP, you call the `UseAmazonSES()` method and pass the api key and server id.

```csharp
// register AmazonSES EDP with EmailServiceFactory
EmailServiceFactory.Instance
    .UseAmazonSES()
    .Create();

// register AmazonSES EDP with Dependency Injection
services.AddEmailNet(AmazonSESEmailDeliveryProvider.Name)
    .UseAmazonSES();
```
