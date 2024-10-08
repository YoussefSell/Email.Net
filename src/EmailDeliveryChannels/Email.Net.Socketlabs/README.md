# Email.Net.Socketlabs - Email delivery channel [Channel]

send emails using socketlabs.

to get started first install

- **[Email.Net.Socketlabs](https://www.nuget.org/packages/Email.Net.Socketlabs/):** `Install-Package Email.Net.Socketlabs`.

if you're using Dependency Injection than install

- **[Email.Net.Socketlabs.DependencyInjection](https://www.nuget.org/packages/Email.Net.Socketlabs.DependencyInjection/):** `Install-Package Email.Net.Socketlabs.DependencyInjection`.

##### Setup

in order to use the Socketlabs Channel, you call the `UseSocketlabs()` method and pass the api key and server id.

```csharp
// register Socketlabs Channel with EmailServiceFactory
EmailServiceFactory.Instance
    .UseSocketlabs(apiKey: "your-socketlabs-api-key", serverId: 15478)
    .Create();

// register Socketlabs Channel with Dependency Injection
services.AddEmailNet(SocketLabsEmailDeliveryChannel.Name)
    .UseSocketlabs(apiKey: "your-socketlabs-api-key", serverId: 15478);
```

##### Custom Channel data

Socketlabs Channel allows you to pass the folowwing data with the message instance

```csharp
// create the message
var message = Message.Compose()

    // to pass a custom message id
    .SetMessageId("your-message-id")

    // to pass a custom mailing id
    .SetMailingId("your-mailing-Id")

    // to pass a defrent api key
    .UseCustomApiKey("you-custom-api-key")

    // to pass a defrent server id
    .UseCustomServerId(15874);
```
