var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// add Email.Net configuration
builder.Services.AddEmailNet(options =>
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
.UseSmtp(options => options.UseGmailSmtp("your-email@gmail.com", "password"))
.UseMailKit(options => options.UseOutlookSmtp("your-email@outlook.com", "password"))
.UseSocketlabs(apiKey: "", serverId: 0)
.UseMailgun(apiKey: "", domain: "")
.UseSendGrid(apiKey: "");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

await app.RunAsync();
