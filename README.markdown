![alt text](https://github.com/lukencode/FluentEmail/raw/master/assets/fluentemail_logo_64x64.png "FluentEmail")

# FluentEmail - All in one email sender for .NET and .NET Core
The easiest way to send email from .NET and .NET Core. Use Razor for email templates and send using SendGrid, MailGun, SMTP and more.


## Nuget Packages

[FluentEmail.Core](src/FluentEmail.Core) - Just the domain model. Includes very basic defaults, but is also included with every other package here.

[FluentEmail.Smtp](src/Senders/FluentEmail.Smtp) - Send email via SMTP server.

[FluentEmail.Razor](src/Renderers/FluentEmail.Razor) - Generate emails using Razor templates. Anything you can do in ASP.NET is possible here. Uses the [RazorLight](https://github.com/toddams/RazorLight) project under the hood. 

[FluentEmail.Mailgun](src/Senders/FluentEmail.Mailgun) - Send emails via MailGun's REST API.

[FluentEmail.SendGrid](src/Senders/FluentEmail.SendGrid) - Send email via the SendGrid API.

[FluentEmail.Mailtrap](src/Senders/FluentEmail.Mailtrap) - Send emails to Mailtrap. Uses [FluentEmail.Smtp](src/Senders/FluentEmail.Smtp) for delivery.

[FluentEmail.MailKit](src/Senders/FluentEmail.MailKit) - Send emails using the [MailKit](https://github.com/jstedfast/MailKit) email library.

**Basic Usage**
```csharp
var email = await Email
    	.From("john@email.com")
    	.To("bob@email.com", "bob")
    	.Subject("hows it going bob")
    	.Body("yo bob, long time no see!")
	.SendAsync();
```


**Dependency Injection**

Configure FluentEmail in startup.cs with these helper methods. This will inject IFluentEmail (send a single email) and IFluentEmailFactory (used to send multiple emails in a single context) with the 
ISender and ITemplateRenderer configured using AddRazorRenderer(), AddSmtpSender() or other packages.

```csharp
public void ConfigureServices(IServiceCollection services)
{
	services
		.AddFluentEmail("fromemail@test.test")
		.AddRazorRenderer()
		.AddSmtpSender("localhost", 25);
}
```

**Using a template**

```csharp
// Using Razor templating package (or set using AddRazorRenderer in services)
Email.DefaultRenderer = new RazorRenderer();

var template = "Dear @Model.Name, You are totally @Model.Compliment.";

var email = Email
    .From("bob@hotmail.com")
    .To("somedude@gmail.com")
    .Subject("woo nuget")
    .UsingTemplate(template, new { Name = "Luke", Compliment = "Awesome" });
```

**Sending Emails**

```csharp
// Using Smtp Sender package (or set using AddSmtpSender in services)
Email.DefaultSender = new SmtpSender();

//send normally
email.Send();

//send asynchronously
await email.SendAsync();
```

**Template File from Disk**  

```csharp
var email = Email
    .From("bob@hotmail.com")
    .To("somedude@gmail.com")
    .Subject("woo nuget")
    .UsingTemplateFromFile($"{Directory.GetCurrentDirectory()}/Mytemplate.cshtml", new { Name = "Rad Dude" });
```

**Embedded Template File**  

**Note for .NET Core 2 users:** You'll need to add the following line to the project containing any embedded razor views. See [this issue for more details](https://github.com/aspnet/Mvc/issues/6021).

```xml
<MvcRazorExcludeRefAssembliesFromPublish>false</MvcRazorExcludeRefAssembliesFromPublish>
```

```csharp
var email = new Email("bob@hotmail.com")
	.To("benwholikesbeer@twitter.com")
	.Subject("Hey cool name!")
	.UsingTemplateFromEmbedded("Example.Project.Namespace.template-name.cshtml", 
		new { Name = "Bob" }, 
		TypeFromYourEmbeddedAssembly.GetType().GetTypeInfo().Assembly);
```

**More Info**

<a href="http://lukencode.com/2018/07/01/send-email-in-dotnet-core-with-fluent-email/">Sending email in .NET Core with FluentEmail</a>


**[The future of FluentEmail](https://lukelowrey.com/fluentemail-future/)**
Looking for collaborators, discussion and hoping to keep improving the library!
