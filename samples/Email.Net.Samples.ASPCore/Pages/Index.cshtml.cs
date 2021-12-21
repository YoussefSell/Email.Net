namespace Email.Net.Samples.ASPCore.Pages;

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

        /* send the message, this will use the default EDP set in the option */
        var result = _emailService.Send(message);

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

        _logger.LogInformation("sent: {result}", result.IsSuccess);
    }
}