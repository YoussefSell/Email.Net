
// create the email service instance
var mailer = EmailServiceFactory.Instance
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

        /* to use a deferent Channel as your default one, just uncomment the one you need */
        //options.DefaultEmailDeliveryChannel = SocketLabsEmailDeliveryChannel.Name;
        //options.DefaultEmailDeliveryChannel = AmazonSESEmailDeliveryChannel.Name;
        //options.DefaultEmailDeliveryChannel = SendgridEmailDeliveryChannel.Name;
        //options.DefaultEmailDeliveryChannel = MailKitEmailDeliveryChannel.Name;
        //options.DefaultEmailDeliveryChannel = MailgunEmailDeliveryChannel.Name;
    })
    // register the Channels
    .UseSmtp(options => options.UseGmailSmtp("your-email@gmail.com", "password"))
    .UseMailKit(options => options.UseOutlookSmtp("your-email@outlook.com", "password"))
    .UseSocketlabs(apiKey: "", serverId: 0)
    .UseMailgun(apiKey: "", domain: "")
    .UseSendGrid(apiKey: "")
    .Create();

/* compose the email message */
var message = EmailMessage.Compose()
    .To("to@email.net")
    .WithPlainTextContent("this is a test email")
    .WithHtmlContent("<p>this is a test email</p>")
    .WithHighPriority()
    .Build();

/* send the message, this will use the default Channel set in the option */
var result = await mailer.SendAsync(message);

/*
 * if you want to send the email using a deferent Channel you just need to pass the name of the channel 
 * the channel needs to be registered in order to use it 
 */
//var result = mailer.Send(message, SocketLabsEmailDeliveryChannel.Name);

/*
 * if the Channel is not registered you can use an instance of the Channel instead.
 */
//var myChannel = new SocketLabsEmailDeliveryChannel(new SocketLabsEmailDeliveryChannelOptions { ApiKey = "", DefaultServerId = 0});
//var result = mailer.Send(message, myChannel);

Console.WriteLine("sent: {0}", result.IsSuccess);