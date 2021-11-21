var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// add Email.Net configuration
builder.Services.AddEmailNet(op =>
{
    op.PauseSending = false;
    op.DefaultFrom = new System.Net.Mail.MailAddress("from@email.net");
    op.DefaultEmailDeliveryProvider = SmtpEmailDeliveryProvider.Name;
})
.UseSmtp(op => op.UseGmailSmtp("", ""))
.UseMailgun(apiKey: "", domain: "")
.UseSendGrid(apiKey: "")
.UseSocketlabs(apiKey: "", serverId: 1)
.UseMailKit(op => op.UseGmailSmtp("", ""));

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
