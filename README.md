# Email.Net

[![](https://img.shields.io/github/license/YoussefSell/Email.Net)](https://github.com/YoussefSell/Email.Net/blob/master/LICENSE)
[![](https://img.shields.io/nuget/v/Email.Net)](https://www.nuget.org/packages/Email.Net/)
![Build](https://github.com/YoussefSell/Email.Net/actions/workflows/ci.yml/badge.svg)

Email.Net simplifies sending emails in .NET applications, providing a clean architecture with access to multiple email delivery channels.

## Quick setup

to get started install the package using the [NuGet](https://www.nuget.org/packages/Email.Net/) package manager `Install-Package Email.Net`.

## Getting started

when you will send an emails using Email.Net, there are three component that you will interact with:

- **EmailMessage**: the email message content to be sent.
- **EmailService**: the email service.
- **Channel**: the Email Delivery Channel.

you first compose your message, than you pass it to the email service, than service will send your message using an Channel.

### 1. EmailMessage

the message contain your email content, which includes the following

- **From:** the email address to be used as the sender.
- **To:** the email addresses to be used as the recipients.
- **ReplayTo:** the reply-to email addresses.
- **Bcc:** the blind carbon copy email addresses.
- **Cc:** the carbon copy email addresses.
- **Subject:** the email subject.
- **Charset:** the optional character set for your message.
- **PlainTextContent:** the email plain text content.
- **HtmlContent:** the email HTML content.
- **Attachments:** the list of attachments.
- **Priority:** the priority of this email message.
- **Headers:** custom headers to be sent with this email message.
- **ChannelData:** custom data to be passed to the Channel used for sending the email.

now let see how can we compose a message:

```csharp
var message = EmailMessage.Compose()
    .To("to@email.net")
    .WithSubject("test email")
    .WithPlainTextContent("this is a test email")
    .WithHtmlContent("<p>this is a test email</p>")
    .WithHighPriority()
    .Build();
```

on the `EmailMessage` class you will find a method called `Compose()`, this method will give you a fluent API to compose your email so use the `'.'` and intellisense to see all the available function to compose you message, once you're done, call `Build()` to create an instance of the `EmailMessage`.

now we have a message let's try to send it.

### 2- Channels [Email Delivery Channel]

Channels are what actually used to send the emails under the hood, when you install Email.Net you get a channel by default which is `SmtpEmailDeliveryChannel`, which uses SmtpClient to send emails.

we have also other Channels that you can use, but they exist in a separate packages:

- **[Email.Net.Socketlabs](https://www.nuget.org/packages/Email.Net.Socketlabs/):** to send emails using Socketlabs.
- **[Email.Net.SendGrid](https://www.nuget.org/packages/Email.Net.SendGrid/):** to send emails using SendGrid.
- **[Email.Net.MailKit](https://www.nuget.org/packages/Email.Net.MailKit/):** to send emails using MailKit.
- **[Email.Net.Mailgun](https://www.nuget.org/packages/Email.Net.Mailgun/):** to send emails using Mailgun.
- **[Email.Net.AmazonSES](https://www.nuget.org/packages/Email.Net.AmazonSES/):** to send emails using AmazonSES.

and we will be adding more in the future, but if you want to create your own Channel you can follow this [guide](#) and you will learn how to build one.

### 3- EmailService

the email service is what you will be interacting with to send emails, to create an instance of the service you can use the `EmailServiceFactory`

```csharp
var emailService = EmailServiceFactory.Instance
    .UseOptions(options =>
    {
        /*
         * if set to true we will not send any email,
         * great if we don't want to send emails while testing other functionalities
         */
        options.PauseSending = false;

        /* used to specify the default from to be used when sending the emails */
        options.DefaultFrom = new MailAddress("from@email.net");

        /* set the default Channel to be used for sending the emails */
        options.DefaultEmailDeliveryChannel = SmtpEmailDeliveryChannel.Name;
    })
    // register the Channels
    .UseSmtp(options => options.UseGmailSmtp("your-email@gmail.com", "password"))
    .Create();
```

The `EmailServiceFactory.Instance` provides a fluent API with three key methods:

- **UseOptions():** Configure the email service options.
- **UseChannel():** Register the Channels to be used for sending emails.
- **Create():** Build and return the EmailService instance.

starting with `UseOptions()` you can configure the `EmailService` options, there are three options:

- **PauseSending:** to pause the sending of emails, if set to true nothing will be sent.
- **DefaultFrom:** to set the default Sender email, so that you don't have to do it each time on the message, note that if you have specified a Sender email on the message this value will be ignored.
- **DefaultEmailDeliveryChannel:** to specify the default Channel that should be used to send the emails, because you can configure multiple Channels you should indicate which one you want to be used by default.

`UseChannel()` takes an instance of the Channel, like so: `UseChannel(new SmtpEmailDeliveryChannel(configuration))`, but you're not going to use this method, instead you will use the extension methods given to you by the Channels as we seen on the example above, the SMTP Channel has an extension method `UseSmtp()` that will allow you to register it.

finally `Create()` will simply create an instance of the `EmailService`.

you only need to create the email service once and reuse it in your app.

now you have an instance of the `EmailService` you can start sending emails.

```csharp
// get the email service
var emailService = EmailServiceFactory.Instance
    .UseOptions(options =>
    {
        options.PauseSending = false;
        options.DefaultFrom = new MailAddress("from@email.net");
        options.DefaultEmailDeliveryChannel = SmtpEmailDeliveryChannel.Name;
    })
    .UseSmtp(options => options.UseGmailSmtp("your-email@gmail.com", "password"))
    .Create();

// create the message
var message = EmailMessage.Compose()
    .To("to@email.net")
    .WithSubject("test email")
    .WithPlainTextContent("this is a test email")
    .WithHtmlContent("<p>this is a test email</p>")
    .WithHighPriority()
    .Build();

// send the message
var result = emailService.Send(message);
```

## working with Dependency Injection

to register Email.Net with DI all you have to do is call the `AddEmailNet` method on the Services collection like so:

```csharp
// add Email.Net configuration
services.AddEmailNet(options =>
{
    options.PauseSending = false;
    options.DefaultFrom = new MailAddress("from@email.net");
    options.DefaultEmailDeliveryChannel = SmtpEmailDeliveryChannel.Name;
})
.UseSmtp(options => options.UseGmailSmtp("your-email@gmail.com", "password"));
```

then you can inject the Email Service in your classes constructors using `IEmailService`

```csharp
public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IEmailService _emailService;

    public IndexModel(ILogger<IndexModel> logger, IEmailService emailService)
    {
        _logger = logger;
        _emailService = emailService;
    }

    public void OnGet()
    {
        /* compose the email message */
        var message = EmailMessage.Compose()
            .To("to@email.net")
            .WithPlainTextContent("this is a test email")
            .WithHtmlContent("<p>this is a test email</p>")
            .WithHighPriority()
            .Build();

        /* send the message, this will use the default Channel set in the option */
        var result = _emailService.Send(message);

        /* log the result */
        _logger.LogInformation("sent: {result}", result.IsSuccess);
    }
}
```

for full documentation check the [Wiki](https://github.com/YoussefSell/Email.Net/wiki) page.

## Samples

here are some samples of how you can integrate Email.Net with different app types:

- [Console app](https://github.com/YoussefSell/Email.Net/tree/master/samples/Email.Net.Samples.Console)
- [ASP Core app](https://github.com/YoussefSell/Email.Net/tree/master/samples/Email.Net.Samples.ASPCore)

## Blog posts

here you will find a list of blog posts explaining how to integrate Email.Net in your applications, also if you have written one let's add it here:

- [How to send emails with C# - the right way](https://youssefsellami.com/how-to-send-emails-with-csharp/)
