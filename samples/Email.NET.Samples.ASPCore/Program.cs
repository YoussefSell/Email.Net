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

    /* set the default EDP to be used for sending the emails */
    options.DefaultEmailDeliveryProvider = SmtpEmailDeliveryProvider.Name;

    /* to use a deferent EDP as your default one, just uncomment the one you need */
    //options.DefaultEmailDeliveryProvider = SocketLabsEmailDeliveryProvider.Name;
    //options.DefaultEmailDeliveryProvider = AmazonSESEmailDeliveryProvider.Name;
    //options.DefaultEmailDeliveryProvider = SendgridEmailDeliveryProvider.Name;
    //options.DefaultEmailDeliveryProvider = MailKitEmailDeliveryProvider.Name;
    //options.DefaultEmailDeliveryProvider = MailgunEmailDeliveryProvider.Name;
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

app.Run();
