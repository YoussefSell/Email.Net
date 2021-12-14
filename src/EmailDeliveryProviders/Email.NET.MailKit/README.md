# Email.NET.MailKit - Email delivery provider [EDP]

send emails using MailKit.

to get started first install
- **[Email.Net.MailKit](https://www.nuget.org/packages/Email.Net.MailKit/):** `Install-Package Email.Net.MailKit`.  

if you're using Dependency Injection than install 
- **[Email.Net.MailKit.DependencyInjection](https://www.nuget.org/packages/Email.Net.MailKit.DependencyInjection/):** `Install-Package Email.Net.MailKit.DependencyInjection`.  


##### Setup
in order to use the MailKit EDP, you call the `UseMailKit()` method and pass the required options.

```csharp
// register MailKit EDP with EmailServiceFactory
EmailServiceFactory.Instance
    .UseMailKit(options => {
       option.SmtpOptions = new SmtpOptions
       {
            Port = 587,
            EnableSsl = true,
            Host = "smtp.gmail.com",
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential("your@gmail.com", "password"),
       }
    })
    .Create();

// register MailKit EDP with Dependency Injection
services.AddEmailNet(options => options.DefaultEmailDeliveryProvider = SmtpEmailDeliveryProvider.Name)
    .UseMailKit(options => {
       option.SmtpOptions = new SmtpOptions
       {
            Port = 587,
            EnableSsl = true,
            Host = "smtp.gmail.com",
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential("your@gmail.com", "password"),
       }
    });
```
Note: there are pre-defined SMTP configurations that you can use for a quick setup

- `UseGmailSmtp()`: set the SMTP options to use Gmail SMTP server for sending the emails.
- `UseOutlookSmtp()`: set the SMTP options to use Outlook SMTP server for sending the emails.

```csharp
// get the email service
EmailServiceFactory.Instance
    // use Gmail SMTP
    .UseMailKit(options => options.UseGmailSmtp("your-email@gmail.com", "password"))
    // or to use Outlook SMTP
    .UseMailKit(options => options.UseOutlookSmtp("your-email@outlook.com", "password"))
    .Create();
```

##### Custom EDP data
what if you want to send the emails using different SMTP options without changing the default configuration, for example using your customer SMTP configuration, you can achieve that using EDP data in the `Message` object, by using the [PassEdpData](https://github.com/YoussefSell/Email.Net/wiki/Message#passedpdata) method.

```csharp
// create the message
var message = Message.Compose()
    
    // pass custom smtp options
    .UseCustomSmtpOptions(new SmtpOptions
    {
        Port = 25,
        EnableSsl = true,
        Host = "client.smtp.server.com",
        UseDefaultCredentials = false,
        Credentials = new NetworkCredential("username", "password"),
     });
```
