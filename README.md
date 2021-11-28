# Email.NET

Send emails from your .Net application with a flexible solution that guarantee clean architectures, and access to different types of providers.

## Quick setup

to get started install the package using the [NuGet](https://www.nuget.org/packages/Email.NET/) package manager `Install-Package Email.Net`.

## Getting started

when you will send an email, there are three component that you will interact with:

- **Message**: the email message content to be sent.
- **EmailService**: the email service.
- **Edp**: the Email Delivery Provider.

you first compose your message, than you pass the message to the email service, than service will send the message using an EDP.

### 1. Message

the message contain your email content, which includes the following

- **From:** the email address to be used as the sender.
- **To:** the email addresses to be used as the recipients.
- **ReplayTo:** the replay-to email addresses.
- **Bcc:** the blind carbon copy email addresses.
- **Cc:** the carbon copy email addresses.
- **Subject:** the email subject.
- **Charset:** the optional character set for your message.
- **PlainTextContent:** the email plain text content.
- **HtmlContent:** the email HTML content.
- **Attachments:** the list of attachments.
- **Priority:** the priority of this email message.
- **Headers:** custom headers to be sent with this email message.
- **EdpData:** custom data to be passed to the EDP used for sending the email.

now let see how can we compose a message:

```csharp
var message = Message.Compose()
    .To("to@email.net")
    .WithSubject("test email")
    .WithPlainTextContent("this is a test email")
    .WithHtmlContent("<p>this is a test email</p>")
    .WithHighPriority()
    .Build();
```

on the `Message` class you will find a method called `Compose()`, this method will give you a fluent API to compose your email so use the `'.'` and intellisense to see all the available function to compose you message, once you're done, call `Build()` to create an instance of the `Message`.

now we have a message let's try to send it.

### 2- EDPs [Email Delivery Provider]

EDPs are what actually used to send the emails under the hood, when you install Email.NET you get an EDP by default which is `SmtpEmailDeliveryProvider` that you can use to send emails using SMTP.

we have also other EDPs that you can use, but they exist in a separate packages:

- **[Email.NET.Socketlabs](https://www.nuget.org/packages/Email.Net.Socketlabs/):** to send emails using Socketlabs.
- **[Email.NET.SendGrid](https://www.nuget.org/packages/Email.Net.SendGrid/):** to send emails using SendGrid.
- **[Email.NET.MailKit](https://www.nuget.org/packages/Email.Net.MailKit/):** to send emails using MailKit.
- **[Email.NET.Mailgun](https://www.nuget.org/packages/Email.Net.Mailgun/):** to send emails using Mailgun.
- **[Email.NET.AmazonSES](https://www.nuget.org/packages/Email.Net.AmazonSES/):** to send emails using AmazonSES.

and we will be adding more in the future, but if you want to create your own EDP you can follow this [tutorial](#) and you will learn how to build one.

### 3- EmailService

the email service is what you will be interacting with to send emails, to create an instance of email service use can use the email `EmailServiceFactory`

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

        /* set the default EDP to be used for sending the emails */
        options.DefaultEmailDeliveryProvider = SmtpEmailDeliveryProvider.Name;
    })
    // register the EDPs
    .UseSmtp(options => options.UseGmailSmtp("your-email@gmail.com", "password"))
    .Create();
```

on the `EmailServiceFactory` class you will find a static property `Instance` that give you an instance of the factory, than you will have access to three methods on the factory:

- **UseOptions():** to configure the email service options.
- **UseEDP():** to register the EDPs to be used for sending emails.
- **Create():** to create an instance of the EmailService.

starting with `UseOptions()` you can configure the `EmailService` options, there are three options:

- **PauseSending:** to pause the sending of emails, if set to true nothing will be sent.
- **DefaultFrom:** you can set the default Sender email, so that you don't have to do it each time on the message, note that if you have specified a Sender email on the message this value will be ignored.
- **DefaultEmailDeliveryProvider:** specify the default EDP that should be used to send the emails, because you can configure multiple EDPs you should indicate which one you want to be used.

`UseEDP()` takes an instance of the EDP, like so: `UseEDP(new SmtpEmailDeliveryProvider(configuration))`, but you're not going to use this method, instead you will use the extension methods given to you by the EDPs as we seen on the example above, the SMTP EDP has an extension method `UseSmtp()` that will allow you to register it.

finally `Create()` will simply create an instance of the `EmailService`.

now you have an instance of the `EmailService` you can start sending emails.

```csharp
// get the email service
var emailService = EmailServiceFactory.Instance
    .UseOptions(options =>
    {
        options.PauseSending = false;
        options.DefaultFrom = new MailAddress("from@email.net");
        options.DefaultEmailDeliveryProvider = SmtpEmailDeliveryProvider.Name;
    })
    .UseSmtp(options => options.UseGmailSmtp("your-email@gmail.com", "password"))
    .Create();

// create the message
var message = Message.Compose()
    .To("to@email.net")
    .WithSubject("test email")
    .WithPlainTextContent("this is a test email")
    .WithHtmlContent("<p>this is a test email</p>")
    .WithHighPriority()
    .Build();

// send the message
var result = emailService.Send(message);
```

for full documentation check the [Wiki](https://github.com/YoussefSell/Email.Net/wiki) page.
