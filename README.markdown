![alt text](https://github.com/lukencode/FluentEmail/raw/master/assets/fluentemail_logo_64x64.png "FluentEmail")

# FluentEmail - All in one email sender for .NET and .NET Core
The easiest way to send email from .NET and .NET Core. Use Razor for email templates and send using SendGrid, MailGun, SMTP and more.

Maintained by Luke Lowrey - follow me  on twitter **[@lukencode](https://twitter.com/lukencode)** for updates. See my blog for a detailed guide [A complete guide to send email in .NET](https://lukelowrey.com/dotnet-email-guide-2021/) 


## Nuget Packages

### Core Library

* [FluentEmail.Core](src/FluentEmail.Core) - Just the domain model. Includes very basic defaults, but is also included with every other package here.
* [FluentEmail.Smtp](src/Senders/FluentEmail.Smtp) - Send email via SMTP server.

### Renderers

* [FluentEmail.Razor](src/Renderers/FluentEmail.Razor) - Generate emails using Razor templates. Anything you can do in ASP.NET is possible here. Uses the [RazorLight](https://github.com/toddams/RazorLight) project under the hood. 
* [FluentEmail.Liquid](src/Renderers/FluentEmail.Liquid) - Generate emails using [Liquid templates](https://shopify.github.io/liquid/). Uses the [Fluid](https://github.com/sebastienros/fluid) project under the hood. 

### Mail Provider Integrations

* [FluentEmail.Mailgun](src/Senders/FluentEmail.Mailgun) - Send emails via MailGun's REST API.
* [FluentEmail.SendGrid](src/Senders/FluentEmail.SendGrid) - Send email via the SendGrid API.
* [FluentEmail.Mailtrap](src/Senders/FluentEmail.Mailtrap) - Send emails to Mailtrap. Uses [FluentEmail.Smtp](src/Senders/FluentEmail.Smtp) for delivery.
* [FluentEmail.MailKit](src/Senders/FluentEmail.MailKit) - Send emails using the [MailKit](https://github.com/jstedfast/MailKit) email library.

## Basic Usage
```csharp
var email = await Email
    .From("john@email.com")
    .To("bob@email.com", "bob")
    .Subject("hows it going bob")
    .Body("yo bob, long time no see!")
    .SendAsync();
```


## Dependency Injection

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
Example to take a dependency on IFluentEmail:

```c#
public class EmailService {

   private IFluentEmail _fluentEmail;

   public EmailService(IFluentEmail fluentEmail) {
     _fluentEmail = fluentEmail;
   }

   public async Task Send() {
     await _fluentEmail.To("hellO@gmail.com")
     .Body("The body").SendAsync();
   }
}

```



## Using a Razor template

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

## Using a Liquid template

[Liquid templates](https://shopify.github.io/liquid/) are a more secure option for Razor templates as they run in more restricted environment.
While Razor templates have access to whole power of CLR functionality like file access, they also
are more insecure if templates come from untrusted source. Liquid templates also have the benefit of being faster
to parse initially as they don't need heavy compilation step like Razor templates do.

Model properties are exposed directly as properties in Liquid templates so they also become more compact.

See [Fluid samples](https://github.com/sebastienros/fluid) for more examples.

```csharp
// Using Liquid templating package (or set using AddLiquidRenderer in services)

// file provider is used to resolve layout files if they are in use
var fileProvider = new PhysicalFileProvider(Path.Combine(someRootPath, "EmailTemplates"));
var options = new LiquidRendererOptions
{
    FileProvider = fileProvider
};

Email.DefaultRenderer = new LiquidRenderer(Options.Create(options));

// template which utilizes layout
var template = @"
{% layout '_layout.liquid' %}
Dear {{ Name }}, You are totally {{ Compliment }}.";

var email = Email
    .From("bob@hotmail.com")
    .To("somedude@gmail.com")
    .Subject("woo nuget")
    .UsingTemplate(template, new ViewModel { Name = "Luke", Compliment = "Awesome" });
```

## Sending Emails

```csharp
// Using Smtp Sender package (or set using AddSmtpSender in services)
Email.DefaultSender = new SmtpSender();

//send normally
email.Send();

//send asynchronously
await email.SendAsync();
```

## Template File from Disk

```csharp
var email = Email
    .From("bob@hotmail.com")
    .To("somedude@gmail.com")
    .Subject("woo nuget")
    .UsingTemplateFromFile($"{Directory.GetCurrentDirectory()}/Mytemplate.cshtml", new { Name = "Rad Dude" });
```

## Embedded Template File

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

