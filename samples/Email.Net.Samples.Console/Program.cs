
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

        /* set the default EDP to be used for sending the emails */
        options.DefaultEmailDeliveryProvider = SmtpEmailDeliveryProvider.Name;

        /* to use a deferent EDP as your default one, just uncomment the one you need */
        //options.DefaultEmailDeliveryProvider = SocketLabsEmailDeliveryProvider.Name;
        //options.DefaultEmailDeliveryProvider = AmazonSESEmailDeliveryProvider.Name;
        //options.DefaultEmailDeliveryProvider = SendgridEmailDeliveryProvider.Name;
        //options.DefaultEmailDeliveryProvider = MailKitEmailDeliveryProvider.Name;
        //options.DefaultEmailDeliveryProvider = MailgunEmailDeliveryProvider.Name;
    })
    // register the EDPs
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

/* send the message, this will use the default EDP set in the option */
var result = mailer.Send(message);

/*
 * if you want to send the email using a deferent EDP you just need to pass the name of the edp 
 * the edp needs to be registered in order to use it 
 */
//var result = mailer.Send(message, SocketLabsEmailDeliveryProvider.Name);

/*
 * if the EDP is not registered you can use an instance of the EDP instead.
 */
//var myEdp = new SocketLabsEmailDeliveryProvider(new SocketLabsEmailDeliveryProviderOptions { ApiKey = "", DefaultServerId = 0});
//var result = mailer.Send(message, myEdp);
